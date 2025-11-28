namespace Heidari.Multitenancy.Application.Attributes;

/// <summary>
/// Marks a request as requiring a valid tenant context.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
public sealed class RequiresTenantAttribute : Attribute
{
}
