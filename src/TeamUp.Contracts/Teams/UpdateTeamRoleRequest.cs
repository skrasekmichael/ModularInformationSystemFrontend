namespace TeamUp.Contracts.Teams;

public sealed record UpdateTeamRoleRequest
{
	public required TeamRole Role { get; set; }
}
