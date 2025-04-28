using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AuthService.API.Data;
using AuthService.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ReservationSystem.Shared.Clients;
using Microsoft.OpenApi.Models;
using AuthService.API.Interfaces;
using AuthService.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(8005); });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"🔍 Connection string used: {connectionString}");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("❌ ERROR: Connection string 'DefaultConnection' not found or is empty in configuration.");
}

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)),
        // Add MySQL options configuration here
        mysqlOptions =>
        {
            mysqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5, // Number of retry attempts
                maxRetryDelay: TimeSpan.FromSeconds(30), // Maximum delay between retries
                errorNumbersToAdd: null // List of specific MySQL error numbers to retry (null uses defaults)
            );
        }
    ));
builder.Services.AddIdentity<ApplicationUser, Role>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHttpClient<NetworkHttpClient>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter your JWT token in the following format: Bearer {your token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "bearer",
                Name = "Authorization",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevClient", policy =>
    {
        policy.WithOrigins("http://localhost:4201")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSecretKey = Environment.GetEnvironmentVariable("JWT__SECRET_KEY");
        var issuer = Environment.GetEnvironmentVariable("JWT__ISSUER");
        var audience = Environment.GetEnvironmentVariable("JWT__AUDIENCE");

        if (string.IsNullOrEmpty(jwtSecretKey))
            throw new ArgumentNullException(nameof(jwtSecretKey), "JWT__SECRET_KEY environment variable is not set.");

        if (string.IsNullOrEmpty(issuer))
            throw new ArgumentNullException(nameof(issuer), "JWT__ISSUER environment variable is not set.");

        if (string.IsNullOrEmpty(audience))
            throw new ArgumentNullException(nameof(audience), "JWT__AUDIENCE environment variable is not set.");

        var key = Encoding.ASCII.GetBytes(jwtSecretKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddHostedService<ExpiredTokenCleanupService>();

builder.Services.AddScoped<IUserAuthService, UserAuthService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AuthDbContext>();

    try
    {
        context.Database.Migrate();
        Console.WriteLine("✅ Database Migrations Applied Successfully.");

        // Seed roles after ensuring database schema exists
        /*
        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        var roleSeeder = new RoleSeeder(roleManager);
        await roleSeeder.SeedRolesAsync();
        */
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations: {ex.Message}");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuthService.API.Middleware.TokenBlacklistMiddleware>();
app.UseCors("AllowAngularDevClient");
app.MapControllers();

app.Run();
