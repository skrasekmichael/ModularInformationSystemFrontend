using CommunityToolkit.Mvvm.Messaging;

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
	private readonly IAuthService _authService;

	public LoginService(ApiClient client, IMessenger messenger, ICacheStorage cacheStorage, IAuthService authService)
	{
		_client = client;
		_messenger = messenger;
		_cacheStorage = cacheStorage;
		_authService = authService;
	}

	public Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken ct)
	{
		return _client.LoginAsync(request, ct);
	}

	public async Task ValidateCacheAsync(CancellationToken ct)
	{
		var loggedInUser = await _authService.GetUserIdAsync();

		var cacheOwner = await _cacheStorage.GetRecordAsync<Guid>("cache-owner", ct);
		if (cacheOwner?.Value != loggedInUser.Value)
		{
			await _cacheStorage.ClearAsync(ct);
			await _cacheStorage.SetRecordAsync("cache-owner", new CacheRecord<Guid>(loggedInUser.Value, DateTime.UtcNow.AddYears(1)), ct);
		}
	}
}
