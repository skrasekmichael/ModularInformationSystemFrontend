using System.ComponentModel.DataAnnotations;

namespace TeamUp.Client.Pages.Panels;

public sealed class EmailInput
{
	[Required]
	[EmailAddress]
	public required string Email { get; set; }

	public IDictionary<string, string[]>? Errors { get; set; }
}
