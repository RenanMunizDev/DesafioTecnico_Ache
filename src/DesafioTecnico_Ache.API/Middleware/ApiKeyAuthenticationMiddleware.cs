namespace DesafioTecnico_Ache.API.Middleware;

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;
    private const string ApiKeyHeaderName = "X-API-Key";
    
    public ApiKeyAuthenticationMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // Allow Swagger and OpenAPI endpoints
        if (IsPublicEndpoint(context.Request.Path))
        {
            await _next(context);
            return;
        }
        
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            _logger.LogWarning("API Key missing for request {Path}", context.Request.Path);
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = "API Key is missing" });
            return;
        }
        
        var apiKey = _configuration.GetValue<string>("ApiKey");
        
        if (string.IsNullOrEmpty(apiKey) || !apiKey.Equals(extractedApiKey))
        {
            _logger.LogWarning("Invalid API Key provided for request {Path}", context.Request.Path);
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = "Invalid API Key" });
            return;
        }
        
        await _next(context);
    }
    
    private static bool IsPublicEndpoint(PathString path)
    {
        return path.StartsWithSegments("/swagger") ||
               path.StartsWithSegments("/swagger/index.html") ||
               path.StartsWithSegments("/swagger/v1/swagger.json") ||
               path.Value?.Contains("swagger", StringComparison.OrdinalIgnoreCase) == true ||
               path.Value == "/";
    }
}
