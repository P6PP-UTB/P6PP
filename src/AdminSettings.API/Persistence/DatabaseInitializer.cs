using Dapper;
using MySqlConnector;

namespace AdminSettings.Persistence;

public class DatabaseInitializer
{
    private readonly string _connectionString;
    private readonly string _adminConnectionString;
    private readonly string _databaseName;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IConfiguration configuration, ILogger<DatabaseInitializer> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                              ?? throw new Exception("Connection string not found.");

        _logger = logger;

        var builder = new MySqlConnectionStringBuilder(_connectionString);
        _databaseName = builder.Database;
        builder.Database = ""; // Přepnutí na root úroveň
        _adminConnectionString = builder.ToString();
    }

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            _logger.LogInformation("Checking for existence of database '{Database}'...", _databaseName);

            await using var adminConn = new MySqlConnection(_adminConnectionString);
            await adminConn.OpenAsync();
            _logger.LogInformation("Tady");
            var dbExists = await adminConn.ExecuteScalarAsync<string>(
                "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @DatabaseName",
                new { DatabaseName = _databaseName });

            if (dbExists == null)
            {
                _logger.LogInformation("Creating database '{Database}'...", _databaseName);
                await adminConn.ExecuteAsync($"CREATE DATABASE `{_databaseName}`;");
                _logger.LogInformation("Database '{Database}' created.");
            }

            await using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            const string tableSql = @"
                CREATE TABLE IF NOT EXISTS AuditLogs (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    UserId VARCHAR(255) NOT NULL,
                    TimeStamp DATETIME NOT NULL,
                    Action VARCHAR(255) NOT NULL
                );";

            await conn.ExecuteAsync(tableSql);
            _logger.LogInformation("Table 'AuditLogs' ensured.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database initialization failed.");
            throw;
        }
    }
}
