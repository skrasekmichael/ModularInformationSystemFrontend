namespace TeamUp.Contracts.Teams;

public sealed class EventTypeResponse
{
	public required EventTypeId Id { get; init; }
	public required string Name { get; set; }
	public required string Description { get; set; }
}
