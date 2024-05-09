using System.Security.Claims;

using BitzArt.Blazor.Cookies;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using TeamUp.Contracts.Users;
using TeamUp.DAL;

namespace TeamUp.Client.Services;

public sealed class AuthService : IAuthService
{
	private readonly ICookieService _cookieService;
	private readonly AuthenticationStateProvider _authenticationStateProvider;
	private readonly NavigationManager _navigationManager;

	public AuthService(ICookieService cookieService, AuthenticationStateProvider authenticationStateProvider, NavigationManager navigationManager)
	{
		_cookieService = cookieService;
		_authenticationStateProvider = authenticationStateProvider;
		_navigationManager = navigationManager;
	}

	public async Task<string?> GetTokenAsync(CancellationToken ct = default)
	{
		var cookie = await _cookieService.GetAsync("JWT");
		return cookie?.Value;
	}

	public async Task<UserId> GetUserIdAsync()
	{
		var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
		var guid = new Guid(state.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
		return UserId.FromGuid(guid);
	}

	public async Task LogoutAsync(string url = "/login", CancellationToken ct = default)
	{
		await _cookieService.RemoveAsync("JWT", ct);
		if (_authenticationStateProvider is ClientAuthenticationStateProvider authProvider)
		{
			authProvider.Logout();
		}

		_navigationManager.NavigateTo(url);
	}
}
