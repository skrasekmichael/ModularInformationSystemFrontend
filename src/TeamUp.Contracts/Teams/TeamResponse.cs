namespace TeamUp.Contracts.Teams;

public sealed class TeamResponse
{
	public required string Name { get; set; }
	public required List<TeamMemberResponse> Members { get; set; }
	public required List<EventTypeResponse> EventTypes { get; set; }
}
