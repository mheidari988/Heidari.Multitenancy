namespace Heidari.Multitenancy.Application.Interfaces;

/// <summary>
/// Provides a minimal cache abstraction for tenant-scoped request handling.
/// </summary>
public interface ICacheProvider
{
	/// <summary>
	/// Attempts to retrieve a cached value for the specified key.
	/// </summary>
	/// <param name="key">The cache key.</param>
	/// <param name="value">The cached value, when present.</param>
	/// <typeparam name="T">The type of the cached value.</typeparam>
	/// <returns><c>true</c> when an entry exists; otherwise, <c>false</c>.</returns>
	bool TryGetValue<T>(string key, out T? value);

	/// <summary>
	/// Stores a value in the cache with optional expirations.
	/// </summary>
	/// <param name="key">The cache key.</param>
	/// <param name="value">The value to store.</param>
	/// <param name="absoluteExpiration">Optional absolute expiration.</param>
	/// <param name="slidingExpiration">Optional sliding expiration.</param>
	/// <typeparam name="T">The type of the cached value.</typeparam>
	void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);
}
