# Content Search API - by Deniz DOĞRU

Task: A search engine service built with .NET 9.0 that aggregates content from multiple providers using Clean Architecture principles.

##  Architecture

This project follows **Clean Architecture** with clear separation of concerns:

```
ContentSearchAPI/
├── Domain/          # Business entities & rules (no dependencies)
├── Application/     # Interfaces & DTOs (depends on Domain)
├── Infrastructure/  # Data access & external services
└── API/            # Controllers & presentation layer
```

### Key Design Patterns

- **Repository Pattern**: Generic data access abstraction
- **Unit of Work**: Transaction management
- **Factory Pattern**: Dynamic provider creation (JSON/XML)
- **Template Method**: Base provider with specialized implementations
- **Singleton**: Rate limiter and cache services

##  Features

### Implemented
- ✅ Content search with filters (keyword, type, sorting)
- ✅ Pagination support
- ✅ Multi-provider aggregation (JSON & XML)
- ✅ Scoring algorithm (base + recency + interaction)
- ✅ Rate limiting (sliding window)
- ✅ In-memory caching (15-minute TTL)
- ✅ Background services (sync, scoring, cache refresh)
- ✅ Health checks
- ✅ Structured logging (Serilog)
- ✅ Swagger/OpenAPI documentation
- ✅ IAuditable interface on all entities

### Future Enhancements

**JWT Authentication**
If implemented, would use `Microsoft.AspNetCore.Authentication.JwtBearer` with token-based security on endpoints.

**Polly Resilience**
Could add retry policies and circuit breakers for HTTP calls to external providers using Polly library.

**Redis Caching**
Current in-memory cache can be replaced with `StackExchange.Redis` for distributed caching.

**Docker Support**
A `Dockerfile` and `docker-compose.yml` would containerize the API with SQL Server and Redis dependencies.

##  Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
```bash
git clone <repository-url>
cd ContentSearchAPI
```

2. **Restore packages**
```bash
dotnet restore
```

3. **Update database connection**

Edit `appsettings.json` connection string if needed:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=SearchEngineDB;Trusted_Connection=true;TrustServerCertificate=true"
}
```

4. **Create database migration**
```bash
dotnet ef migrations add InitialCreate --project ContentSearchAPI.Infrastructure --startup-project ContentSearchAPI
```

5. **Apply migration**
```bash
dotnet ef database update --project ContentSearchAPI
```

6. **Run the application**
```bash
dotnet run --project ContentSearchAPI
```

7. **Access Swagger UI**

Navigate to: `https://localhost:5001/swagger`

##  API Endpoints

### Search Contents
```http
GET /api/contents/search?keyword=test&contentType=Video&sortBy=Popularity&page=1&pageSize=10
```

**Query Parameters:**
- `keyword` (optional): Search term
- `contentType` (optional): `Video` or `Text`
- `sortBy` (optional): `Popularity` or `Relevance` (default)
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 10, max: 50)

### Get Content by ID
```http
GET /api/contents/{id}
```

### Health Check
```http
GET /health
```

### Mock Providers (for testing)
```http
GET /mock/provider1/content  # JSON format
GET /mock/provider2/data     # XML format
```

##  Scoring Algorithm

The scoring system combines multiple factors:

**Final Score = (Base Score × Content Type Multiplier) + Recency Score + Interaction Score**

### Base Score
- **Video**: `views/1000 + likes/100`
- **Text**: `readingTime + reactions/50`

### Content Type Multiplier
- **Video**: 1.5
- **Text**: 1.0

### Recency Score
- Within 1 week: +5
- Within 1 month: +3
- Within 3 months: +1
- Older: 0

### Interaction Score
- **Video**: `(likes/views) × 10`
- **Text**: `(reactions/readingTime) × 5`

##  Configuration

### Provider Settings

Providers are configured in `appsettings.json`:

```json
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
```

### Background Services

```json
"BackgroundServices": {
  "ProviderSyncIntervalMinutes": 30,
  "ScoreCalculationIntervalMinutes": 60,
  "CacheRefreshIntervalMinutes": 20
}
```

### Logging

Serilog is configured to write to both console and file:

```json
"Serilog": {
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft": "Warning"
    }
  }
}
```

Logs are saved to: `logs/contentsearch-{Date}.log`

##  Testing

### Mock Providers

The application includes built-in mock providers for testing:

- **JSON Provider**: Returns sample video and text content
- **XML Provider**: Returns sample content in XML format

Both can be accessed directly for testing provider integration.

### Rate Limiting

Each provider has a configurable hourly request limit. The sliding window algorithm prevents exceeding these limits.

##  Project Structure

```
ContentSearchAPI/
├── ContentSearchAPI.Domain/
│   ├── Common/
│   │   ├── IAuditable.cs
│   │   └── BaseEntity.cs
│   ├── Entities/
│   │   ├── Content.cs
│   │   ├── ProviderConfig.cs
│   │   └── ProviderRequestLog.cs
│   └── Enums/
│       ├── ContentType.cs
│       ├── DataFormat.cs
│       └── SortBy.cs
│
├── ContentSearchAPI.Application/
│   ├── DTOs/
│   ├── Interfaces/
│   │   ├── Repositories/
│   │   └── Providers/
│   └── ...
│
├── ContentSearchAPI.Infrastructure/
│   ├── Persistence/
│   ├── Repositories/
│   ├── Providers/
│   └── Services/
│
└── ContentSearchAPI/
    ├── Controllers/
    ├── Middleware/
    ├── BackgroundServices/
    └── Program.cs
```

##  Architecture Decisions

### Why Clean Architecture?

- **Testability**: Business logic isolated from infrastructure
- **Maintainability**: Clear separation of concerns
- **Flexibility**: Easy to swap implementations (e.g., in-memory to Redis cache)
- **Independence**: Domain layer has zero dependencies

### Why Factory Pattern for Providers?

- Supports multiple data formats (JSON, XML)
- Easy to add new provider types
- Centralizes provider instantiation logic
- Runtime provider selection based on configuration

### Why Singleton for Rate Limiter?

- Maintains shared state across all requests
- Prevents race conditions with SemaphoreSlim
- Efficient memory usage
- Uses IServiceProvider to create scopes for database access

### Why In-Memory Cache?

- Simple to implement
- No external dependencies for development
- Can be easily replaced with Redis in production
- Sufficient for single-instance deployments


---

**Built with .NET 9.0** | **Clean Architecture** | **Serilog** | **EF Core** | **Swagger**
