namespace NotificationSystem.Brokers;

public interface IDistributedCache
{
    Task<bool> ContainsAsync(string key);
    Task AddAsync(string key, TimeSpan expiration);
}