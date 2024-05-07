using Blazored.LocalStorage;

namespace TeamUp.DAL.Cache;

internal sealed class ClientCacheStorage : ICacheStorage
{
	private readonly ILocalStorageService _localStorage;

	public ClientCacheStorage(ILocalStorageService localStorage)
	{
		_localStorage = localStorage;
	}

	public async ValueTask<CacheRecord<T>?> GetRecordAsync<T>(string key, CancellationToken ct)
	{
		var record = await _localStorage.GetItemAsync<CacheRecord<T>>(key, ct);
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

	public ValueTask SetRecordAsync<T>(string key, CacheRecord<T> record, CancellationToken ct) => _localStorage.SetItemAsync(key, record, ct);

	public ValueTask RemoveAsync(string key, CancellationToken ct) => _localStorage.RemoveItemAsync(key, ct);

	public ValueTask ClearAsync(CancellationToken ct) => _localStorage.ClearAsync(ct);
}
