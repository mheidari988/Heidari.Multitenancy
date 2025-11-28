# Application behaviors

The application package adds tenant-aware MediatR pipeline behaviors plus host-facing interfaces to keep tenant handling consistent without pulling framework dependencies into the domain.

## What you get
- `TenantEnforcementBehavior` – fails fast with `TenantNotSetException` when a `[RequiresTenant]` request runs without a tenant.
- `TenantValidationBehavior` – compares the request `TenantId` to `ICurrentTenant.Id` and throws `TenantAccessException` on mismatch.
- `TenantScopedCacheBehavior` – caches `ICacheable` responses per tenant using the key format `Tenant:{tenantId}:{CacheKey}`.
- Contracts: `ICurrentTenant`, `ITenantProvider`, `ICacheProvider`, `ICacheable`, and the `[RequiresTenant]` attribute plus `TenantRequestBase<TResponse>`. See [Application contracts](application-contracts.md) for host-side guidance and examples.

## Setup
1) Install the package in your application layer:
   ```
   dotnet add package Heidari.Multitenancy.Application
   ```
2) Implement host-specific services (web worker, API, messaging host, etc.):
   - `ICurrentTenant`: resolve the tenant from JWT/headers/route and expose it for the current scope.
   - `ICacheProvider`: wrap your cache of choice (e.g., IMemoryCache, Redis) behind the provided interface.
   - `ITenantProvider` (optional): load tenant metadata (id, name, connection string) from your tenant registry or configuration.
3) Register MediatR and the pipeline behaviors:
   ```csharp
   services.AddMediatR(cfg =>
   {
       cfg.AddOpenBehavior(typeof(TenantEnforcementBehavior<,>));
       cfg.AddOpenBehavior(typeof(TenantValidationBehavior<,>));
       cfg.AddOpenBehavior(typeof(TenantScopedCacheBehavior<,>));
   });
   ```

## Request patterns
- Use `TenantRequestBase<TResponse>` for tenant-scoped commands/queries; it enforces a non-default `TenantId` and is already marked with `[RequiresTenant]`.
- For other request types, add `[RequiresTenant]` when a tenant must be present.
- Implement `ICacheable` on queries you want cached; set `CacheKey`, `BypassCache`, and optional expiration seconds.

## How tenant resolution fits in
- The behaviors assume your host has already populated `ICurrentTenant` for the current scope; they do not set it.
- Typical flow: middleware/filter extracts the tenant (e.g., from JWT), populates `ICurrentTenant`, then handlers consume it for filtering, persistence, or logging.
- `ITenantProvider` is for your own handlers/services to fetch tenant metadata without coupling to infrastructure details.

## Caching notes
- Caching runs only when:
  - A tenant is present (`ICurrentTenant.HasTenant` is true).
  - The request implements `ICacheable` and `BypassCache` is false.
- Absolute and sliding expirations are optional and expressed in seconds on the request; the behavior converts them to `TimeSpan` when storing.

See also: [Tenant context propagation](tenant-context-propagation.md) for host-level patterns to set `ICurrentTenant`/`ITenantContext`.
