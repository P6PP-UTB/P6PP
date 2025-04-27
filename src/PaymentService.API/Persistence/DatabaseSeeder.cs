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
           INSERT INTO Payment (PaymentID, UserId, RoleId, CreditAmount, Price, Status, TransactionType, CreatedAt) VALUES
           (1, 3, 1, 250, 0, 'completed', 'credit', NOW()),
           (2, 2, 3, 0, 200, 'completed', 'reservation', NOW()),
           (3, 3, 1, 0, 500, 'failed', 'reservation', NOW());
        ";

        await connection.ExecuteAsync(insertPaymentsQuery);

        _logger.LogInformation("Payments seeded successfully.");

        _logger.LogInformation("Seeding UserCredit...");

        const string insertUserCreditQuery = @"
           INSERT INTO UserCredit (UserId, RoleId, CreditBalance) VALUES
           (1, 1, 1000),
           (2, 2, 500),
           (3, 1, 250);";
        await connection.ExecuteAsync(insertUserCreditQuery);
        _logger.LogInformation("UserCredit seeded successfully.");
        _logger.LogInformation("Seeding completed.");


    }
}