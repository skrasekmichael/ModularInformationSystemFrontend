using Blazored.LocalStorage;

namespace TeamUp.DAL.Cache;

internal sealed class ServerCacheStorage : ICacheStorage
{
	private readonly ILocalStorageService _localStorage;

	public ServerCacheStorage(ILocalStorageService localStorage)
	{
		_localStorage = localStorage;
	}

	public async ValueTask<CacheRecord<T>?> GetRecordAsync<T>(string key, CancellationToken ct)
	{
		CacheRecord<T>? record;

		try
		{
			record = await _localStorage.GetItemAsync<CacheRecord<T>>(key, ct);
		}
		catch
		{
			return null;
		}


		if (record is null)
		{
			return null;
		}

		if (record.ValidUntilUtc < DateTime.UtcNow)
		{
			await _localStorage.RemoveItemAsync(key, ct);
			return null;
		}

		return record;
	}

	public async ValueTask SetRecordAsync<T>(string key, CacheRecord<T> record, CancellationToken ct)
	{
		try
		{
			await _localStorage.SetItemAsync(key, record, ct);
		}
		catch
		{
		}
	}

	public async ValueTask RemoveAsync(string key, CancellationToken ct)
	{
		try
		{
			await _localStorage.RemoveItemAsync(key, ct);
		}
		catch
		{
		}
	}

	public async ValueTask ClearAsync(CancellationToken ct)
	{
		try
		{
			await _localStorage.ClearAsync(ct);
		}
		catch
		{
		}
	}
}
