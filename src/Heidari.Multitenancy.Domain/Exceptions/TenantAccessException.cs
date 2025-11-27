using Heidari.Multitenancy.Domain.Models;

namespace Heidari.Multitenancy.Domain.Exceptions;

/// <summary>
/// Exception thrown when an operation is attempted under a tenant different from the expected tenant.
/// </summary>
public class TenantAccessException : InvalidOperationException
{
	/// <summary>
	/// Gets the tenant identifier that was expected for the operation.
	/// </summary>
	public TenantId ExpectedTenantId { get; }

	/// <summary>
	/// Gets the tenant identifier that was actually associated with the operation.
	/// </summary>
	public TenantId ActualTenantId { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="TenantAccessException"/> class.
	/// </summary>
	/// <param name="expectedTenantId">The tenant identifier the operation requires.</param>
	/// <param name="actualTenantId">The tenant identifier supplied or detected during the operation.</param>
	public TenantAccessException(TenantId expectedTenantId, TenantId actualTenantId)
		: base($"Tenant mismatch. Expected '{expectedTenantId}', actual '{actualTenantId}'.")
	{
		ExpectedTenantId = expectedTenantId;
		ActualTenantId = actualTenantId;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TenantAccessException"/> class with a custom message.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="expectedTenantId">The tenant identifier the operation requires.</param>
	/// <param name="actualTenantId">The tenant identifier supplied or detected during the operation.</param>
	public TenantAccessException(string message, TenantId expectedTenantId, TenantId actualTenantId)
		: base(message)
	{
		ExpectedTenantId = expectedTenantId;
		ActualTenantId = actualTenantId;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TenantAccessException"/> class with a custom message and an inner exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="innerException">The inner exception that caused this exception.</param>
	/// <param name="expectedTenantId">The tenant identifier the operation requires.</param>
	/// <param name="actualTenantId">The tenant identifier supplied or detected during the operation.</param>
	public TenantAccessException(string message, Exception innerException, TenantId expectedTenantId, TenantId actualTenantId)
		: base(message, innerException)
	{
		ExpectedTenantId = expectedTenantId;
		ActualTenantId = actualTenantId;
	}
}
