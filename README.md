# Heidari.Multitenancy

Lightweight, clean-architecture-friendly multitenancy building blocks for .NET. This repository starts with pure domain abstractions (no framework dependencies) and is designed to be extended with optional integration packages.

## Packages
- Heidari.Multitenancy.Domain – domain-level value objects, interfaces, and domain exceptions for tenant-aware systems. See [Overview](docs/overview.md) and [Tenant abstractions](docs/concepts/tenant-abstractions.md).
- Heidari.Multitenancy.Application – MediatR-friendly tenant behaviors, caching hooks, and application-layer contracts (`ICurrentTenant`, `ITenantProvider`, `ICacheProvider`, `ICacheable`). See [Application behaviors](docs/guides/application-behaviors.md).

## Quickstart
1) Install the domain package into your project:  
   `dotnet add package Heidari.Multitenancy.Domain`
2) Model tenant ownership in your entities:
   ```csharp
   using Heidari.Multitenancy.Domain.Interfaces;
   using Heidari.Multitenancy.Domain.Models;

   public sealed class Order : IHasTenant
   {
       public Order(TenantId tenantId, string number)
       {
           TenantId = tenantId;
           Number = number;
       }

       public TenantId TenantId { get; }
       public string Number { get; }
   }
   ```
3) Provide tenant context in your application layer (web request, background job, etc.):
   ```csharp
   using Heidari.Multitenancy.Domain.Interfaces;
   using Heidari.Multitenancy.Domain.Models;

   // Example implementation; actual retrieval is up to your host (middleware, filter, worker)
   public sealed class AmbientTenantContext : ITenantContext
   {
       public AmbientTenantContext(TenantId? currentTenantId)
       {
           CurrentTenantId = currentTenantId;
       }

       public TenantId? CurrentTenantId { get; }
       public bool HasTenant => CurrentTenantId is not null;
   }
   ```
4) (Optional) Add the application layer package for MediatR pipelines and caching:
   - Install: `dotnet add package Heidari.Multitenancy.Application`
   - Register behaviors: add `TenantEnforcementBehavior`, `TenantValidationBehavior`, and `TenantScopedCacheBehavior` to the MediatR pipeline.
   - Implement host-specific services:
     - `ICurrentTenant`: resolve from JWT/header/route and expose the active tenant.
     - `ICacheProvider`: wrap your cache of choice (e.g., IMemoryCache/Redis) without leaking framework details.
     - `ITenantProvider`: resolve tenant metadata from your tenant store or configuration.

## Documentation
- [Overview](docs/overview.md)
- [Getting started](docs/getting-started.md)
- Concepts: [Tenant abstractions](docs/concepts/tenant-abstractions.md)
- Guides: [Tenant context propagation](docs/guides/tenant-context-propagation.md), [Application behaviors](docs/guides/application-behaviors.md), [Application contracts](docs/guides/application-contracts.md)

## Contributing
- Keep the domain layer dependency-free; place integrations in dedicated packages.
- Add docs and samples alongside code changes.
- Open issues and PRs are welcome.

## License
[MIT](LICENSE)
