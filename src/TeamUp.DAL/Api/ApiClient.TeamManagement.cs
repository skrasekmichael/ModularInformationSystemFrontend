﻿using RailwayResult;

using TeamUp.Contracts.Events;
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

	public Task<Result> ChangeNicknameAsync(TeamId teamId, ChangeNicknameRequest request, CancellationToken ct) =>
		SendAsync(HttpMethod.Put, $"/api/v1/teams/{teamId.Value}/nickname", request, ct);

	public Task<Result> UpdaterTeamRoleAsync(TeamId teamId, TeamMemberId memberId, UpdateTeamRoleRequest request, CancellationToken ct) =>
		SendAsync(HttpMethod.Put, $"/api/v1/teams/{teamId.Value}/members/{memberId.Value}/role", request, ct);

	public Task<Result<EventTypeId>> CreateEventTypeAsync(TeamId teamId, UpsertEventTypeRequest request, CancellationToken ct) =>
		SendAsync<UpsertEventTypeRequest, EventTypeId>(HttpMethod.Post, $"/api/v1/teams/{teamId.Value}/event-types", request, ct);


	public Task<Result> ChangeOwnershipAsync(TeamId teamId, TeamMemberId memberId, CancellationToken ct) =>
		SendAsync(HttpMethod.Put, $"/api/v1/teams/{teamId.Value}/owner", memberId.Value, ct);


	public Task<Result<List<EventSlimResponse>>> GetEventsAsync(TeamId teamId, CancellationToken ct) =>
		SendAsync<List<EventSlimResponse>>(HttpMethod.Get, $"/api/v1/teams/{teamId.Value}/events", ct);

	public Task<Result<EventResponse>> GetEventAsync(TeamId teamId, EventId eventId, CancellationToken ct) =>
		SendAsync<EventResponse>(HttpMethod.Get, $"/api/v1/teams/{teamId.Value}/events/{eventId.Value}", ct);

	public Task<Result<EventId>> CreateEventAsync(TeamId teamId, CreateEventRequest request, CancellationToken ct) =>
		SendAsync<CreateEventRequest, EventId>(HttpMethod.Post, $"/api/v1/teams/{teamId.Value}/events", request, ct);

	public Task<Result> UpsertEventReply(TeamId teamId, EventId eventId, UpsertEventReplyRequest request, CancellationToken ct) =>
		SendAsync(HttpMethod.Put, $"/api/v1/teams/{teamId.Value}/events/{eventId.Value}", request, ct);


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
