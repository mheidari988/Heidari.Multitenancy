# Application contracts

This guide explains the application-layer interfaces (`ICurrentTenant`, `ITenantProvider`, `ICacheable`, `ICacheProvider`) and shows host-level examples for each. These contracts stay framework-agnostic; you implement them in your host (API, worker, etc.) and register them for the MediatR behaviors to consume.

## ICurrentTenant

Represents the active tenant for the current execution scope (per HTTP request, job, or message). The application behaviors assume it is already populated.

### Responsibilities

- Expose `TenantId Id`, `string Identifier`, `string Name`, and `bool HasTenant`.
- Contain no framework types; resolution happens in your host.

### Example: Web API resolving from JWT/header

```csharp
using Heidari.Multitenancy.Application.Interfaces;
using Heidari.Multitenancy.Domain.Models;

public sealed class HttpCurrentTenant : ICurrentTenant
{
    public HttpCurrentTenant(TenantId id, string identifier, string name)
    {
        Id = id;
        Identifier = identifier;
        Name = name;
        HasTenant = true;
    }

    private HttpCurrentTenant()
    {
        HasTenant = false;
        Identifier = string.Empty;
        Name = string.Empty;
    }

    public static ICurrentTenant FromHttp(HttpContext context)
    {
        // Resolve however you prefer: JWT claim, header, or route segment.
        var tenantSlug = context.User.FindFirst("tenant")?.Value
                         ?? context.Request.Headers["X-Tenant"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(tenantSlug))
        {
            return new HttpCurrentTenant();
        }

        return new HttpCurrentTenant(new TenantId(tenantSlug), tenantSlug, tenantSlug);
    }

    public TenantId Id { get; }
    public string Identifier { get; }
    public string Name { get; }
    public bool HasTenant { get; }
}
```

Register it as scoped per request in your DI container after resolving the tenant from the HTTP pipeline.

## ITenantProvider

Retrieves tenant metadata (id, name, optional connection string) by `TenantId` or identifier. Use it wherever you need to translate a slug to a full tenant record.

### Responsibilities

- Provide lookups without leaking infrastructure-specific details to application code.
- Implemented in your host/infrastructure (database, config, directory, etc.).

### Example: In-memory/provider backed by a repository

```csharp
using Heidari.Multitenancy.Application.Interfaces;
using Heidari.Multitenancy.Domain.Models;

public sealed class TenantProvider : ITenantProvider
{
    private readonly ITenantRepository _repo; // your own abstraction

    public TenantProvider(ITenantRepository repo)
    {
        _repo = repo;
    }

    public TenantInfo? GetTenantInfo(TenantId tenantId) =>
        _repo.FindById(tenantId);

    public TenantInfo? GetTenantInfo(string identifier) =>
        _repo.FindByIdentifier(identifier);
}
```

Use case: when a request comes in with a tenant slug, resolve it to `TenantInfo` for provisioning, connection-string selection, or logging.

## ICacheable

Marks MediatR requests that can be cached. `TenantScopedCacheBehavior` uses it to build tenant-specific cache keys and short-circuit handlers.

### Properties

- `string CacheKey` – stable, deterministic key for the request.
- `bool BypassCache` – opt-out per request.
- `int? AbsoluteExpirationSeconds` – optional absolute expiration.
- `int? SlidingExpirationSeconds` – optional sliding expiration.

### Example: Cacheable query

```csharp
using Heidari.Multitenancy.Application.Interfaces;
using Heidari.Multitenancy.Domain.Models;
using MediatR;

public sealed class GetDashboardQuery : IRequest<DashboardDto>, ICacheable
{
    public GetDashboardQuery(TenantId tenantId, string userId)
    {
        TenantId = tenantId;
        UserId = userId;
    }

    public TenantId TenantId { get; }
    public string UserId { get; }

    public string CacheKey => $"dashboard:{UserId}";
    public bool BypassCache { get; init; }
    public int? AbsoluteExpirationSeconds { get; init; } = 300;
    public int? SlidingExpirationSeconds { get; init; } = 60;
}
```

