# AGENTS — Contributor Guide for Automated and Human Collaborators

This file describes how to work in this repository with consistency, quality, and safety. Follow these practices when making changes or reviewing PRs.

## Principles
- Keep the domain layer pure: no framework or infrastructure dependencies in `src/Heidari.Multitenancy.Domain`.
- Keep the application layer framework-agnostic: MediatR behaviors and interfaces must not depend on ASP.NET/EF or host details.
- Favor small, focused changes with clear intent and tests.
- Preserve existing user changes; do not revert unrelated work.
- Prefer composition over inheritance; keep abstractions minimal and framework-agnostic.
- Documentation is first-class: update docs and samples alongside code changes.

## Coding guidelines
- Target .NET 10; respect nullable and implicit usings settings already configured.
- Domain rules belong in the domain package; framework-specific helpers go into dedicated packages (Application/Infrastructure/Web).
- Application package: keep MediatR behaviors generic; do not add host-specific dependencies. `ICurrentTenant`, `ITenantProvider`, `ICacheProvider`, and `ICacheable` remain pure abstractions.
- No EF Core/ASP.NET attributes or external dependencies in the domain project.
- Use strongly typed IDs where provided (`TenantId`); avoid leaking raw strings/Guids for tenant identity.
- Keep XML docs up to date for all public surface areas.

## Testing
- Add or update unit tests for all new behavior. Tests live under `tests/Heidari.Multitenancy.Tests`.
- Prefer xUnit `Fact`/`Theory` with clear AAA pattern and minimal test doubles.
- Run `dotnet test` before submitting or publishing changes.
- When adding pipeline behaviors (enforcement, validation, cache), include positive/negative coverage for required attributes, tenant mismatch, cache bypass/hit/miss, and nullable tenant scenarios.

## CI/CD
- GitHub Actions workflow publishes to NuGet only on tags (`v*`) and only after build + test succeed.
- Do not bypass tests in the pipeline; keep the publish gate intact.

## Documentation
- Root `README.md` and `docs/` must stay in sync with code changes.
- Add guides/samples when introducing new concepts or integration patterns.

## Change hygiene
- Use meaningful commit messages; avoid amending others’ commits.
- Do not use destructive git commands on user changes.
- Keep changes ASCII unless file already uses Unicode and there is clear justification.

## When in doubt
- Ask questions about intent before broad refactors.
- Default to conservative changes that preserve backward compatibility unless explicitly directed otherwise.
