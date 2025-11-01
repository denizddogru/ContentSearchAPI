using ContentSearchAPI.Application.Interfaces;
using ContentSearchAPI.Application.Interfaces.Providers;
using ContentSearchAPI.Application.Interfaces.Repositories;
using ContentSearchAPI.BackgroundServices;
using ContentSearchAPI.Infrastructure.Persistence;
using ContentSearchAPI.Infrastructure.Providers;
using ContentSearchAPI.Infrastructure.Repositories;
using ContentSearchAPI.Infrastructure.Services;
using ContentSearchAPI.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/contentsearch-.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Enum'ları string olarak serialize et (Swagger için)
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Content Search API",
        Version = "v1",
        Description = "A search engine API that aggregates content from multiple providers"
    });
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IContentRepository, ContentRepository>();
builder.Services.AddScoped<IProviderConfigRepository, ProviderConfigRepository>();

// Services
builder.Services.AddScoped<IContentService, ContentService>();
builder.Services.AddScoped<IScoringService, ScoringService>();
builder.Services.AddScoped<IProviderService, ProviderService>();
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<IRateLimiter, RateLimiter>();

// Providers
builder.Services.AddScoped<IContentProviderFactory, ContentProviderFactory>();
builder.Services.AddHttpClient();

// Background Services
builder.Services.AddHostedService<ProviderSyncService>();
builder.Services.AddHostedService<ScoreCalculationService>();
builder.Services.AddHostedService<CacheRefreshService>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseMiddleware<GlobalExceptionHandler>();

// Swagger her zaman açık
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Content Search API V1");
    c.RoutePrefix = string.Empty; // Swagger root path'te açılsın (/)
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Add Serilog request logging
app.UseSerilogRequestLogging();

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

try
{
    Log.Information("Starting Content Search API");

    // Seed database with sample data
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();

        try
        {
            var seeder = new DatabaseSeeder(context, logger);
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while seeding the database");
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
