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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Content Search API V1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
