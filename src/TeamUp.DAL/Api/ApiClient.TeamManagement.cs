using RailwayResult;

using TeamUp.Contracts.Invitations;
using TeamUp.Contracts.Teams;

namespace TeamUp.DAL.Api;

public sealed partial class ApiClient
{
	public Task<Result<List<TeamSlimResponse>>> GetMyTeamsAsync(CancellationToken ct) =>
		SendAsync<List<TeamSlimResponse>>(HttpMethod.Get, "/api/v1/teams", ct);

	public Task<Result<TeamId>> CreateTeamAsync(CreateTeamRequest request, CancellationToken ct) =>
		SendAsync<CreateTeamRequest, TeamId>(HttpMethod.Post, "/api/v1/teams", request, ct);

	public Task<Result> DeleteTeamAsync(TeamId teamId, CancellationToken ct) =>
		SendAsync(HttpMethod.Delete, $"/api/v1/teams/{teamId.Value}", ct);

	public Task<Result> RemoveTeamMemberAsync(TeamId teamId, TeamMemberId memberId, CancellationToken ct) =>
		SendAsync(HttpMethod.Delete, $"/api/v1/teams/{teamId.Value}/members/{memberId.Value}", ct);

	public Task<Result<TeamResponse>> GetTeamAsync(TeamId teamId, CancellationToken ct) =>
		SendAsync<TeamResponse>(HttpMethod.Get, $"/api/v1/teams/{teamId.Value}", ct);

	public Task<Result<List<InvitationResponse>>> GetMyInvitationsAsync(CancellationToken ct) =>
		SendAsync<List<InvitationResponse>>(HttpMethod.Get, "/api/v1/invitations", ct);

	public Task<Result<List<TeamInvitationResponse>>> GetTeamInvitationsAsync(TeamId teamId, CancellationToken ct) =>
		SendAsync<List<TeamInvitationResponse>>(HttpMethod.Get, $"/api/v1/invitations/teams/{teamId.Value}", ct);

	public Task<Result> AcceptInvitationAsync(InvitationId invitationId, CancellationToken ct) =>
		SendAsync(HttpMethod.Post, $"/api/v1/invitations/{invitationId.Value}/accept", ct);

	public Task<Result> RemoveInvitationAsync(InvitationId invitationId, CancellationToken ct) =>
		SendAsync(HttpMethod.Delete, $"/api/v1/invitations/{invitationId.Value}", ct);

	public Task<Result> InviteUserAsync(InviteUserRequest request, CancellationToken ct) =>
		SendAsync(HttpMethod.Post, "/api/v1/invitations", request, ct);
}
