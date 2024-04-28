using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace TeamUp.Client.Services;

public sealed record UserInfo
{
	public required string UserId { get; set; }
	public required string Email { get; set; }
	public required string Username { get; set; }
}


internal sealed class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
	private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

	private AuthenticationState State { get; set; } = Anonymous;

	public PersistentAuthenticationStateProvider(PersistentComponentState state)
	{
		if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
		{
			return;
		}

		var claims = new Claim[]
		{
			new(ClaimTypes.NameIdentifier, userInfo.UserId),
			new(ClaimTypes.Name, userInfo.Username),
			new(ClaimTypes.Email, userInfo.Email)
		};

		State = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "JWT")));
	}

	public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(State);

	public void Login(string jwt)
	{
		var handler = new JwtSecurityTokenHandler();
		var jsonToken = handler.ReadJwtToken(jwt);
		var identity = new ClaimsIdentity(jsonToken.Claims, "JWT");
		var user = new ClaimsPrincipal(identity);

		State = new AuthenticationState(user);
		NotifyAuthenticationStateChanged(Task.FromResult(State));
	}

	public void Logout()
	{
		State = Anonymous;
		//NotifyAuthenticationStateChanged(Task.FromResult(State));
	}
}
