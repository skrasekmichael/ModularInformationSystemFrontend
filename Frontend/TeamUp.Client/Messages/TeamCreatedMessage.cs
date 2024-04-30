using TeamUp.Contracts.Teams;

namespace TeamUp.Client.Messages;

public sealed record TeamCreatedMessage
{
	public required TeamId TeamId { get; init; }
	public required string Name { get; init; }
}
