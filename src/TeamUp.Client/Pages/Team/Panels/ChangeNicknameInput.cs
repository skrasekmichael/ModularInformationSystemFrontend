using System.ComponentModel.DataAnnotations;

namespace TeamUp.Client.Pages.Team.Panels;

public sealed class ChangeNicknameInput
{
	[Required]
	public required string Nickname { get; set; }

	public IDictionary<string, string[]>? Errors { get; set; }
}
