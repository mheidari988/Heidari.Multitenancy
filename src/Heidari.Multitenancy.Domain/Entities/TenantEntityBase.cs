using Heidari.Multitenancy.Domain.Interfaces;
using Heidari.Multitenancy.Domain.Models;

namespace Heidari.Multitenancy.Domain.Entities;

/// <summary>
/// Optional base type for tenant-scoped domain entities.
/// </summary>
/// <remarks>
/// Keeps tenant awareness in one place while allowing consumers to compose with other base types (auditing, soft-delete, etc.).
/// </remarks>
public abstract class TenantEntityBase : IHasTenant
{
	/// <summary>
	/// Gets the identifier of the tenant that owns this entity.
	/// </summary>
	public TenantId TenantId { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="TenantEntityBase"/> class.
	/// </summary>
	/// <param name="tenantId">The tenant that owns the entity.</param>
	/// <exception cref="ArgumentException">Thrown when <paramref name="tenantId"/> is not provided.</exception>
	protected TenantEntityBase(TenantId tenantId)
	{
		if (tenantId == default)
		{
			throw new ArgumentException("Tenant id must be specified.", nameof(tenantId));
		}

		TenantId = tenantId;
	}
}
