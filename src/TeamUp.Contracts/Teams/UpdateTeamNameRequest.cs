using System.ComponentModel.DataAnnotations;

namespace TeamUp.Contracts.Teams;

public sealed record UpdateTeamNameRequest
{
	[DataType(DataType.Text)]
	public required string Name { get; set; }
}
