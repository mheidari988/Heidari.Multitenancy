# Tenant abstractions

The domain package provides a small set of types to model tenant awareness without pulling in any infrastructure dependencies.

## TenantId (value object)
- Strongly typed identifier for tenants.
- Immutable `record struct` with validation against null/whitespace.
- Use anywhere a tenant identifier is required to avoid mixing with other IDs.

## TenantInfo (metadata)
- Holds `TenantId`, `Name`, and optional `ConnectionString`.
- Pure data carrier; fetching and caching of tenant metadata is up to your application.

## ITenantContext
- Exposes the tenant associated with the current execution scope (`CurrentTenantId`, `HasTenant`).
- Implemented by the host (middleware, filter, background worker) to make tenant info available to domain/application services.

## IHasTenant
- Marker interface for domain entities that belong to a tenant.
- Encourages composition with other concerns (auditing, soft-delete, aggregate roots).

## TenantEntityBase (optional helper)
- Abstract base that implements `IHasTenant` and requires `TenantId` in its constructor.
- Useful when you want a single place to enforce tenant assignment, yet remain free to layer auditing/soft-delete in your own base classes.

## Domain exceptions
- `TenantNotSetException` – throw when a tenant is required but not present in the current context.
- `TenantAccessException` – throw when the tenant in context does not match the expected tenant for an operation.

These types are framework-agnostic and keep the domain layer pure. Framework-specific integrations (e.g., ASP.NET Core middleware, EF Core configuration) should live in opt-in packages built on top of these contracts.
