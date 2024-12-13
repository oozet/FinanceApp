using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

public class CacheService
{
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, bool> _keys;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
        _keys = new ConcurrentDictionary<string, bool>();
    }

    public void Set<T>(string key, T value, MemoryCacheEntryOptions options = null)
    {
        if (options == null)
        {
            _cache.Set(key, value);
        }
        else
        {
            _cache.Set(key, value, options);
        }
        _keys[key] = true;
    }

    public bool TryGetValue<T>(string key, out T value)
    {
        return _cache.TryGetValue(key, out value);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        _keys.TryRemove(key, out _);
    }

    public IEnumerable<string> GetKeys()
    {
        return _keys.Keys;
    }
}
