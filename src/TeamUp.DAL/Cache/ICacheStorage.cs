namespace TeamUp.DAL.Cache;

public interface ICacheStorage
{
	public ValueTask<CacheRecord<T>?> GetRecordAsync<T>(string key, CancellationToken ct);
	public ValueTask SetRecordAsync<T>(string key, CacheRecord<T> record, CancellationToken ct);
	public ValueTask RemoveAsync(string key, CancellationToken ct);
	public ValueTask ClearAsync(CancellationToken ct);
}
