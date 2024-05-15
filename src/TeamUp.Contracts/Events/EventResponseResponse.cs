using TeamUp.Contracts.Teams;

namespace TeamUp.Contracts.Events;

public sealed class EventResponseResponse
{
	public required TeamMemberId TeamMemberId { get; init; }
	public required string TeamMemberNickname { get; init; }
	public required ReplyType Type { get; set; }
	public required string Message { get; set; }
	public required DateTime TimeStampUtc { get; set; }
}
