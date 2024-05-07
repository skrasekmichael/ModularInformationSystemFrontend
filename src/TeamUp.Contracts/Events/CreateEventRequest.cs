using System.ComponentModel.DataAnnotations;

using TeamUp.Contracts.Teams;

namespace TeamUp.Contracts.Events;

public sealed record CreateEventRequest
{
	public required EventTypeId EventTypeId { get; set; }

	[DataType(DataType.DateTime)]
	public required DateTime FromUtc { get; set; }

	[DataType(DataType.DateTime)]
	public required DateTime ToUtc { get; set; }

	[DataType(DataType.Text)]
	public required string Description { get; set; }

	[DataType(DataType.Time)]
	public required TimeSpan MeetTime { get; set; }

	[DataType(DataType.Time)]
	public required TimeSpan ReplyClosingTimeBeforeMeetTime { get; set; }
}
