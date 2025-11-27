using Heidari.Multitenancy.Domain.Models;

namespace Heidari.Multitenancy.Domain.Interfaces;

/// <summary>
/// Provides access to the tenant associated with the current execution context.
/// </summary>
/// <remarks>
/// Implementations are expected to be supplied by the hosting application (e.g., web request, background job).
/// </remarks>
public interface ITenantContext
{
	/// <summary>
	/// Gets the identifier of the current tenant, or <c>null</c> when no tenant is associated.
	/// </summary>
	TenantId? CurrentTenantId { get; }

	/// <summary>
	/// Gets a value indicating whether a tenant has been associated with the current context.
	/// </summary>
	bool HasTenant { get; }
}
