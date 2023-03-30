using StackExchange.Redis;
using System.Text.Json;

namespace MvcLab3.Services
{
  public class RedisCacheService : ICacheService
  {
    private readonly IConnectionMultiplexer _redisCon;
    private readonly IDatabase _cache;

    // 1 günlüğüne cache de tutar
    private TimeSpan ExpireTime => TimeSpan.FromDays(1);

    public RedisCacheService(IConnectionMultiplexer redisCon)
    {
      _redisCon = redisCon;
      _cache = redisCon.GetDatabase();
    }

    // key göre cache
    public async Task Clear(string key)
    {
      await _cache.KeyDeleteAsync(key);
    }

    // bütün cache temizle
    public void ClearAll()
    {
      var endpoints = _redisCon.GetEndPoints(true);
      foreach (var endpoint in endpoints)
      {
        var server = _redisCon.GetServer(endpoint);
        server.FlushAllDatabases();
      }
    }

    // obje olarak cache null ise ekler 
    public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> action) where T : class
    {
      var result = await _cache.StringGetAsync(key);
      if (result.IsNull)
      {
        result = JsonSerializer.SerializeToUtf8Bytes(await action());
        await SetValueAsync(key, result);
      }
      return JsonSerializer.Deserialize<T>(result);
    }

    //key bazlı sonucu getirir
    public async Task<string> GetValueAsync(string key)
    {
      return await _cache.StringGetAsync(key);
    }

    // şu keye lu value set etmek için kullandık
    public async Task<bool> SetValueAsync(string key, string value)
    {
      return await _cache.StringSetAsync(key, value, ExpireTime);
    }

    // cache yoksa oluştur varsa getir
    public T GetOrAdd<T>(string key, Func<T> action) where T : class
    {
      var result = _cache.StringGet(key);
      if (result.IsNull)
      {
        result = JsonSerializer.SerializeToUtf8Bytes(action());
        _cache.StringSet(key, result, ExpireTime);
      }
      return JsonSerializer.Deserialize<T>(result);
    }
  }
}
