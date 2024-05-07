namespace TeamUp.DAL.Cache;

public sealed record CacheRecord<TValue>(TValue Value, DateTime ValidUntilUtc);
