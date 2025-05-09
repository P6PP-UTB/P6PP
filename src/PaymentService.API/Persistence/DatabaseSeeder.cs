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
       

        


    }
}