Key format applied by the behavior: `Tenant:{tenantId}:{CacheKey}` (e.g., `Tenant:tenant-1:dashboard:42`).

### Real-world use case: tenant dashboard caching (5-minute cache per tenant)

Scenario: You have a Clean Architecture app with MediatR. The “Tenant dashboard” query is expensive (aggregations). You want it cached per tenant for 5 minutes.

1. Define the query and make it cacheable:

   ```csharp
   using Heidari.Multitenancy.Application.Interfaces;
   using Heidari.Multitenancy.Domain.Models;
   using MediatR;

   public sealed class GetDashboardQuery : IRequest<DashboardDto>, ICacheable
   {
       public GetDashboardQuery(TenantId tenantId, string userId)
       {
           TenantId = tenantId;
           UserId = userId;
       }

       public TenantId TenantId { get; }
       public string UserId { get; }

       public string CacheKey => $"dashboard:{UserId}";
       public bool BypassCache { get; init; }
       public int? AbsoluteExpirationSeconds { get; init; } = 300; // 5 minutes
       public int? SlidingExpirationSeconds { get; init; } = 60;  // optional
   }
   ```

2. Implement the handler as usual (no cache code inside):

   ```csharp
   public sealed class GetDashboardHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
   {
       // inject repositories, etc.
       public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken ct)
       {
           // expensive aggregation for this tenant/user
           // return the DTO
           return await _dashboardAggregator.BuildAsync(request.TenantId, request.UserId, ct);
       }
   }
   ```

3. Host setup (once):
   - Register `TenantScopedCacheBehavior<,>` (plus enforcement/validation behaviors) with MediatR.
   - Provide `ICurrentTenant` from your host (JWT/header/route).
   - Provide an `ICacheProvider` that wraps your cache (IMemoryCache/Redis).

Runtime flow:

- Request arrives with tenant set in `ICurrentTenant`.
- `TenantScopedCacheBehavior` sees `GetDashboardQuery` implements `ICacheable`.
- It builds `Tenant:{tenantId}:{CacheKey}` (e.g., `Tenant:tenant-123:dashboard:alice`).
- If cached, it returns the cached `DashboardDto` immediately.
- If not cached, it runs the handler, stores the result with the provided expirations, and returns it.

Result: per-tenant caching with no cache logic in handlers, and keys are automatically scoped so tenant A never receives tenant B’s cached data.

## ICacheProvider

Minimal cache abstraction used by `TenantScopedCacheBehavior` to store/read responses.

### Responsibilities

- Provide `TryGetValue<T>` and `Set<T>` with optional expirations.
- Hide framework-specific cache types from the application package.

### Example: Wrapping IMemoryCache (host-level)

```csharp
using Heidari.Multitenancy.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

public sealed class MemoryCacheProvider : ICacheProvider
{
    private readonly IMemoryCache _cache;

    public MemoryCacheProvider(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool TryGetValue<T>(string key, out T? value)
    {
        if (_cache.TryGetValue(key, out var obj) && obj is T typed)
        {
            value = typed;
            return true;
        }

        value = default;
        return false;
    }

    public void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
    {
        var options = new MemoryCacheEntryOptions();
        if (absoluteExpiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = absoluteExpiration;
        }
        if (slidingExpiration.HasValue)
        {
            options.SlidingExpiration = slidingExpiration;
        }

        _cache.Set(key, value, options);
    }
}
```

Use case: the MediatR pipeline caches query responses per tenant; subsequent identical requests return immediately from cache when the tenant and `CacheKey` match.

### Real-world use case: host-level cache provider (IMemoryCache)

Scenario: You’re running an API with MediatR. You want `TenantScopedCacheBehavior` to use your in-process cache. Implement `ICacheProvider` once and let the behavior handle tenant scoping.

1. Implement `ICacheProvider` with `IMemoryCache` (above).
2. Register it in DI:
   ```csharp
   services.AddMemoryCache();
   services.AddScoped<ICacheProvider, MemoryCacheProvider>();
   ```
