using Heidari.Multitenancy.Domain.Models;

namespace Heidari.Multitenancy.Tests.Domain.Models;

public class TenantIdTests
{
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData("\t")]
	public void Ctor_Throws_WhenValueIsNullOrWhitespace(string? value)
	{
		Assert.Throws<ArgumentException>(() => new TenantId(value!));
	}

	[Fact]
	public void Ctor_SetsValue_WhenValid()
	{
		var id = new TenantId("tenant-123");

		Assert.Equal("tenant-123", id.Value);
	}

	[Fact]
	public void ToString_ReturnsValue()
	{
		var id = new TenantId("tenant-abc");

		Assert.Equal("tenant-abc", id.ToString());
	}

	[Fact]
	public void Equality_IsValueBased()
	{
		var left = new TenantId("same");
		var right = new TenantId("same");
		var different = new TenantId("other");

		Assert.Equal(left, right);
		Assert.NotEqual(left, different);
	}
}
