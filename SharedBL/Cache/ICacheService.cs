namespace SharedBL.Cache;

public interface ICacheService
{
    void Set(string key, string value, TimeSpan expiry);
    string Get(string key);
    void GetOrSet(string key, Func<string> func, TimeSpan expiry);
}