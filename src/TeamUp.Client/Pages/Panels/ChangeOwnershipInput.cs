using TeamUp.Contracts.Teams;

namespace TeamUp.Client.Pages.Panels;

public class ChangeOwnershipInput
{
	public IEnumerable<TeamMemberResponse> SelectedMember { get; set; } = Array.Empty<TeamMemberResponse>();

	public List<TeamMemberResponse> Members { get; set; } = [];
}
