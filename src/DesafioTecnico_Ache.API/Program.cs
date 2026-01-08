using DesafioTecnico_Ache.API.Middleware;
using DesafioTecnico_Ache.Application.Commands;
using DesafioTecnico_Ache.Application.Queries;
using DesafioTecnico_Ache.Domain.Interfaces;
using DesafioTecnico_Ache.Infrastructure.Repositories;
using DesafioTecnico_Ache.Infrastructure.SAP;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SAP SD Integration API",
        Version = "v1",
        Description = "API REST para integração com SAP S/4HANA SD (Sales & Distribution) - Módulo OData"
    });
    
    // API Key Security Definition
    options.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "X-API-Key",
        Description = "Digite a API Key: SAP-API-KEY-DEMO-2026-ACHE-DESAFIO"
    });
    
    // Apply security globally
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS Configuration (OWASP recommendation)
builder.Services.AddCors(options =>
{
    options.AddPolicy("SecurePolicy", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "https://localhost" })
              .AllowAnyHeader()
              .WithMethods("GET", "POST")
              .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

// Dependency Injection
builder.Services.AddScoped<ISapODataService, SapODataService>();
builder.Services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
builder.Services.AddScoped<IGetSalesOrderQueryHandler, GetSalesOrderQueryHandler>();
builder.Services.AddScoped<ICreateSalesOrderCommandHandler, CreateSalesOrderCommandHandler>();

// Security: Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = System.Threading.RateLimiting.PartitionedRateLimiter.Create<HttpContext, string>(context =>
        System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Request.Headers["X-API-Key"].ToString(),
            factory: _ => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});

var app = builder.Build();

// Configure the HTTP request pipeline - Always enable Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SAP SD Integration API v1");
    options.RoutePrefix = string.Empty;
});

// Security middleware (OWASP recommendations)
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseHttpsRedirection();

// CORS before authentication
app.UseCors("SecurePolicy");

// API Key authentication middleware
app.UseMiddleware<ApiKeyAuthenticationMiddleware>();

app.UseRateLimiter();
app.MapControllers();

app.Run();
