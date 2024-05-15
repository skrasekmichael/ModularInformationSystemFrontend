using RailwayResult;

using TeamUp.Contracts.Users;
using TeamUp.DAL.Api;
using TeamUp.DAL.Cache;

namespace TeamUp.DAL.Services;

public sealed class AccountService
{
	private readonly ApiClient _client;
	private readonly ICacheStorage _cacheStorage;
	private readonly IAuthService _authService;

	public AccountService(ApiClient client, ICacheStorage cacheStorage, IAuthService authService)
	{
		_client = client;
		_cacheStorage = cacheStorage;
		_authService = authService;
	}

	public Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken ct)
	{
		return _client.LoginAsync(request, ct);
	}

	public async Task ClearCacheAsync(CancellationToken ct)
	{
		var cacheOwner = await _cacheStorage.GetRecordAsync<Guid>("cache-owner", ct);
		await _cacheStorage.ClearAsync(ct);

		if (cacheOwner is not null)
		{
			await _cacheStorage.SetRecordAsync("cache-owner", new CacheRecord<Guid>(cacheOwner.Value, DateTime.UtcNow.AddYears(1)), ct);
		}
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
