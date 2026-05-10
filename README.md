# FunPokedexApi

A REST API that retrieves Pokémon information from [PokéAPI](https://pokeapi.co/) and optionally 
translates descriptions using the FunTranslations API.

> **Note:** The project requirements reference both the official FunTranslations API
> (`https://funtranslations.com/api`) and a self-hosted instance
> (`https://api.funtranslations.mercxry.me/v1`). Currently the project uses the self-hosted
> instance, as explicitly indicated in the functional requirements of the endpoint.
> The base URL is configurable via `appsettings.json`.


## Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/pokemon/{name}` | Returns Pokémon info with original description |
| GET | `/pokemon/translated/{name}` | Returns Pokémon info with translated description |

### Translation logic
- Pokemon with `habitat = "cave"` or `isLegendary = true` → Yoda translation
- All other Pokemon → Shakespeare translation
- If translation fails for any reason (rate limit, service unavailable, etc.) → fallback to original description

### Input validation
Pokemon names must contain only letters, numbers and hyphens. The following inputs are rejected with a `400 Bad Request`:
- Empty or white space
- Names composed only of digits (IDs are not accepted, names only)
- Names with special characters or spaces

Valid examples: `mewtwo`, `mr-mime`, `nidoran-f`, `porygon2`, `ho-oh`

### Example response
```json
{
  "name": "mewtwo",
  "description": "It was created by a scientist after years of horrific gene-splicing experiments.",
  "habitat": "rare",
  "isLegendary": true
}
```

---

## How to run

### Option 1 — Docker (recommended, nothing else required)

1. Install [Docker Desktop](https://www.docker.com/products/docker-desktop/)
2. Clone the repository
```bash
git clone https://github.com/Alessandro-DellOrto/FunPokedexApi.git
cd FunPokedexApi
```
3. Build and run
```bash
docker compose up --build
```
4. The API is available at `http://localhost:8080`

```bash
# Examples
curl http://localhost:8080/pokemon/mewtwo
curl http://localhost:8080/pokemon/translated/mewtwo
```

### Option 2 — .NET 10 SDK

1. Install [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
2. Clone the repository
```bash
git clone https://github.com/Alessandro-DellOrto/FunPokedexApi.git
cd FunPokedexApi
```
3. Run
```bash
dotnet run --project FunPokedexApi/FunPokedexApi.csproj
```
4. The API is available at `http://localhost:5054`
5. Swagger UI is available at `http://localhost:5054/swagger`

### Run tests
```bash
dotnet test
```

---

## Configuration

External API base URLs are defined in `appsettings.json` and can be overridden via environment variables without rebuilding the application:

```json
{
  "ExternalApis": {
    "PokeApi": {
      "BaseUrl": "https://pokeapi.co/api/v2/"
    },
    "FunTranslations": {
      "BaseUrl": "https://api.funtranslations.mercxry.me/v1/"
    }
  }
}
```

When running via Docker, these can be overridden in `docker-compose.yml`:

```yaml
environment:
  - ExternalApis__PokeApi__BaseUrl=https://pokeapi.co/api/v2/
  - ExternalApis__FunTranslations__BaseUrl=https://api.funtranslations.mercxry.me/v1/
```

---

## Architecture overview

The solution follows a layered architecture inspired by Clean Architecture principles:

- **FunPokedexApi** — Web layer, Minimal API endpoints, middleware
- **FunPokedexApi.Application** — Business logic, interfaces, domain models
- **FunPokedexApi.Infrastructure** — HTTP clients, DTOs, dependency injection wiring

Dependencies flow inward: Infrastructure and Web depend on Application, never the other way around.

---

## Project structure

```
FunPokedexApi/
├── FunPokedexApi/                  # Web layer
│   ├── Endpoints/
│   ├── Middleware/
│   ├── Validators/
│   └── Program.cs
├── FunPokedexApi.Application/      # Business logic
│   ├── Interfaces/
│   ├── Models/
│   └── Services/
├── FunPokedexApi.Infrastructure/   # External integrations
│   ├── ApiClients/
│   ├── DTOs/
│   └── DependencyInjection.cs
└── tests/
    ├── FunPokedexApi.UnitTests/
    └── FunPokedexApi.IntegrationTests/  # Scaffolded, see note below
```

> **Integration Tests:** The `FunPokedexApi.IntegrationTests` project is scaffolded and ready.
> The recommended approach is **WireMock.Net** alongside **WebApplicationFactory** to spin up 
> the application in memory and simulate external API responses without hitting real endpoints.

---

## What I'd do differently for a production API

### Caching
Pokémon data is immutable, so both endpoints are ideal candidates for caching. 
I would introduce a distributed cache using **Redis** to avoid redundant HTTP calls to PokéAPI and FunTranslations. 
This would also help mitigate the strict rate limit imposed by FunTranslations.
For simplicity, this implementation does not include any caching layer.

### Resilience — Circuit Breaker
I would add **Polly** to implement a circuit breaker pattern on the FunTranslations client. 
When the service starts returning errors (e.g. `429 Too Many Requests`), the circuit breaker would open and stop forwarding requests for a configured period, falling back to the original description immediately.

### Authentication & Authorization
If access to the API needs to be restricted, I would implement an authentication mechanism such as **OAuth 2.0 / JWT Bearer tokens**, integrating with an identity provider (e.g. Azure AD, Auth0) via ASP.NET's built-in middleware.

### Structured Logging
Currently exceptions in `PokemonService` are logged via `ILogger` with a warning level. 
In production I would integrate **Serilog** with structured JSON output, adding a correlation ID per request to trace the full lifecycle of each call across services. 
Logs would be shipped to a centralised platform (e.g. Elastic, Datadog, Azure Monitor).

### Environment-specific Configuration
Currently the application uses a single `appsettings.json`. 
In production I would introduce environment-specific configuration files (`appsettings.Staging.json`, `appsettings.Production.json`) with sensitive values (API keys, connection strings) managed via environment variables or a secrets vault, never committed to source control. 
ASP.NET's configuration system supports this out of the box.

### Health Checks
I would expose a `/health` endpoint using ASP.NET's built-in `AddHealthChecks()` to allow Docker or Kubernetes to monitor container readiness and liveness.

### Rate Limiting
Beyond the circuit breaker on outbound calls, I would protect our own endpoints from abuse using ASP.NET's built-in rate limiting middleware.