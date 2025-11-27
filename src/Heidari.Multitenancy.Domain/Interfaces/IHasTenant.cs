using Heidari.Multitenancy.Domain.Models;

namespace Heidari.Multitenancy.Domain.Interfaces;

/// <summary>
/// Marks a domain type as belonging to a specific tenant.
/// </summary>
public interface IHasTenant
{
	/// <summary>
	/// Gets the tenant identifier to which the entity belongs.
	/// </summary>
	TenantId TenantId { get; }
}
