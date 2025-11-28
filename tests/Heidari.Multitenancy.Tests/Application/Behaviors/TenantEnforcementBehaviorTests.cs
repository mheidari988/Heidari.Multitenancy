using Heidari.Multitenancy.Application.Attributes;
using Heidari.Multitenancy.Application.Behaviors;
using Heidari.Multitenancy.Domain.Exceptions;
using Heidari.Multitenancy.Domain.Models;
using Heidari.Multitenancy.Tests.Application.TestDoubles;
using MediatR;

namespace Heidari.Multitenancy.Tests.Application.Behaviors;

public class TenantEnforcementBehaviorTests
{
	[Fact]
	public async Task Handle_Throws_WhenTenantRequiredButNotSet()
	{
		var currentTenant = new FakeCurrentTenant();
		var behavior = new TenantEnforcementBehavior<TenantRequiredRequest, string>(currentTenant);
		var request = new TenantRequiredRequest();

		var act = () => behavior.Handle(request, () => Task.FromResult("ok"), CancellationToken.None);

		await Assert.ThrowsAsync<TenantNotSetException>(act);
	}

	[Fact]
	public async Task Handle_InvokesNext_WhenTenantPresent()
	{
		var currentTenant = new FakeCurrentTenant(hasTenant: true, tenantId: new TenantId("tenant-1"));
		var behavior = new TenantEnforcementBehavior<TenantRequiredRequest, string>(currentTenant);
		var request = new TenantRequiredRequest();
		var nextCalled = false;

		var result = await behavior.Handle(request, () =>
		{
			nextCalled = true;
			return Task.FromResult("ok");
		}, CancellationToken.None);

		Assert.True(nextCalled);
		Assert.Equal("ok", result);
	}

	[Fact]
	public async Task Handle_Skips_WhenTenantNotRequired()
	{
		var currentTenant = new FakeCurrentTenant();
		var behavior = new TenantEnforcementBehavior<NonTenantRequest, string>(currentTenant);
		var request = new NonTenantRequest();

		var result = await behavior.Handle(request, () => Task.FromResult("ok"), CancellationToken.None);

		Assert.Equal("ok", result);
	}

	[RequiresTenant]
	private sealed class TenantRequiredRequest : IRequest<string>
	{
	}

	private sealed class NonTenantRequest : IRequest<string>
	{
	}
}
