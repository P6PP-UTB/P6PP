using AdminSettings.Persistence.Entities;
using AdminSettings.Persistence.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace AdminSettings.Services;

public class AuditLogService
{
    private readonly AuditLogRepository _repository;
    private readonly IMemoryCache _memoryCache;
    private const string CacheKey = "auditlogs";

    public AuditLogService(AuditLogRepository repository, IMemoryCache memoryCache)
    {
        _repository = repository;
        _memoryCache = memoryCache;
    }

    public async Task<IEnumerable<AuditLog>> GetAllAsync()
    {
        if (!_memoryCache.TryGetValue(CacheKey, out IEnumerable<AuditLog>? logs))
        {
            logs = await _repository.GetAllAsync();

            _memoryCache.Set(CacheKey, logs, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
        }

        return logs!;
    }

    public async Task<int> CreateAsync(AuditLog log)
    {
        log.TimeStamp = DateTime.UtcNow;

        var id = await _repository.AddAsync(log);

        _memoryCache.Remove(CacheKey);
        _memoryCache.Set("auditlog:" + id, log, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });

        return id;
    }

    public async Task<IEnumerable<AuditLog>> GetByUserAsync(string userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<AuditLog>> GetByActionAsync(string action)
    {
        return await _repository.GetByActionAsync(action);
    }

    public async Task ArchiveAsync(int id)
    {
        await _repository.ArchiveAsync(id);
        _memoryCache.Remove(CacheKey);
    }
}