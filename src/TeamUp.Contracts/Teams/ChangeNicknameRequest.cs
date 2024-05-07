using System.ComponentModel.DataAnnotations;

namespace TeamUp.Contracts.Teams;

public sealed record ChangeNicknameRequest
{
	[DataType(DataType.Text)]
	public required string Nickname { get; set; }
}
