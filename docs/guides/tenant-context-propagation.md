# Tenant context propagation

These patterns show how to surface `ITenantContext` across different hosts while keeping the domain package dependency-free.

## ASP.NET Core (HTTP requests)
1) Resolve the tenant identifier from the incoming request (header, host, path segment, token).
2) Create an `ITenantContext` implementation per request and register it as scoped.
3) Inject `ITenantContext` into application services that require tenant awareness.
```csharp
builder.Services.AddScoped<ITenantContext>(sp =>
{
    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    var tenantId = ResolveTenantFromRequest(httpContext); // your logic
    return new RequestTenantContext(tenantId);
});
```

## Background workers
- Extract tenant metadata from the job payload or message.
- Construct the `ITenantContext` before executing the job handler.
```csharp
public async Task HandleAsync(JobPayload payload, CancellationToken ct)
{
    var context = new AmbientTenantContext(new TenantId(payload.TenantId));
    // pass context into the handler or store it in an ambient scope your handler can read
}
```

## Messaging
- Include the tenant identifier in message headers.
- Middleware/filters on the consumer side read the header and set up `ITenantContext` for the handler pipeline.

## Data access enforcement
- In repositories or query handlers, compare the expected tenant to the context tenant and throw `TenantAccessException` on mismatch.
- Prefer passing `TenantId` explicitly into queries/commands to keep tenant checks obvious.

Keep tenant resolution close to the host boundary, and let the rest of your application depend only on the `ITenantContext` contract and domain exceptions. If you use the application package, surface the same resolved tenant through your `ICurrentTenant` implementation so the MediatR behaviors can enforce and validate it. This preserves purity while enabling enforcement wherever tenant mismatches matter.
