using ReservationSystem.Shared.Cors;
using ReservationSystem.Shared.Middlewares;
using UserService.API.Abstraction;
using UserService.API.Extensions;
using UserService.API.Features;
using UserService.API.Features.Roles;
using UserService.API.Persistence;
using UserService.API.Persistence.Repositories;
using UserService.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5189);
});

var corsSettingsSection = builder.Configuration.GetSection("Cors");
builder.Services.Configure<CorsSettings>(corsSettingsSection);
var corsSettings = corsSettingsSection.Get<CorsSettings>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevClient", policy =>
    {
        policy.WithOrigins(corsSettings.AllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterServices(builder.Configuration);
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService.API.Services.UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.Configure<AuthMiddlewareOptions>(options =>
{
    options.ExcludedRoutes = new List<ExcludedRoute>
    {
        new() { Path = "/api/user", Method = "POST", MatchType = PathMatchType.Exact },
        new() { Path = "/api/user", Method = "GET", MatchType = PathMatchType.StartsWith },
        new() { Path = "/api/users", Method = "GET", MatchType = PathMatchType.Exact }
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var databaseInitializer = services.GetRequiredService<DatabaseInitializer>();
    await databaseInitializer.InitializeDatabaseAsync();

    // Seed the database
    var dbSeeder = services.GetRequiredService<DatabaseSeeder>();
    await dbSeeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseCors("AllowAngularDevClient");
app.UseCors("AllowAngularNgClient");

app.UseRouting();

app.UseMiddleware<AuthMiddleware>();

app.UseEndpoints(endpoints =>
{
    // USER ENDPOINTS
    GetUserByIdEndpoint.Register(endpoints);
    GetUsersEndpoint.Register(endpoints);
    DeleteUserEndpoint.Register(endpoints);
    UpdateUserEndpoint.Register(endpoints);
    CreateUserEndpoint.Register(endpoints);
    AssignUserRoleEndpoint.Register(endpoints);
    DeactivateUserEndpoint.Register(endpoints);
    ActivateUserEndpoint.Register(endpoints);

    // ROLE ENDPOINTS
    GetRoleByIdEndpoint.Register(endpoints);
    GetRolesEndpoint.Register(endpoints);
    CreateRoleEndpoint.Register(endpoints);

});

app.Run();

