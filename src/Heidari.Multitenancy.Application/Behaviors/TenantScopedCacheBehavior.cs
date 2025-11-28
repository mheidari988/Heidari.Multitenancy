using Heidari.Multitenancy.Application.Interfaces;
using Heidari.Multitenancy.Domain.Models;
using MediatR;

namespace Heidari.Multitenancy.Application.Behaviors;

/// <summary>
/// Caches MediatR request responses within the scope of the current tenant.
/// </summary>
public sealed class TenantScopedCacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : notnull
{
	private readonly ICacheProvider _cacheProvider;
	private readonly ICurrentTenant _currentTenant;

	/// <summary>
	/// Initializes a new instance of the <see cref="TenantScopedCacheBehavior{TRequest, TResponse}"/> class.
	/// </summary>
	/// <param name="cacheProvider">Cache abstraction to store request results.</param>
	/// <param name="currentTenant">Provides access to the active tenant context.</param>
	public TenantScopedCacheBehavior(ICacheProvider cacheProvider, ICurrentTenant currentTenant)
	{
		_cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
		_currentTenant = currentTenant ?? throw new ArgumentNullException(nameof(currentTenant));
	}

	/// <inheritdoc />
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		if (request is null)
		{
			throw new ArgumentNullException(nameof(request));
		}

		cancellationToken.ThrowIfCancellationRequested();

		if (request is not ICacheable cacheable || cacheable.BypassCache || !_currentTenant.HasTenant)
		{
			return await next();
		}

		var cacheKey = BuildCacheKey(_currentTenant.Id, cacheable.CacheKey);

		if (_cacheProvider.TryGetValue(cacheKey, out TResponse? cachedResponse))
		{
			return cachedResponse!;
		}

		var response = await next();

		var absoluteExpiration = cacheable.AbsoluteExpirationSeconds.HasValue
			? TimeSpan.FromSeconds(cacheable.AbsoluteExpirationSeconds.Value)
			: (TimeSpan?)null;

		var slidingExpiration = cacheable.SlidingExpirationSeconds.HasValue
			? TimeSpan.FromSeconds(cacheable.SlidingExpirationSeconds.Value)
			: (TimeSpan?)null;

		_cacheProvider.Set(cacheKey, response, absoluteExpiration, slidingExpiration);

		return response;
	}

	private static string BuildCacheKey(TenantId tenantId, string cacheKey) => $"Tenant:{tenantId}:{cacheKey}";
}
