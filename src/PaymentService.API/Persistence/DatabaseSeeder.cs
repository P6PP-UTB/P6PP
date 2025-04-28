using Dapper;


namespace PaymentService.API.Persistence;

public class DatabaseSeeder
{
    private readonly DapperContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(DapperContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await using var connection = await _context.CreateConnectionAsync();

        _logger.LogInformation("Seeding Payments...");

        const string insertPaymentsQuery = @"
                   INSERT INTO Payment (PaymentID, UserId, RoleId, CreditAmount, Price, Status, TransactionType, CreatedAt)
            SELECT * FROM (SELECT 1 AS PaymentID, 3 AS UserId, 1 AS RoleId, 250 AS CreditAmount, 0 AS Price, 'completed' AS Status, 'credit' AS TransactionType, NOW() AS CreatedAt) AS tmp
            WHERE NOT EXISTS (
                SELECT 1 FROM Payment WHERE PaymentID = 1
            )
            UNION ALL
            SELECT * FROM (SELECT 2 AS PaymentID, 2 AS UserId, 3 AS RoleId, 0 AS CreditAmount, 200 AS Price, 'completed' AS Status, 'reservation' AS TransactionType, NOW() AS CreatedAt) AS tmp
            WHERE NOT EXISTS (
                SELECT 1 FROM Payment WHERE PaymentID = 2
            )
            UNION ALL
            SELECT * FROM (SELECT 3 AS PaymentID, 3 AS UserId, 1 AS RoleId, 0 AS CreditAmount, 500 AS Price, 'failed' AS Status, 'reservation' AS TransactionType, NOW() AS CreatedAt) AS tmp
            WHERE NOT EXISTS (
                SELECT 1 FROM Payment WHERE PaymentID = 3
            );
        ";

        await connection.ExecuteAsync(insertPaymentsQuery);

        _logger.LogInformation("Payments seeded successfully.");

        _logger.LogInformation("Seeding UserCredit...");

        const string insertUserCreditQuery = @"
               INSERT INTO UserCredit (UserId, RoleId, CreditBalance)
        SELECT * FROM (SELECT 1 AS UserId, 1 AS RoleId, 1000 AS CreditBalance) AS tmp
        WHERE NOT EXISTS (
            SELECT 1 FROM UserCredit WHERE UserId = 1 AND RoleId = 1
        )
        UNION ALL
        SELECT * FROM (SELECT 2 AS UserId, 2 AS RoleId, 500 AS CreditBalance) AS tmp
        WHERE NOT EXISTS (
            SELECT 1 FROM UserCredit WHERE UserId = 2 AND RoleId = 2
        )
        UNION ALL
        SELECT * FROM (SELECT 3 AS UserId, 1 AS RoleId, 250 AS CreditBalance) AS tmp
        WHERE NOT EXISTS (
            SELECT 1 FROM UserCredit WHERE UserId = 3 AND RoleId = 1
        );
        ";
        await connection.ExecuteAsync(insertUserCreditQuery);
        _logger.LogInformation("UserCredit seeded successfully.");
        _logger.LogInformation("Seeding completed.");


    }
}