using RailwayResult;

using TeamUp.Contracts.Teams;

namespace TeamUp.ApiLayer;

public sealed partial class ApiClient
{
	public Task<Result<List<TeamSlimResponse>>> GetMyTeamsAsync(CancellationToken ct) =>
		SendAsync<List<TeamSlimResponse>>(HttpMethod.Get, "/api/v1/teams", ct);

	public Task<Result<TeamId>> CreateTeamAsync(CreateTeamRequest request, CancellationToken ct) =>
		SendAsync<CreateTeamRequest, TeamId>(HttpMethod.Post, "/api/v1/teams", request, ct);

	public Task<Result> DeleteTeamAsync(TeamId teamId, CancellationToken ct) =>
		SendAsync(HttpMethod.Delete, $"/api/v1/teams/{teamId.Value}", ct);

	public Task<Result<TeamResponse>> GetTeamAsync(TeamId teamId, CancellationToken ct) =>
		SendAsync<TeamResponse>(HttpMethod.Get, $"/api/v1/teams/{teamId.Value}", ct);
}
