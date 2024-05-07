using System.Security.Claims;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.AspNetCore.Components.Authorization;

using RailwayResult;

using TeamUp.Contracts.Users;
using TeamUp.DAL.Api;
using TeamUp.DAL.Cache;

namespace TeamUp.DAL.Services;

public sealed class LoginService
{
	private readonly ApiClient _client;
	private readonly IMessenger _messenger;
	private readonly ICacheStorage _cacheStorage;
	private readonly AuthenticationStateProvider _authStateProvider;

	public LoginService(ApiClient client, IMessenger messenger, ICacheStorage cacheStorage, AuthenticationStateProvider authStateProvider)
	{
		_client = client;
		_messenger = messenger;
		_cacheStorage = cacheStorage;
		_authStateProvider = authStateProvider;
	}

	public Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken ct)
	{
		return _client.LoginAsync(request, ct);
	}

	public async Task ValidateCacheAsync(CancellationToken ct)
	{
		var state = await _authStateProvider.GetAuthenticationStateAsync();
		var loggedInUser = new Guid(state.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

		var cacheOwner = await _cacheStorage.GetRecordAsync<Guid>("cache-owner", ct);
		if (cacheOwner?.Value != loggedInUser)
		{
			await _cacheStorage.ClearAsync(ct);
			await _cacheStorage.SetRecordAsync("cache-owner", new CacheRecord<Guid>(loggedInUser, DateTime.UtcNow.AddYears(1)), ct);
		}
	}
}
