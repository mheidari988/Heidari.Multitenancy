# Heidari.Multitenancy

Lightweight, clean-architecture-friendly multitenancy building blocks for .NET. This repository starts with pure domain abstractions (no framework dependencies) and is designed to be extended with optional integration packages.

## Packages
- Heidari.Multitenancy.Domain – domain-level value objects, interfaces, and domain exceptions for tenant-aware systems. See [Overview](docs/overview.md) and [Tenant abstractions](docs/concepts/tenant-abstractions.md).

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

## Documentation
- [Overview](docs/overview.md)
- [Getting started](docs/getting-started.md)
- Concepts: [Tenant abstractions](docs/concepts/tenant-abstractions.md)
- Guides: [Tenant context propagation](docs/guides/tenant-context-propagation.md)

## Contributing
- Keep the domain layer dependency-free; place integrations in dedicated packages.
- Add docs and samples alongside code changes.
- Open issues and PRs are welcome.

## License
[MIT](LICENSE)