3. Ensure you’ve registered `TenantScopedCacheBehavior<,>` in MediatR and provided `ICurrentTenant`.

Runtime flow:

- A cacheable request arrives with a tenant set.
- `TenantScopedCacheBehavior` builds `Tenant:{tenantId}:{CacheKey}` and calls your `MemoryCacheProvider`.
- On hit, the cached response is returned immediately.
- On miss, the handler runs; the behavior calls `Set` with the optional absolute/sliding expirations you specified on the request.

Result: You get tenant-prefixed caching without leaking `IMemoryCache` into application code, and you can swap implementations (e.g., Redis) by changing only the `ICacheProvider` binding.

System design (request to response):

1. Client sends an HTTP request with a tenant token/header/route value.
2. Middleware resolves the tenant and populates `ICurrentTenant` (scoped).
3. Controller/endpoint calls `IMediator.Send(request)`.
4. MediatR pipeline runs behaviors in order:
   - `TenantEnforcementBehavior`: ensures a tenant exists if `[RequiresTenant]`.
   - `TenantValidationBehavior`: compares request `TenantId` to `ICurrentTenant.Id`.
   - `TenantScopedCacheBehavior`: if `request is ICacheable` and tenant is set, builds `Tenant:{tenantId}:{CacheKey}` and checks `ICacheProvider`.
5. Cache decision:
   - Cache hit: return cached response immediately.
   - Cache miss: invoke the handler; on success, `Set` the result with the request’s expirations.
6. Response flows back through MediatR to the controller and then to the client.

## TenantRequestBase<TResponse>

Abstract base for tenant-scoped MediatR requests. It enforces a non-default `TenantId` and is decorated with `[RequiresTenant]`, so the enforcement behavior fails fast if no tenant is set.

### Responsibilities
- Require a `TenantId` in the constructor (guards against default).
- Carry the `TenantId` property for validation (`TenantValidationBehavior`).
- Signal tenant requirement via `[RequiresTenant]`.

### Real-world use case: tenant-scoped command
Scenario: A multitenant SaaS needs to create projects per tenant. Every create command must include the tenant and must not run without one.

1. Define the command by extending `TenantRequestBase`:
   ```csharp
   using Heidari.Multitenancy.Application.Abstractions;
   using Heidari.Multitenancy.Domain.Models;
   using MediatR;

   public sealed class CreateProjectCommand : TenantRequestBase<Guid>
   {
       public CreateProjectCommand(TenantId tenantId, string name)
           : base(tenantId)
       {
           Name = name;
       }

       public string Name { get; }
   }
   ```

2. Implement the handler; rely on the pipeline to ensure tenant presence and validation:
   ```csharp
   using Heidari.Multitenancy.Application.Interfaces;

   public sealed class CreateProjectHandler : IRequestHandler<CreateProjectCommand, Guid>
   {
       private readonly ICurrentTenant _currentTenant;
       private readonly IProjectRepository _projects; // your abstraction

       public CreateProjectHandler(ICurrentTenant currentTenant, IProjectRepository projects)
       {
           _currentTenant = currentTenant;
           _projects = projects;
       }

       public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken ct)
       {
           // TenantEnforcementBehavior ensured a tenant is set,
           // TenantValidationBehavior ensured request.TenantId matches ICurrentTenant.Id.

           var project = new Project(request.TenantId, request.Name);
           await _projects.SaveAsync(project, ct);
           return project.Id;
       }
   }
   ```

3. Runtime flow:
   - Client sends command with tenant in JWT/header/route; host populates `ICurrentTenant`.
   - MediatR pipeline:
     - `TenantEnforcementBehavior` rejects if no tenant is set (because `[RequiresTenant]`).
     - `TenantValidationBehavior` rejects if `request.TenantId` != `ICurrentTenant.Id`.
     - Handler executes and persists the tenant-scoped entity.

Result: Tenant presence and correctness are guaranteed by construction; handlers can assume the tenant is already enforced and validated.
