using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using BitzArt.Blazor.Cookies;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using TeamUp.Contracts.Users;
using TeamUp.DAL;

namespace TeamUp.Services;

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
		var token = await GetTokenAsync();

		var handler = new JwtSecurityTokenHandler();
		var jsonToken = handler.ReadJwtToken(token);

		var guid = new Guid(jsonToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
		return UserId.FromGuid(guid);
	}

	public async Task LogoutAsync(string url = "/login", CancellationToken ct = default)
	{
		await _cookieService.RemoveAsync("JWT", ct);
		if (_authenticationStateProvider is ServerAuthenticationStateProvider authProvider)
		{
			//authProvider.Logout();
		}

		if (!string.IsNullOrWhiteSpace(url))
		{
			_navigationManager.NavigateTo(url);
		}
	}
}
