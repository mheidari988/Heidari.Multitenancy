using Heidari.Multitenancy.Application.Attributes;
using Heidari.Multitenancy.Domain.Models;
using MediatR;

namespace Heidari.Multitenancy.Application.Abstractions;

/// <summary>
/// Base type for tenant-scoped MediatR requests.
/// </summary>
[RequiresTenant]
public abstract class TenantRequestBase<TResponse> : IRequest<TResponse>
{
	/// <summary>
	/// Gets the identifier of the tenant associated with the request.
	/// </summary>
	public TenantId TenantId { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="TenantRequestBase{TResponse}"/> class.
	/// </summary>
	/// <param name="tenantId">The tenant identifier.</param>
	/// <exception cref="ArgumentException">Thrown when <paramref name="tenantId"/> is not provided.</exception>
	protected TenantRequestBase(TenantId tenantId)
	{
		if (tenantId == default)
		{
			throw new ArgumentException("Tenant id must be specified.", nameof(tenantId));
		}

		TenantId = tenantId;
	}
}
