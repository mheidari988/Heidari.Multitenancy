namespace Heidari.Multitenancy.Domain.Models;

/// <summary>
/// Strongly-typed identifier representing a tenant across the system.
/// </summary>
/// <remarks>
/// Immutable, lightweight wrapper around a non-empty string to avoid leaking raw identifiers.
/// </remarks>
public readonly record struct TenantId
{
	/// <summary>
	/// Gets the underlying tenant identifier value.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="TenantId"/> struct.
	/// </summary>
	/// <param name="value">The tenant identifier string.</param>
	/// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null, empty, or whitespace.</exception>
	public TenantId(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ArgumentException("Tenant id cannot be null or whitespace.", nameof(value));
		}

		Value = value;
	}

	/// <summary>
	/// Returns the string representation of the tenant identifier.
	/// </summary>
	/// <returns>The tenant identifier value.</returns>
	public override string ToString() => Value;
}
