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


builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

builder.Services.AddIdentity<ApplicationUser, Role>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularNgClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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


builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        //var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        var jwtSecretKey =
            "e1ec5a67c31e5e3d36b59c7f00478432c998c5f524bf3c9ad75e221c48a729d0fea2704fdcc8571390945d05843cec8e7a5303b766a06e92c88f34330890d9ee82db2d2e48b61d3d645aa270cf45fdf2fd22080fd1d3b7603bc0a3d4b77f6eb3bac50d5abe4897a093fa821b5561cdf65fd1f872b0165f283390d8ad0201bc02e69b569b3a2ede792e3310e3d6d967d87a0e00954f01cc1391e3466d03144489a12bbc119f73acef92fb5da06880522b9582a3a08c797aeab1a008e2c1d6a423768966028f7c40c0d07faf7f9b3c57e5abc28582f87de2b7760219a7380d8992669f7c6be0a4ab1eb26018fa9653a9c198d3abec9fdbebbfe18559933e9fbdf5";
        //var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
        var issuer = "Local";
        //var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
        var audience = "local";


        if (string.IsNullOrEmpty(jwtSecretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            throw new ArgumentNullException("JWT configuration is missing.");
        }

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
app.UseCors("AllowAngularDevClient");
app.UseCors("AllowAngularNgClient");

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuthService.API.Middleware.TokenBlacklistMiddleware>();

app.MapControllers();

app.Run();