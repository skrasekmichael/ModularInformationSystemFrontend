using CommunityToolkit.Mvvm.Messaging;

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

	public TeamService(ApiClient client, IMessenger messenger, CacheFacade cache)
	{
		_client = client;
		_messenger = messenger;
		_cache = cache;

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
			await _cache.UpdateAsync<TeamResponse>($"team-{teamId.Value}", team =>
			{
				var toRemove = team.Members.Find(member => member.Id == memberId);
				if (toRemove is not null)
				{
					team.Members.Remove(toRemove);
				}
			}, ct);
		});
	}

	public Task<Result<TeamResponse>> GetTeamAsync(TeamId teamId, bool forceFetch, CancellationToken ct)
	{
		return _cache.GetAsync($"team-{teamId.Value}", () => _client.GetTeamAsync(teamId, ct), TimeSpan.FromMinutes(10), forceFetch, ct);
	}
}
