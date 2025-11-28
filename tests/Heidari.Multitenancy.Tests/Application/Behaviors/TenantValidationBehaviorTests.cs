using Heidari.Multitenancy.Application.Behaviors;
using Heidari.Multitenancy.Domain.Exceptions;
using Heidari.Multitenancy.Domain.Models;
using Heidari.Multitenancy.Tests.Application.TestDoubles;
using MediatR;

namespace Heidari.Multitenancy.Tests.Application.Behaviors;

public class TenantValidationBehaviorTests
{
	[Fact]
	public async Task Handle_Throws_WhenTenantMismatch()
	{
		var currentTenant = new FakeCurrentTenant(hasTenant: true, tenantId: new TenantId("tenant-a"));
		var behavior = new TenantValidationBehavior<TenantRequest, string>(currentTenant);
		var request = new TenantRequest(new TenantId("tenant-b"));

		var act = () => behavior.Handle(request, () => Task.FromResult("ok"), CancellationToken.None);

		await Assert.ThrowsAsync<TenantAccessException>(act);
	}

	[Fact]
	public async Task Handle_Allows_WhenTenantMatches()
	{
		var tenantId = new TenantId("tenant-1");
		var currentTenant = new FakeCurrentTenant(hasTenant: true, tenantId: tenantId);
		var behavior = new TenantValidationBehavior<TenantRequest, string>(currentTenant);
		var request = new TenantRequest(tenantId);
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
	public async Task Handle_Skips_WhenTenantNotSet()
	{
		var currentTenant = new FakeCurrentTenant();
		var behavior = new TenantValidationBehavior<TenantRequest, string>(currentTenant);
		var request = new TenantRequest(new TenantId("tenant-1"));

		var result = await behavior.Handle(request, () => Task.FromResult("ok"), CancellationToken.None);

		Assert.Equal("ok", result);
	}

	[Fact]
	public async Task Handle_Skips_WhenRequestHasNoTenantId()
	{
		var currentTenant = new FakeCurrentTenant(hasTenant: true, tenantId: new TenantId("tenant-1"));
		var behavior = new TenantValidationBehavior<NoTenantRequest, string>(currentTenant);
		var request = new NoTenantRequest();

		var result = await behavior.Handle(request, () => Task.FromResult("ok"), CancellationToken.None);

		Assert.Equal("ok", result);
	}

	[Fact]
	public async Task Handle_Allows_WhenNullableTenantIdMatches()
	{
		var tenantId = new TenantId("tenant-1");
		var currentTenant = new FakeCurrentTenant(hasTenant: true, tenantId: tenantId);
		var behavior = new TenantValidationBehavior<NullableTenantRequest, string>(currentTenant);
		var request = new NullableTenantRequest(tenantId);
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
	public async Task Handle_Skips_WhenNullableTenantIdIsNull()
	{
		var currentTenant = new FakeCurrentTenant(hasTenant: true, tenantId: new TenantId("tenant-1"));
		var behavior = new TenantValidationBehavior<NullableTenantRequest, string>(currentTenant);
		var request = new NullableTenantRequest(null);

		var result = await behavior.Handle(request, () => Task.FromResult("ok"), CancellationToken.None);

		Assert.Equal("ok", result);
	}

	private sealed class TenantRequest : IRequest<string>
	{
		public TenantRequest(TenantId tenantId)
		{
			TenantId = tenantId;
		}

		public TenantId TenantId { get; }
	}

	private sealed class NullableTenantRequest : IRequest<string>
	{
		public NullableTenantRequest(TenantId? tenantId)
		{
			TenantId = tenantId;
		}

		public TenantId? TenantId { get; }
	}

	private sealed class NoTenantRequest : IRequest<string>
	{
	}
}
