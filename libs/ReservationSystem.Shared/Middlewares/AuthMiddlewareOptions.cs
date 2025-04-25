namespace ReservationSystem.Shared.Middlewares;

public class AuthMiddlewareOptions
{
    public List<ExcludedRoute> ExcludedRoutes { get; set; } = new();
}

public enum PathMatchType
{
    Exact,
    StartsWith
}

public class ExcludedRoute
{
    public string Path { get; set; } = default!;
    public string? Method { get; set; }
    public PathMatchType MatchType { get; set; } = PathMatchType.Exact;
}