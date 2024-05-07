using CommunityToolkit.Mvvm.Messaging;

using RailwayResult;

using TeamUp.Contracts.Invitations;
using TeamUp.Contracts.Teams;
using TeamUp.DAL.Api;
using TeamUp.DAL.Cache;

namespace TeamUp.DAL.Services;

public sealed class InvitationsService
{
	private readonly ApiClient _client;
	private readonly IMessenger _messenger;
	private readonly CacheFacade _cache;

	public InvitationsService(ApiClient client, IMessenger messenger, CacheFacade cache)
	{
		_client = client;
		_messenger = messenger;
		_cache = cache;
	}

	public Task<Result<List<InvitationResponse>>> GetMyInvitationsAsync(bool forceFetch, CancellationToken ct)
	{
		return _cache.GetAsync("my-invitations", () => _client.GetMyInvitationsAsync(ct), TimeSpan.FromMinutes(5), forceFetch, ct);
	}

	public Task<Result<List<TeamInvitationResponse>>> GetTeamInvitationsAsync(TeamId teamId, bool forceFetch, CancellationToken ct)
	{
		return _cache.GetAsync($"team-{teamId.Value}-invitations", () => _client.GetTeamInvitationsAsync(teamId, ct), TimeSpan.FromMinutes(10), forceFetch, ct);
	}

	public async Task<Result> AcceptInvitationAsync(InvitationId invitationId, CancellationToken ct)
	{
		var result = await _client.AcceptInvitationAsync(invitationId, ct);
		return await result.TapAsync(async () =>
		{
			await _cache.UpdateAsync<List<InvitationResponse>>("my-invitations", invitations =>
			{
				var toRemove = invitations.Find(invitation => invitation.Id == invitationId);
				if (toRemove is not null)
				{
					invitations.Remove(toRemove);
				}
			}, ct);
		});
	}

	public async Task<Result> CancelInvitationAsync(TeamId teamId, InvitationId invitationId, CancellationToken ct)
	{
		var result = await _client.RemoveInvitationAsync(invitationId, ct);
		return await result.TapAsync(async () =>
		{
			await _cache.UpdateAsync<List<TeamInvitationResponse>>($"team-{teamId.Value}-invitations", invitations =>
			{
				var toRemove = invitations.Find(invitation => invitation.Id == invitationId);
				if (toRemove is not null)
				{
					invitations.Remove(toRemove);
				}
			}, ct);
		});
	}

	public async Task<Result> DeclineInvitationAsync(InvitationId invitationId, CancellationToken ct)
	{
		var result = await _client.RemoveInvitationAsync(invitationId, ct);
		return await result.TapAsync(async () =>
		{
			await _cache.UpdateAsync<List<InvitationResponse>>("my-invitations", invitations =>
			{
				var toRemove = invitations.Find(invitation => invitation.Id == invitationId);
				if (toRemove is not null)
				{
					invitations.Remove(toRemove);
				}
			}, ct);
		});
	}

	public Task<Result> InviteUserAsync(InviteUserRequest request, CancellationToken ct)
	{
		return _client.InviteUserAsync(request, ct);
	}
}
