using Heidari.Multitenancy.Domain.Entities;
using Heidari.Multitenancy.Domain.Models;

namespace Heidari.Multitenancy.Tests.Domain.Entities;

public class TenantEntityBaseTests
{
	[Fact]
	public void Ctor_Throws_WhenTenantIdIsDefault()
	{
		var act = () => _ = new TestTenantEntity(default);

		Assert.Throws<ArgumentException>(act);
	}

	[Fact]
	public void Ctor_SetsTenantId_WhenValid()
	{
		var tenantId = new TenantId("tenant-123");
		var entity = new TestTenantEntity(tenantId);

		Assert.Equal(tenantId, entity.TenantId);
	}

	private sealed class TestTenantEntity : TenantEntityBase
	{
		public TestTenantEntity(TenantId tenantId) : base(tenantId)
		{
		}
	}
}
