# Content Search API

**Author:** Deniz DOĞRU  
**Platform:** .NET 9.0  
**Architecture:** Clean Architecture  

Multi-provider content aggregation, scoring, and search engine service built on .NET 9.0 with Clean Architecture principles.

## Quick Start

### Prerequisites
- .NET 9.0 SDK
- SQL Server
- Visual Studio 2022 / VS Code

### Installation
```bash
# Clone repository
git clone <repository-url>
cd ContentSearchAPI

# Restore packages
dotnet restore

# Update connection string in appsettings.json

# Create migration
dotnet ef migrations add InitialCreate --project ContentSearchAPI.Infrastructure --startup-project ContentSearchAPI

# Update database
dotnet ef database update --project ContentSearchAPI

# Run application
dotnet run --project ContentSearchAPI
```

### Access Points

After running the application, you can access:

- **🎨 Dashboard (Web UI):** `https://localhost:5003/`
  - Interactive Razor Pages interface
  - Search, filter, and view content
  - Detailed scoring breakdowns
  - Responsive design

- **📚 Swagger API Documentation:** `https://localhost:5003/swagger`
  - Complete API documentation
  - Interactive testing capabilities
  - Try out all endpoints

- **🔌 API Base URL:** `https://localhost:5003/api`
  - RESTful API endpoints
  - Search: `/api/contents/search`
  - Get by ID: `/api/contents/{id}`

- **❤️ Health Check:** `https://localhost:5003/health`
  - Application health status
  - Database connectivity check

## Architecture

### Clean Architecture Layers
```
├── Domain          → Entities, Enums (zero dependencies)
├── Application     → Interfaces, DTOs
├── Infrastructure  → Repository, Services, EF Core
└── API             → Controllers, Middleware
```

### Design Patterns
- Repository Pattern
- Unit of Work
- Factory Pattern
- Template Method
- Singleton (Rate Limiter, Cache)

## Features

- ✅ Multi-provider content aggregation (JSON, XML)
- ✅ Intelligent scoring algorithm
- ✅ Search and filtering (keyword, type, sorting)
- ✅ Pagination support
- ✅ Rate limiting (sliding window)
- ✅ In-memory cache (15 min TTL)
- ✅ Background services (sync, scoring)
- ✅ Structured logging with Serilog
- ✅ Health checks
- ✅ IAuditable interface (all entities)
- ✅ Swagger/OpenAPI documentation

## API Documentation

### Search Endpoint
```http
GET /api/contents/search?keyword=net&contentType=Video&sortBy=Popularity&page=1&pageSize=10
```

**Parameters:**
- `keyword` (optional): Search term
- `contentType` (optional): `Video` or `Text`
- `sortBy` (optional): `Popularity` or `Relevance`
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 10, max: 50)

### Additional Endpoints
```http
GET /api/contents/{id}              # Get content by ID
GET /health                         # Health check
GET /mock/provider1/content         # Mock JSON provider
GET /mock/provider2/data            # Mock XML provider
```

## Scoring Algorithm

**Final Score = (Base Score × Type Multiplier) + Recency Score + Interaction Score**

### Base Score Calculation
- **Video**: `views/1000 + likes/100`
- **Text**: `readingTime + reactions/50`

### Type Multipliers
- Video: 1.5
- Text: 1.0

### Recency Bonus
- Within 1 week: +5
- Within 1 month: +3
- Within 3 months: +1
- Older: 0

### Interaction Score
- **Video**: `(likes/views) × 10`
- **Text**: `(reactions/readingTime) × 5`

## Configuration

### Provider Settings (`appsettings.json`)
```json
{
  "Providers": [
    {
      "Id": "provider1",
      "Name": "JSON Provider",
      "Endpoint": "http://localhost:5000/mock/provider1/content",
      "Format": "JSON",
      "RequestLimitPerHour": 100,
      "IsActive": true
    }
  ]
}
```

### Background Services
```json
{
  "BackgroundServices": {
    "ProviderSyncIntervalMinutes": 30,
    "ScoreCalculationIntervalMinutes": 60,
    "CacheRefreshIntervalMinutes": 20
  }
}
```

### Logging Configuration
Logs are written to: `logs/contentsearch-{Date}.log`

## Technical Decisions

### Why Clean Architecture?
- Isolates business logic from infrastructure
- Improves testability
- Easy implementation changes (e.g., in-memory → Redis)
- Domain layer has zero dependencies

### Why Factory Pattern?
- Multi-format data support (JSON, XML)
- Easy to add new providers
- Runtime provider selection based on configuration

### Why Singleton Rate Limiter?
- All requests share the same state
- Thread-safe (using SemaphoreSlim)
- Efficient memory usage
- Accesses DB through scoped IServiceProvider

### Why In-Memory Cache?
- Simple implementation
- No external dependencies (ideal for development)
- Easy migration to Redis in production (thanks to `ICacheService` interface)

### Why Serilog?
- Structured logging
- 200+ sink support
- High performance
- Native .NET integration

## Future Enhancements

### JWT Authentication
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* configuration */ });
```

### Polly (Resilience)
```csharp
builder.Services.AddHttpClient()
    .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)))
    .AddCircuitBreakerPolicy();
```

### Redis Cache
```csharp
builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = "localhost:6379"
);
```

### Docker Support
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
COPY bin/Release/net9.0/publish/ app/
ENTRYPOINT ["dotnet", "ContentSearchAPI.dll"]
```

## Development Process

This project was developed with **Claude Code (Anthropic)** AI assistant.

### AI-Assisted Development Benefits
- Boilerplate code generated 16x faster
- Clean Architecture skeleton: 4 hours → 15 minutes
- Pattern implementations with ready templates
- Auto-generated documentation

### Human-AI Collaboration
- **AI**: Structure, patterns, boilerplate
- **Human**: Domain knowledge, edge cases, refactoring

**Total development time:** ~6 hours (normally ~40 hours)

## Project Statistics

- **Lines of code:** ~3,200
- **File count:** 47
- **Design patterns:** 6
- **API endpoints:** 5
- **Database tables:** 3
- **Background services:** 3

## Project Structure

```
ContentSearchAPI/
├── Domain/
│   ├── Common/              # IAuditable, BaseEntity
│   ├── Entities/            # Content, ProviderConfig, ProviderRequestLog
│   └── Enums/               # ContentType, DataFormat, SortBy
├── Application/
│   ├── DTOs/
│   └── Interfaces/
│       ├── Repositories/    # IRepository, IUnitOfWork
│       └── Providers/       # IContentProvider, IContentProviderFactory
├── Infrastructure/
│   ├── Persistence/         # DbContext, Configurations, Seeder
│   ├── Repositories/        # Repository implementations
│   ├── Providers/           # JsonProvider, XmlProvider, Factory
│   └── Services/            # Scoring, Cache, RateLimiter
└── API/
    ├── Controllers/
    ├── Middleware/
    └── BackgroundServices/
```

---

**Built with .NET 9.0 | Clean Architecture | Serilog | EF Core**