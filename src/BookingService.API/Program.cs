using BookingService.API;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext(builder.Configuration, builder.Environment);
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuth();
builder.Services.AddMediatR();
builder.Services.AddSwagger();
builder.Services.AddControllers()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .AddEndpointValidation();

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

// Ng serve Angular
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


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.ApplyMigrations();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowAngularDevClient");
app.UseCors("AllowAngularNgClient");

app.MapControllers();

app.Run();
