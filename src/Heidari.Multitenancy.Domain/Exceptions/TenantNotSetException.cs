namespace Heidari.Multitenancy.Domain.Exceptions;

/// <summary>
/// Exception thrown when a tenant-aware operation is attempted without a tenant present in the context.
/// </summary>
public class TenantNotSetException : InvalidOperationException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TenantNotSetException"/> class.
	/// </summary>
	public TenantNotSetException()
		: base("Tenant has not been set for the current context.")
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TenantNotSetException"/> class with a custom message.
	/// </summary>
	/// <param name="message">The error message.</param>
	public TenantNotSetException(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TenantNotSetException"/> class with a custom message and an inner exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="innerException">The inner exception that caused this exception.</param>
	public TenantNotSetException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
