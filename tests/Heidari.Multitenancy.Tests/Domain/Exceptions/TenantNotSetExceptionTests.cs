using Heidari.Multitenancy.Domain.Exceptions;

namespace Heidari.Multitenancy.Tests.Domain.Exceptions;

public class TenantNotSetExceptionTests
{
	[Fact]
	public void DefaultCtor_SetsDefaultMessage()
	{
		var exception = new TenantNotSetException();

		Assert.Equal("Tenant has not been set for the current context.", exception.Message);
	}

	[Fact]
	public void Ctor_SetsCustomMessage()
	{
		var exception = new TenantNotSetException("custom");

		Assert.Equal("custom", exception.Message);
	}

	[Fact]
	public void Ctor_SetsInnerException()
	{
		var inner = new InvalidOperationException("inner");

		var exception = new TenantNotSetException("wrapped", inner);

		Assert.Equal(inner, exception.InnerException);
		Assert.Equal("wrapped", exception.Message);
	}
}
