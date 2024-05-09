using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Extensions.Logging;

using RailwayResult;
using RailwayResult.FunctionalExtensions;

using TeamUp.Contracts.Teams;
using TeamUp.DAL.Api;
using TeamUp.DAL.Cache;
using TeamUp.DAL.Messages;

namespace TeamUp.DAL.Services;

public sealed class TeamService
{
	private readonly ApiClient _client;
	private readonly IMessenger _messenger;
	private readonly CacheFacade _cache;
	private readonly IAuthService _authService;

	public TeamService(ApiClient client, IMessenger messenger, CacheFacade cache, IAuthService authService)
	{
		_client = client;
		_messenger = messenger;
		_cache = cache;
		_authService = authService;

		messenger.Register<TeamService, TeamDeletedMessage>(this, async (self, msg) =>
		{
			await _cache.DeleteAsync($"team-{msg.TeamId.Value}", CancellationToken.None);
			await _cache.UpdateAsync<List<TeamSlimResponse>>("my-teams", teams =>
			{
				var toRemove = teams.Find(team => team.TeamId == msg.TeamId);
				if (toRemove is not null)
				{
					teams.Remove(toRemove);
				}
			}, CancellationToken.None);
		});
	}

	public Task<Result<List<TeamSlimResponse>>> GetMyTeamsAsync(bool forceFetch, CancellationToken ct)
	{
		return _cache.GetAsync("my-teams", () => _client.GetMyTeamsAsync(ct), TimeSpan.FromHours(1), forceFetch, ct);
	}

	public async Task<Result<TeamId>> CreateTeamAsync(CreateTeamRequest request, CancellationToken ct)
	{
		var result = await _client.CreateTeamAsync(request, ct);
		return await result.TapAsync(async teamId =>
		{
			var newTeam = new TeamSlimResponse
			{
				Name = request.Name,
				TeamId = teamId
			};

			await _cache.UpdateAsync<List<TeamSlimResponse>>("my-teams", teams => teams.Add(newTeam), ct);

			_messenger.Send(new TeamCreatedMessage
			{
				Name = newTeam.Name,
				TeamId = result.Value
			});
		});
	}

	public async Task<Result> DeleteTeamAsync(TeamId teamId, CancellationToken ct)
	{
		var result = await _client.DeleteTeamAsync(teamId, ct);
		return result.Tap(() =>
		{
			_messenger.Send(new TeamDeletedMessage
			{
				TeamId = teamId
			});
		});
	}

	public async Task<Result> RemoveTeamMemberAsync(TeamId teamId, TeamMemberId memberId, CancellationToken ct)
	{
		var result = await _client.RemoveTeamMemberAsync(teamId, memberId, ct);
		return await result.TapAsync(async () =>
		{
			var team = await _cache.UpdateAsync<TeamResponse>($"team-{teamId.Value}", team =>
			{
				var toRemove = team.Members.Find(member => member.Id == memberId);
				if (toRemove is not null)
				{
					team.Members.Remove(toRemove);
				}
			}, ct);

			if (team is not null)
			{
				_messenger.Send(new TeamDataUpdatedMessage
				{
					TeamId = teamId,
					Team = team
				});
			}
		});
	}

	public Task<Result<TeamResponse>> GetTeamAsync(TeamId teamId, bool forceFetch, CancellationToken ct)
	{
		return _cache.GetAsync($"team-{teamId.Value}", () => _client.GetTeamAsync(teamId, ct), TimeSpan.FromMinutes(10), forceFetch, ct);
	}

	public async Task<Result> ChangeNicknameAsync(TeamId teamId, ChangeNicknameRequest request, CancellationToken ct)
	{
		var result = await _client.ChangeNicknameAsync(teamId, request, ct);
		return await result.TapAsync(async () =>
		{
			var userId = await _authService.GetUserIdAsync();
			var team = await _cache.UpdateAsync<TeamResponse>($"team-{teamId.Value}", team =>
			{
				var toUpdate = team.Members.Find(member => member.UserId == userId);
				if (toUpdate is not null)
				{
					toUpdate.Nickname = request.Nickname;
				}
			}, ct);

			if (team is not null)
			{
				_messenger.Send(new TeamDataUpdatedMessage
				{
					TeamId = teamId,
					Team = team
				});
			}
		});
	}

	public async Task<Result> ChangeTeamRoleAsync(TeamId teamId, TeamMemberId memberId, UpdateTeamRoleRequest request, CancellationToken ct)
	{
		var result = await _client.UpdaterTeamRoleAsync(teamId, memberId, request, ct);
		return await result.TapAsync(async () =>
		{
			var team = await _cache.UpdateAsync<TeamResponse>($"team-{teamId.Value}", team =>
			{
				var toUpdate = team.Members.Find(member => member.Id == memberId);
				if (toUpdate is not null)
				{
					toUpdate.Role = request.Role;
				}
			}, ct);

			if (team is not null)
			{
				_messenger.Send(new TeamDataUpdatedMessage
				{
					TeamId = teamId,
					Team = team
				});
			}
		});
	}

	public async Task<Result> ChangeOwnershipAsync(TeamId teamId, TeamMemberId newOwnerId, CancellationToken ct)
	{
		var result = await _client.ChangeOwnershipAsync(teamId, newOwnerId, ct);
		return await result.TapAsync(async () =>
		{
			var team = await _cache.UpdateAsync<TeamResponse>($"team-{teamId.Value}", team =>
			{
				var originalOwner = team.Members.Find(member => member.Role.IsOwner());
				if (originalOwner is not null)
				{
					originalOwner.Role = TeamRole.Admin;
				}

				var newOwner = team.Members.Find(member => member.Id == newOwnerId);
				if (newOwner is not null)
				{
					newOwner.Role = TeamRole.Owner;
				}
			}, ct);

			if (team is not null)
			{
				_messenger.Send(new TeamDataUpdatedMessage
				{
					TeamId = teamId,
					Team = team
				});
			}
		});
	}
}
