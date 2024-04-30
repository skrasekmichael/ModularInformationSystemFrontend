using TeamUp.Contracts.Teams;

namespace TeamUp.Client.Messages;

public sealed record TeamDeletedMessage
{
	public required TeamId TeamId { get; init; }
}
