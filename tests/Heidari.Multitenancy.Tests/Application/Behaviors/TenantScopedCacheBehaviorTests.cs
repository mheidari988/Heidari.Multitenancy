using Heidari.Multitenancy.Application.Behaviors;
using Heidari.Multitenancy.Application.Interfaces;
using Heidari.Multitenancy.Domain.Models;
using Heidari.Multitenancy.Tests.Application.TestDoubles;
using MediatR;

namespace Heidari.Multitenancy.Tests.Application.Behaviors;

public class TenantScopedCacheBehaviorTests
{
	[Fact]
	public async Task Handle_ReturnsCachedResponse_WhenPresent()
	{
		var currentTenant = new FakeCurrentTenant(hasTenant: true, tenantId: new TenantId("tenant-1"));
		var cacheProvider = new FakeCacheProvider();
		cacheProvider.Set("Tenant:tenant-1:key", "cached-value");
		var behavior = new TenantScopedCacheBehavior<CacheableRequest, string>(cacheProvider, currentTenant);
		var request = new CacheableRequest("key");
		var nextCalled = false;

		var result = await behavior.Handle(request, () =>
		{
			nextCalled = true;
			return Task.FromResult("new-value");
		}, CancellationToken.None);

		Assert.False(nextCalled);
		Assert.Equal("cached-value", result);
	}

	[Fact]
	public async Task Handle_CachesResponse_WhenMissing()
	{
		var tenantId = new TenantId("tenant-1");
		var currentTenant = new FakeCurrentTenant(hasTenant: true, tenantId: tenantId);
		var cacheProvider = new FakeCacheProvider();
		var behavior = new TenantScopedCacheBehavior<CacheableRequest, string>(cacheProvider, currentTenant);
		var request = new CacheableRequest("cache-key", absoluteExpirationSeconds: 60, slidingExpirationSeconds: 30);

		var result = await behavior.Handle(request, () => Task.FromResult("fresh-value"), CancellationToken.None);

		Assert.True(cacheProvider.Entries.TryGetValue("Tenant:tenant-1:cache-key", out var entry));
		Assert.Equal("fresh-value", entry.Value);
		Assert.Equal(TimeSpan.FromSeconds(60), entry.AbsoluteExpiration);
		Assert.Equal(TimeSpan.FromSeconds(30), entry.SlidingExpiration);
		Assert.Equal("fresh-value", result);
	}

	[Fact]
	public async Task Handle_Bypasses_WhenRequested()
	{
		var currentTenant = new FakeCurrentTenant(hasTenant: true, tenantId: new TenantId("tenant-1"));
		var cacheProvider = new FakeCacheProvider();
		var behavior = new TenantScopedCacheBehavior<CacheableRequest, string>(cacheProvider, currentTenant);
		var request = new CacheableRequest("cache-key", bypassCache: true);

		var result = await behavior.Handle(request, () => Task.FromResult("fresh-value"), CancellationToken.None);

		Assert.Empty(cacheProvider.Entries);
		Assert.Equal("fresh-value", result);
	}

	[Fact]
	public async Task Handle_SkipsCache_WhenTenantNotSet()
	{
		var currentTenant = new FakeCurrentTenant();
		var cacheProvider = new FakeCacheProvider();
		var behavior = new TenantScopedCacheBehavior<CacheableRequest, string>(cacheProvider, currentTenant);
		var request = new CacheableRequest("cache-key");

		var result = await behavior.Handle(request, () => Task.FromResult("fresh-value"), CancellationToken.None);

		Assert.Empty(cacheProvider.Entries);
		Assert.Equal("fresh-value", result);
	}

	[Fact]
	public async Task Handle_Skips_WhenRequestNotCacheable()
	{
		var currentTenant = new FakeCurrentTenant(hasTenant: true, tenantId: new TenantId("tenant-1"));
		var cacheProvider = new FakeCacheProvider();
		var behavior = new TenantScopedCacheBehavior<NonCacheableRequest, string>(cacheProvider, currentTenant);
		var request = new NonCacheableRequest();

		var result = await behavior.Handle(request, () => Task.FromResult("fresh-value"), CancellationToken.None);

		Assert.Empty(cacheProvider.Entries);
		Assert.Equal("fresh-value", result);
	}

	private sealed class CacheableRequest : IRequest<string>, ICacheable
	{
		public CacheableRequest(string cacheKey, bool bypassCache = false, int? absoluteExpirationSeconds = null, int? slidingExpirationSeconds = null)
		{
			CacheKey = cacheKey;
			BypassCache = bypassCache;
			AbsoluteExpirationSeconds = absoluteExpirationSeconds;
			SlidingExpirationSeconds = slidingExpirationSeconds;
		}

		public string CacheKey { get; }

		public bool BypassCache { get; }

		public int? AbsoluteExpirationSeconds { get; }

		public int? SlidingExpirationSeconds { get; }
	}

	private sealed class NonCacheableRequest : IRequest<string>
	{
	}
}
