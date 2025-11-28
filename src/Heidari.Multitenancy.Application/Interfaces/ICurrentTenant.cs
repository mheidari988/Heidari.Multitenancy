using Heidari.Multitenancy.Domain.Models;

namespace Heidari.Multitenancy.Application.Interfaces;

/// <summary>
/// Represents the active tenant associated with the current execution context.
/// </summary>
/// <remarks>
/// Implementations are provided by the hosting layer to decouple context detection from application logic.
/// </remarks>
public interface ICurrentTenant
{
	/// <summary>
	/// Gets the unique identifier of the current tenant.
	/// </summary>
	TenantId Id { get; }

	/// <summary>
	/// Gets the human-readable or slug identifier for the tenant.
	/// </summary>
	string Identifier { get; }

	/// <summary>
	/// Gets the display name for the tenant.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Gets a value indicating whether a tenant has been associated with the current context.
	/// </summary>
	bool HasTenant { get; }
}
