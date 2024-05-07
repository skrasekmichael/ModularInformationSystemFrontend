using System.ComponentModel.DataAnnotations;

namespace TeamUp.Contracts.Teams;

public sealed record UpsertEventTypeRequest
{
	[DataType(DataType.Text)]
	public required string Name { get; set; }

	[DataType(DataType.Text)]
	public required string Description { get; set; }
}
