using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ReservationSystem.Shared.Middlewares;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthMiddlewareOptions _options;

    public AuthMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory, IOptions<AuthMiddlewareOptions> options)
    {
        _next = next;
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();
        var method = context.Request.Method;

        // Skip middleware for configured routes
        if (_options.ExcludedRoutes.Any(x =>
                path != null &&
                (x.MatchType == PathMatchType.StartsWith && path.StartsWith(x.Path.ToLower()) ||
                 x.MatchType == PathMatchType.Exact && path.Equals(x.Path.ToLower())) &&
                (x.Method == null || x.Method.Equals(method, StringComparison.OrdinalIgnoreCase))))
        {
            Console.WriteLine($"[Middleware] Skipping auth for: {path} ({method})");
            await _next(context);
            return;
        }

        // Token logic...
        var token = context.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing or empty token.");
            return;
        }

        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, ServiceEndpoints.AuthService.ValidateToken);
        request.Headers.Add("Authorization", token);

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var errorResponse = JsonSerializer.Serialize(new
            {
                success = false,
                message = "Token validation failed: " + errorMessage
            });

            await context.Response.WriteAsync(errorResponse);
            return;
        }

        await _next(context);
    }
}