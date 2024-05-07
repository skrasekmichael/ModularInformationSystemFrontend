using TeamUp.Contracts.Teams;

namespace TeamUp.DAL.Messages;

public sealed record TeamDeletedMessage
{
	public required TeamId TeamId { get; init; }
}
