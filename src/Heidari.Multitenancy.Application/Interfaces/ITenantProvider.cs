using Heidari.Multitenancy.Domain.Models;

namespace Heidari.Multitenancy.Application.Interfaces;

/// <summary>
/// Retrieves tenant metadata independent of infrastructure concerns.
/// </summary>
public interface ITenantProvider
{
	/// <summary>
	/// Gets metadata for the specified tenant identifier.
	/// </summary>
	/// <param name="tenantId">The tenant identifier.</param>
	/// <returns>The tenant metadata when found; otherwise, <c>null</c>.</returns>
	TenantInfo? GetTenantInfo(TenantId tenantId);

	/// <summary>
	/// Gets metadata for the specified tenant identifier string (e.g., slug).
	/// </summary>
	/// <param name="identifier">The tenant identifier string.</param>
	/// <returns>The tenant metadata when found; otherwise, <c>null</c>.</returns>
	TenantInfo? GetTenantInfo(string identifier);
}
