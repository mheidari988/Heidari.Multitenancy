using Heidari.Multitenancy.Application.Attributes;
using Heidari.Multitenancy.Application.Interfaces;
using Heidari.Multitenancy.Domain.Exceptions;
using MediatR;

namespace Heidari.Multitenancy.Application.Behaviors;

/// <summary>
/// Ensures tenant-aware requests execute only when a tenant is present in the current context.
/// </summary>
public sealed class TenantEnforcementBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : notnull
{
	private readonly ICurrentTenant _currentTenant;

	/// <summary>
	/// Initializes a new instance of the <see cref="TenantEnforcementBehavior{TRequest, TResponse}"/> class.
	/// </summary>
	/// <param name="currentTenant">Provides access to the active tenant context.</param>
	public TenantEnforcementBehavior(ICurrentTenant currentTenant)
	{
		_currentTenant = currentTenant ?? throw new ArgumentNullException(nameof(currentTenant));
	}

	/// <inheritdoc />
	public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		if (request is null)
		{
			throw new ArgumentNullException(nameof(request));
		}

		cancellationToken.ThrowIfCancellationRequested();

		if (IsTenantRequired(request) && !_currentTenant.HasTenant)
		{
			throw new TenantNotSetException();
		}

		return next();
	}

	private static bool IsTenantRequired(TRequest request)
	{
		var requestType = request.GetType();
		return Attribute.IsDefined(requestType, typeof(RequiresTenantAttribute), inherit: true);
	}
}
