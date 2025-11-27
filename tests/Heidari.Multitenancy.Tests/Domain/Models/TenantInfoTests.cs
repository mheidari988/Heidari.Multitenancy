using Heidari.Multitenancy.Domain.Models;

namespace Heidari.Multitenancy.Tests.Domain.Models;

public class TenantInfoTests
{
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData(" ")]
	public void Ctor_Throws_WhenNameIsNullOrWhitespace(string? name)
	{
		var act = () => _ = new TenantInfo(new TenantId("id"), name!);

		Assert.Throws<ArgumentException>(act);
	}

	[Fact]
	public void Ctor_SetsProperties_WhenValid()
	{
		var id = new TenantId("id-1");
		var info = new TenantInfo(id, "Tenant One", "Server=db;Database=tenants;");

		Assert.Equal(id, info.Id);
		Assert.Equal("Tenant One", info.Name);
		Assert.Equal("Server=db;Database=tenants;", info.ConnectionString);
	}

	[Fact]
	public void ConnectionString_CanBeNull()
	{
		var info = new TenantInfo(new TenantId("id-2"), "Tenant Two");

		Assert.Null(info.ConnectionString);
	}
}
