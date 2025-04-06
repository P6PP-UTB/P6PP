using UserService.API.Extensions;
using UserService.API.Features;
using UserService.API.Features.Roles;
using UserService.API.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5189);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterServices(builder.Configuration);

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


app.UseRouting();

app.UseEndpoints(endpoints =>
{
    // USER ENDPOINTS
    GetUserByIdEndpoint.Register(endpoints);
    GetUsersEndpoint.Register(endpoints);
    DeleteUserEndpoint.Register(endpoints);
    UpdateUserEndpoint.Register(endpoints);
    CreateUserEndpoint.Register(endpoints);
    AssignUserRoleEndpoint.Register(endpoints);


    // ROLE ENDPOINTS
    GetRoleByIdEndpoint.Register(endpoints);
    GetRolesEndpoint.Register(endpoints);
    CreateRoleEndpoint.Register(endpoints);

});

app.Run();

