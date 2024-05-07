namespace TeamUp.Contracts.Teams;

public sealed class TeamSlimResponse
{
	public required TeamId TeamId { get; set; }
	public required string Name { get; set; }
}
