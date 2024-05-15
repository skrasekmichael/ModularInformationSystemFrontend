using CommunityToolkit.Mvvm.Messaging;

using RailwayResult;
using RailwayResult.FunctionalExtensions;

using TeamUp.Contracts.Events;
using TeamUp.Contracts.Teams;
using TeamUp.DAL.Api;
using TeamUp.DAL.Cache;
using TeamUp.DAL.Messages;

namespace TeamUp.DAL.Services;

public sealed class EventService
{
	private readonly ApiClient _client;
	private readonly IMessenger _messenger;
	private readonly CacheFacade _cache;
	private readonly IAuthService _authService;
	private readonly TeamService _teamService;

	public EventService(ApiClient client, IMessenger messenger, CacheFacade cache, IAuthService authService, TeamService teamService)
	{
		_client = client;
		_messenger = messenger;
		_cache = cache;
		_authService = authService;
		_teamService = teamService;
	}

	public Task<Result<List<EventSlimResponse>>> GetEventsAsync(TeamId teamId, bool forceFetch, CancellationToken ct)
	{
		return _cache.GetAsync($"team-{teamId.Value}-events", async () =>
		{
			var eventResult = await _client.GetEventsAsync(teamId, ct);
			return eventResult.Tap(events =>
			{
				foreach (var e in events)
				{
					e.ReplyCount = [.. e.ReplyCount.OrderBy(x => (int)x.Type)];
				}
			});
		}, TimeSpan.FromMinutes(5), forceFetch, ct);
	}

	public Task<Result<EventResponse>> GetEventAsync(TeamId teamId, EventId eventId, bool forceFetch, CancellationToken ct)
	{
		return _cache.GetAsync($"event-{eventId.Value}", () => _client.GetEventAsync(teamId, eventId, ct), TimeSpan.FromMinutes(5), forceFetch, ct);
	}

	public async Task<Result<EventId>> CreateEventAsync(TeamId teamId, CreateEventRequest request, CancellationToken ct)
	{
		request.FromUtc = request.FromUtc.ToUniversalTime();
		request.ToUtc = request.ToUtc.ToUniversalTime();

		var result = await _client.CreateEventAsync(teamId, request, ct);
		return result.Tap(async eventId =>
		{
			var team = await _teamService.GetTeamAsync(teamId, false, ct);
			if (team.IsFailure)
			{
				return;
			}

			var eventType = team.Value.EventTypes.Find(et => et.Id == request.EventTypeId);
			if (eventType is null)
			{
				return;
			}

			var newEvent = new EventSlimResponse()
			{
				Id = eventId,
				Description = request.Description,
				EventType = eventType.Name,
				FromUtc = request.FromUtc,
				ToUtc = request.ToUtc.ToUniversalTime(),
				InitiatorResponse = null,
				MeetTime = request.MeetTime,
				ReplyClosingTimeBeforeMeetTime = request.ReplyClosingTimeBeforeMeetTime,
				ReplyCount = [],
				Status = EventStatus.Open
			};

			await _cache.UpdateAsync<List<EventSlimResponse>>($"team-{teamId.Value}-events", events => events.Add(newEvent), ct);
		});
	}

	public async Task<Result> UpsertEventReplyAsync(TeamId teamId, EventId eventId, TeamMemberResponse member, UpsertEventReplyRequest request, CancellationToken ct)
	{
		var result = await _client.UpsertEventReply(teamId, eventId, request, ct);
		var timestamp = DateTime.UtcNow;

		return await result.TapAsync(async () =>
		{
			var initiatorResponse = new EventResponseResponse
			{
				TeamMemberId = member.Id,
				Type = request.ReplyType,
				Message = request.Message,
				TeamMemberNickname = member.Nickname,
				TimeStampUtc = timestamp
			};

			await _cache.UpdateAsync<EventResponse>($"event-{eventId.Value}", targetEvent =>
			{
				var response = targetEvent.EventResponses.FirstOrDefault(response => response.TeamMemberId == member.Id);
				if (response is not null)
				{
					response.Message = request.Message;
					response.Type = request.ReplyType;
					response.TimeStampUtc = timestamp;
					return;
				}

				targetEvent.EventResponses.Add(initiatorResponse);
			}, ct);

			var slimResponses = await _cache.UpdateAsync<List<EventSlimResponse>>($"team-{teamId.Value}-events", events =>
			{
				var targetEvent = events.Find(e => e.Id == eventId);
				if (targetEvent is null)
				{
					return;
				}

				var oldIndex = targetEvent.ReplyCount.FindIndex(rt => rt.Type == targetEvent.InitiatorResponse?.Type);
				if (oldIndex != -1)
				{
					if (targetEvent.ReplyCount[oldIndex].Count == 1)
					{
						targetEvent.ReplyCount.RemoveAt(oldIndex);
					}
					else
					{
						targetEvent.ReplyCount[oldIndex] = new ReplyCountResponse
						{
							Type = targetEvent.ReplyCount[oldIndex].Type,
							Count = targetEvent.ReplyCount[oldIndex].Count - 1,
						};
					}
				}

				var newIndex = targetEvent.ReplyCount.FindIndex(rt => rt.Type == initiatorResponse.Type);
				if (newIndex != -1)
				{
					targetEvent.ReplyCount[newIndex] = new ReplyCountResponse
					{
						Type = targetEvent.ReplyCount[newIndex].Type,
						Count = targetEvent.ReplyCount[newIndex].Count + 1,
					};
				}
				else
				{
					targetEvent.ReplyCount.Add(new ReplyCountResponse
					{
						Type = initiatorResponse.Type,
						Count = 1
					});
					targetEvent.ReplyCount = [.. targetEvent.ReplyCount.OrderBy(x => (int)x.Type)];
				}

				targetEvent.InitiatorResponse = initiatorResponse;
			}, ct);

			var slimResponse = slimResponses?.Find(e => e.Id == eventId);
			if (slimResponse is not null)
			{
				_messenger.Send(new EventUpdatedMessage
				{
					Event = slimResponse
				});
			}
		});
	}
}
