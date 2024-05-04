using System.Security.Claims;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Logging;

using TeamUp.Contracts.Teams;
using TeamUp.Contracts.Users;

namespace TeamUp.Client.Components;

public sealed class TeamContext : ComponentBase
{
	[Parameter]
	[EditorRequired]
	public TeamResponse Team { get; set; } = null!;

	[Parameter]
	[EditorRequired]
	public Guid TeamGuid { get; set; } = default!;

	public TeamId TeamId { get; private set; } = null!;

	public TeamRole Role => Member?.Role ?? TeamRole.Member;

	public string Nickname => Member?.Nickname ?? "No Nickname";

	public TeamMemberResponse? Member { get; private set; }

	[Parameter]
	public RenderFragment<TeamContext>? ChildContent { get; set; }

	[Inject]
	private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

	protected override async Task OnParametersSetAsync()
	{
		TeamId = TeamId.FromGuid(TeamGuid);

		var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
		var userIdString = state.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
		var userId = UserId.FromGuid(new Guid(userIdString));

		Member = Team.Members.First(member => member.UserId == userId);
	}

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenRegion(GetHashCode());

		builder.OpenComponent<CascadingValue<TeamContext>>(0);
		builder.AddComponentParameter(1, "Value", this);
		builder.AddComponentParameter(2, "ChildContent", ChildContent?.Invoke(this));
		builder.CloseComponent();

		builder.CloseRegion();
	}
}
