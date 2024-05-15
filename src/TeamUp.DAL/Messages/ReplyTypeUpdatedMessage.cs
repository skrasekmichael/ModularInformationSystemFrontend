using TeamUp.Contracts.Events;
using TeamUp.Contracts.Teams;

namespace TeamUp.DAL.Messages;

public sealed class ReplyTypeUpdatedMessage
{
	public required TeamId TeamId { get; init; }
	public required EventId EventId { get; init; }
	public required TeamMemberId MemberId { get; init; }


}
