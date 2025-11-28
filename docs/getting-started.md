# Getting started

Follow these steps to add tenant awareness to your solution using the domain package.

## 1) Install the domain package
```
dotnet add package Heidari.Multitenancy.Domain
```

## 2) Model tenant ownership in your entities
Use the provided `TenantId` and `IHasTenant` contract to mark tenant-scoped entities.
```csharp
using Heidari.Multitenancy.Domain.Interfaces;
using Heidari.Multitenancy.Domain.Models;

public sealed class Project : IHasTenant
{
    public Project(TenantId tenantId, string name)
    {
        TenantId = tenantId;
        Name = name;
    }

    public TenantId TenantId { get; }
    public string Name { get; }
}
```

If you prefer a base class, extend `TenantEntityBase` and combine it with your own auditing or soft-delete bases as needed.

## 3) Surface the current tenant in your application layer
Implement `ITenantContext` to expose the tenant for the current execution scope (e.g., per HTTP request, job, or message).
```csharp
using Heidari.Multitenancy.Domain.Interfaces;
using Heidari.Multitenancy.Domain.Models;

public sealed class RequestTenantContext : ITenantContext
{
    public RequestTenantContext(TenantId? tenantId)
    {
        CurrentTenantId = tenantId;
    }

    public TenantId? CurrentTenantId { get; }
    public bool HasTenant => CurrentTenantId is not null;
}
```

## 4) Enforce tenant correctness in your application services
When a tenant is required but missing, throw `TenantNotSetException`. When a mismatch occurs between expected and actual tenants, throw `TenantAccessException`.
```csharp
using Heidari.Multitenancy.Domain.Exceptions;
using Heidari.Multitenancy.Domain.Interfaces;
using Heidari.Multitenancy.Domain.Models;

public sealed class InvoiceService
{
    private readonly ITenantContext _context;

    public InvoiceService(ITenantContext context)
    {
        _context = context;
    }

    public Task<Invoice> GetAsync(TenantId tenantId, string invoiceNumber)
    {
        if (!_context.HasTenant)
        {
            throw new TenantNotSetException();
        }

        if (_context.CurrentTenantId != tenantId)
        {
            throw new TenantAccessException(tenantId, _context.CurrentTenantId!.Value);
        }

        // Load the invoice via your persistence layer
        throw new NotImplementedException();
    }
}
```

## 5) (Optional) Add the application package for MediatR pipelines and caching
- Install: `dotnet add package Heidari.Multitenancy.Application`
- Register behaviors: `TenantEnforcementBehavior`, `TenantValidationBehavior`, and `TenantScopedCacheBehavior`.
- Provide host-specific services:
  - `ICurrentTenant`: read the tenant from your host boundary (JWT, header, route) and expose it for the current scope.
  - `ICacheProvider`: wrap your cache of choice without leaking framework types.
  - `ITenantProvider`: look up tenant metadata (id, name, connection string) from your tenant registry.

See the [Application behaviors](guides/application-behaviors.md) guide for a full walkthrough.

Next steps:
- Learn the abstractions in depth: [Tenant abstractions](concepts/tenant-abstractions.md).
- See how to propagate tenant info through different hosts: [Tenant context propagation](guides/tenant-context-propagation.md).
