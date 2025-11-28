using Heidari.Multitenancy.Application.Interfaces;
using Heidari.Multitenancy.Domain.Models;

namespace Heidari.Multitenancy.Tests.Application.TestDoubles;

internal sealed class FakeCurrentTenant : ICurrentTenant
{
	public FakeCurrentTenant(bool hasTenant = false, TenantId? tenantId = null, string? identifier = null, string? name = null)
	{
		if (hasTenant)
		{
			var id = tenantId ?? new TenantId("tenant-1");
			SetTenant(id, identifier, name);
		}
		else
		{
			ClearTenant();
		}
	}

	public TenantId Id { get; private set; }

	public string Identifier { get; private set; } = string.Empty;

	public string Name { get; private set; } = string.Empty;

	public bool HasTenant { get; private set; }

	public void SetTenant(TenantId tenantId, string? identifier = null, string? name = null)
	{
		Id = tenantId;
		Identifier = identifier ?? tenantId.Value;
		Name = name ?? tenantId.Value;
		HasTenant = true;
	}

	public void ClearTenant()
	{
		Id = default;
		Identifier = string.Empty;
		Name = string.Empty;
		HasTenant = false;
	}
}

internal sealed class FakeCacheProvider : ICacheProvider
{
	private readonly Dictionary<string, CacheEntry> _entries = new();

	public IReadOnlyDictionary<string, CacheEntry> Entries => _entries;

	public bool TryGetValue<T>(string key, out T? value)
	{
		if (_entries.TryGetValue(key, out var entry))
		{
			if (entry.Value is T typedValue)
			{
				value = typedValue;
				return true;
			}

			if (entry.Value is null)
			{
				value = default;
				return true;
			}
		}

		value = default;
		return false;
	}

	public void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
	{
		_entries[key] = new CacheEntry(value, absoluteExpiration, slidingExpiration);
	}

	internal readonly record struct CacheEntry(object? Value, TimeSpan? AbsoluteExpiration, TimeSpan? SlidingExpiration);
}
