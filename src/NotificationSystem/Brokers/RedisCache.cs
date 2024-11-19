using NotificationSystem.Brokers;
using StackExchange.Redis;

public class RedisCache : IDistributedCache
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisCache> _logger;

    public RedisCache(ILogger<RedisCache> logger)
    {
        _logger = logger;
        var connection = ConnectionMultiplexer.Connect("redis:6379");
        _database = connection.GetDatabase();
        _logger.LogInformation("Connected to Redis.");
    }

    public async Task<bool> ContainsAsync(string key)
    {
        var exists = await _database.KeyExistsAsync(key);
        _logger.LogInformation($"Checking if key exists in Redis: {key} - Exists: {exists}");
        return exists;
    }

    public async Task AddAsync(string key, TimeSpan expiration)
    {
        await _database.StringSetAsync(key, "true", expiration);
        _logger.LogInformation($"Added key to Redis: {key} with expiration: {expiration}");
    }
}