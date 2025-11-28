using Heidari.Multitenancy.Application.Interfaces;
using Heidari.Multitenancy.Domain.Exceptions;
using Heidari.Multitenancy.Domain.Models;
using MediatR;

namespace Heidari.Multitenancy.Application.Behaviors;

/// <summary>
/// Validates that tenant-scoped requests match the tenant associated with the current context.
/// </summary>
public sealed class TenantValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : notnull
{
	private readonly ICurrentTenant _currentTenant;

	/// <summary>
	/// Initializes a new instance of the <see cref="TenantValidationBehavior{TRequest, TResponse}"/> class.
	/// </summary>
	/// <param name="currentTenant">Provides access to the current tenant context.</param>
	public TenantValidationBehavior(ICurrentTenant currentTenant)
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

		if (_currentTenant.HasTenant)
		{
			ValidateTenant(request);
		}

		return next();
	}

	private void ValidateTenant(TRequest request)
	{
		var tenantIdProperty = request.GetType().GetProperty("TenantId");
		if (tenantIdProperty == null)
		{
			return;
		}

		var tenantIdType = Nullable.GetUnderlyingType(tenantIdProperty.PropertyType) ?? tenantIdProperty.PropertyType;
		if (tenantIdType != typeof(TenantId))
		{
			return;
		}

		var value = tenantIdProperty.GetValue(request);
		if (value is not TenantId requestTenantId)
		{
			return;
		}

		if (_currentTenant.Id != requestTenantId)
		{
			throw new TenantAccessException(_currentTenant.Id, requestTenantId);
		}
	}
}
