namespace WebSocketService.Cache
{
    public interface ICacheService
    {
        Task<T> GetCacheAsync<T>(Guid key) where T : class;
        Task SetCacheAsync<T>(Guid key, T value) where T : class;
    }
}
