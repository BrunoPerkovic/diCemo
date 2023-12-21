using StackExchange.Redis;

namespace SharedBL.Cache;

public class CacheService : ICacheService
{
    private readonly Lazy<ConnectionMultiplexer> _redisConnection;

    public CacheService(string connectionString)
    {
        _redisConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(connectionString);
        });
    }
    
    public void Set(string key, string value, TimeSpan expiry)
    {
        var db = _redisConnection.Value.GetDatabase();
        db.StringSet(key, value, expiry);
    }

    public string Get(string key)
    {
        var db = _redisConnection.Value.GetDatabase();
        return db.StringGet(key);
    }
}