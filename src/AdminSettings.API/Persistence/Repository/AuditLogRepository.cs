using AdminSettings.Persistence.Entities;
using Dapper;
using System.Data;

namespace AdminSettings.Persistence.Repositories;

public class AuditLogRepository
{
    private readonly DapperContext _context;

    public AuditLogRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AuditLog>> GetAllAsync()
    {
        using var connection = await _context.CreateConnectionAsync();
        const string query = "SELECT * FROM AuditLogs;";
        return await connection.QueryAsync<AuditLog>(query);
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId)
    {
        using var connection = await _context.CreateConnectionAsync();
        const string query = "SELECT * FROM AuditLogs WHERE UserId = @UserId;";
        return await connection.QueryAsync<AuditLog>(query, new { UserId = userId });
    }

    public async Task<IEnumerable<AuditLog>> GetByActionAsync(string action)
    {
        using var connection = await _context.CreateConnectionAsync();
        const string query = "SELECT * FROM AuditLogs WHERE Action = @Action;";
        return await connection.QueryAsync<AuditLog>(query, new { Action = action });
    }

    public async Task<AuditLog?> GetByIdAsync(int id)
    {
        using var connection = await _context.CreateConnectionAsync();
        const string query = "SELECT * FROM AuditLogs WHERE Id = @Id;";
        return await connection.QuerySingleOrDefaultAsync<AuditLog>(query, new { Id = id });
    }

    public async Task<int> AddAsync(AuditLog log)
    {
        using var connection = await _context.CreateConnectionAsync();
        const string query = @"
            INSERT INTO AuditLogs (UserId, TimeStamp, Action)
            VALUES (@UserId, @TimeStamp, @Action);
            SELECT LAST_INSERT_ID();";
        return await connection.ExecuteScalarAsync<int>(query, log);
    }

    public async Task ArchiveAsync(int id)
    {
        using var connection = await _context.CreateConnectionAsync();
        const string query = "UPDATE AuditLogs SET Action = 'Archived' WHERE Id = @Id;";
        await connection.ExecuteAsync(query, new { Id = id });
    }
}
