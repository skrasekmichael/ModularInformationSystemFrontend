using System.ComponentModel.DataAnnotations;

using TeamUp.Contracts.Teams;

namespace TeamUp.Client.Pages.Team.Panels;

public sealed class ChangeTeamRoleInput
{
	[Required]
	public required TeamRole Role { get; set; }

	public string TargetMember { get; set; } = string.Empty;

	public void Reset()
	{
		Role = TeamRole.Member;
	}
}
