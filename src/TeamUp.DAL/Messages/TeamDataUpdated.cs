using TeamUp.Contracts.Teams;

namespace TeamUp.DAL.Messages;

public sealed record TeamDataUpdatedMessage
{
	public required TeamId TeamId { get; init; }
	public required TeamResponse Team { get; init; }
}
