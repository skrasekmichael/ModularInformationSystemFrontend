using System.ComponentModel.DataAnnotations;

using TeamUp.Contracts.Teams;

namespace TeamUp.Contracts.Invitations;

public sealed record InviteUserRequest
{
	public required TeamId TeamId { get; set; }

	[DataType(DataType.EmailAddress)]
	public required string Email { get; set; }
}
