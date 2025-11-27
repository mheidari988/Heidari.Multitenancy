using Heidari.Multitenancy.Domain.Exceptions;
using Heidari.Multitenancy.Domain.Models;

namespace Heidari.Multitenancy.Tests.Domain.Exceptions;

public class TenantAccessExceptionTests
{
	[Fact]
	public void DefaultCtor_SetsMessageAndProperties()
	{
		var expected = new TenantId("expected");
		var actual = new TenantId("actual");

		var exception = new TenantAccessException(expected, actual);

		Assert.Equal(expected, exception.ExpectedTenantId);
		Assert.Equal(actual, exception.ActualTenantId);
		Assert.Equal("Tenant mismatch. Expected 'expected', actual 'actual'.", exception.Message);
	}

	[Fact]
	public void Ctor_SetsCustomMessage()
	{
		var exception = new TenantAccessException("custom", new TenantId("expected"), new TenantId("actual"));

		Assert.Equal("custom", exception.Message);
	}

	[Fact]
	public void Ctor_SetsInnerException()
	{
		var inner = new InvalidOperationException("inner");
		var exception = new TenantAccessException("wrapped", inner, new TenantId("expected"), new TenantId("actual"));

		Assert.Equal(inner, exception.InnerException);
		Assert.Equal("wrapped", exception.Message);
	}
}
