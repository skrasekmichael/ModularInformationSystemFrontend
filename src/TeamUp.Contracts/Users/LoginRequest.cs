using System.ComponentModel.DataAnnotations;

namespace TeamUp.Contracts.Users;

public sealed record LoginRequest
{
	[DataType(DataType.EmailAddress)]
	public required string Email { get; set; }

	[DataType(DataType.Password)]
	public required string Password { get; set; }
}
