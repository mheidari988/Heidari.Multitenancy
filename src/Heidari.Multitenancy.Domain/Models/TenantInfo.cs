namespace Heidari.Multitenancy.Domain.Models;

/// <summary>
/// Describes a tenant using minimal metadata required at the domain layer.
/// </summary>
/// <remarks>
/// This type is intentionally simple to avoid coupling to infrastructure concerns.
/// </remarks>
public sealed class TenantInfo
{
	/// <summary>
	/// Gets the unique identifier of the tenant.
	/// </summary>
	public TenantId Id { get; }

	/// <summary>
	/// Gets the human-friendly name of the tenant.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Gets the optional connection string associated with the tenant, when known to the domain.
	/// </summary>
	public string? ConnectionString { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="TenantInfo"/> class.
	/// </summary>
	/// <param name="id">The tenant identifier.</param>
	/// <param name="name">The display name of the tenant.</param>
	/// <param name="connectionString">An optional connection string for tenant-specific data stores.</param>
	/// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null, empty, or whitespace.</exception>
	public TenantInfo(TenantId id, string name, string? connectionString = null)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Tenant name cannot be null or whitespace.", nameof(name));
		}

		Id = id;
		Name = name;
		ConnectionString = connectionString;
	}
}
