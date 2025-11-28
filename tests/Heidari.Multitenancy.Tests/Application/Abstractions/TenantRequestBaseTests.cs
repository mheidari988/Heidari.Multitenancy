using Heidari.Multitenancy.Application.Abstractions;
using Heidari.Multitenancy.Application.Attributes;
using Heidari.Multitenancy.Domain.Models;

namespace Heidari.Multitenancy.Tests.Application.Abstractions;

public class TenantRequestBaseTests
{
	[Fact]
	public void Ctor_Throws_WhenTenantIdIsDefault()
	{
		Assert.Throws<ArgumentException>(() => new SampleTenantRequest(default));
	}

	[Fact]
	public void Ctor_SetsTenantId()
	{
		var tenantId = new TenantId("tenant-123");

		var request = new SampleTenantRequest(tenantId);

		Assert.Equal(tenantId, request.TenantId);
	}

	[Fact]
	public void RequiresTenantAttribute_IsInherited()
	{
		var isDefined = Attribute.IsDefined(typeof(SampleTenantRequest), typeof(RequiresTenantAttribute), inherit: true);

		Assert.True(isDefined);
	}

	private sealed class SampleTenantRequest : TenantRequestBase<string>
	{
		public SampleTenantRequest(TenantId tenantId)
			: base(tenantId)
		{
		}
	}
}
