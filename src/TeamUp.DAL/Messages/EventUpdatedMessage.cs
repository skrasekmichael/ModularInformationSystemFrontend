using TeamUp.Contracts.Events;

namespace TeamUp.DAL.Messages;

public sealed class EventUpdatedMessage
{
	public required EventSlimResponse Event { get; init; }
}
