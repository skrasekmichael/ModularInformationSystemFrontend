using System.ComponentModel.DataAnnotations;

namespace TeamUp.Client.Pages.Team.Panels;

public sealed class InviteUserInput
{
	[Required]
	[EmailAddress]
	public required string Email { get; set; }

	public IDictionary<string, string[]>? Errors { get; set; }
}
