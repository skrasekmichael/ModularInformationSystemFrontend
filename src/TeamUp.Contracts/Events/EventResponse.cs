﻿using TeamUp.Contracts.Teams;

namespace TeamUp.Contracts.Events;

public sealed class EventResponse
{
	public required EventTypeId EventTypeId { get; init; }
	public required string EventType { get; init; }
	public required DateTime FromUtc { get; init; }
	public required DateTime ToUtc { get; init; }
	public required string Description { get; init; }
	public required EventStatus Status { get; init; }
	public required TimeSpan MeetTime { get; init; }
	public required TimeSpan ReplyClosingTimeBeforeMeetTime { get; init; }
	public required List<EventResponseResponse> EventResponses { get; init; }
}
