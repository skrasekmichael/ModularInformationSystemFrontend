using RailwayResult;

namespace TeamUp.DAL.Cache;

public sealed class CacheFacade
{
	private readonly ICacheStorage _cacheStorage;

	public CacheFacade(ICacheStorage cacheStorage)
	{
		_cacheStorage = cacheStorage;
	}

	public ValueTask ClearAsync(CancellationToken ct) => _cacheStorage.ClearAsync(ct);

	public ValueTask DeleteAsync(string key, CancellationToken ct) => _cacheStorage.RemoveAsync(key, ct);

	public async ValueTask UpdateAsync<T>(string key, Action<T> update, CancellationToken ct) where T : class
	{
		var record = await _cacheStorage.GetRecordAsync<T>(key, ct);
		if (record is null)
		{
			return;
		}

		update(record.Value);
		await _cacheStorage.SetRecordAsync(key, record, ct);
	}

	public async Task<Result<T>> GetAsync<T>(string key, Func<Task<Result<T>>> fetchAsync, TimeSpan lifetime, bool forceFetch, CancellationToken ct)
	{
		CacheRecord<T>? record = null;

		if (!forceFetch)
		{
			record = await _cacheStorage.GetRecordAsync<T>(key, ct);
		}

		if (record is not null)
		{
			return record.Value;
		}

		var validValue = await fetchAsync();
		if (validValue.IsSuccess)
		{
			await _cacheStorage.SetRecordAsync(key, new CacheRecord<T>(validValue.Value, DateTime.UtcNow.Add(lifetime)), ct);
		}

		return validValue;
	}
}
