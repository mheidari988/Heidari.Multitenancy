namespace Heidari.Multitenancy.Application.Interfaces;

/// <summary>
/// Marks a MediatR request as supporting tenant-scoped caching.
/// </summary>
public interface ICacheable
{
	/// <summary>
	/// Gets the cache key representing the request.
	/// </summary>
	string CacheKey { get; }

	/// <summary>
	/// Gets a value indicating whether the cache should be bypassed for this request.
	/// </summary>
	bool BypassCache { get; }

	/// <summary>
	/// Gets the absolute expiration time in seconds for the cache entry.
	/// </summary>
	int? AbsoluteExpirationSeconds { get; }

	/// <summary>
	/// Gets the sliding expiration time in seconds for the cache entry.
	/// </summary>
	int? SlidingExpirationSeconds { get; }
}
