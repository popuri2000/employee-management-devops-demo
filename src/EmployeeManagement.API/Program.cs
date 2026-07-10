using System.Threading.RateLimiting;
using Asp.Versioning;
using EmployeeManagement.API.Middleware;
using EmployeeManagement.Application;
using EmployeeManagement.Infrastructure;
using EmployeeManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/api-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14));

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Employee Management API",
        Version = "v1",
        Description = "A production-ready REST API for managing employees, built with ASP.NET Core 9, EF Core, and Clean Architecture."
    });
});

const string CorsPolicyName = "AllowWebClient";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "https://localhost:7270", "http://localhost:5217" };

        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

const string RateLimiterPolicyName = "ApiFixedWindow";
builder.Services.AddRateLimiter(limiterOptions =>
{
    limiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    limiterOptions.AddFixedWindowLimiter(RateLimiterPolicyName, windowOptions =>
    {
        windowOptions.PermitLimit = 100;
        windowOptions.Window = TimeSpan.FromMinutes(1);
        windowOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        windowOptions.QueueLimit = 10;
    });
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddHealthChecks();

var app = builder.Build();

// Apply pending EF Core migrations automatically on startup (demo convenience).
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging();

app.UseGlobalExceptionMiddleware();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management API v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseCors(CorsPolicyName);

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers().RequireRateLimiting(RateLimiterPolicyName);
app.MapHealthChecks("/health");

app.Run();

public partial class Program
{
}
