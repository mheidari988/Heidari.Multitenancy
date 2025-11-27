# Overview

Heidari.Multitenancy provides lightweight, dependency-free abstractions to make tenant awareness a first-class concept in clean architecture solutions. The goal is to keep the domain layer pure while enabling host-specific integrations (ASP.NET Core middleware, EF Core configurations, background worker plumbing) via opt-in packages.

## Design principles
- Domain purity: no framework or infrastructure dependencies in the core abstractions.
- Strong typing: tenant identifiers use a dedicated value object to avoid accidental mix-ups.
- Composition over inheritance: interfaces first, optional base types when helpful.
- Opt-in integrations: framework-specific helpers belong in separate packages.
- Documentation and samples: clear guides and runnable examples to minimize adoption friction.

## What this repository provides today
- A domain package (`Heidari.Multitenancy.Domain`) with value objects, interfaces, and domain exceptions for tenant-aware models.
- XML-documented APIs to ease discovery in IDEs and generated references.

## Roadmap (high level)
- Application/infrastructure packages for common hosts (ASP.NET Core, EF Core, workers).
- Samples that showcase end-to-end tenant resolution and enforcement.
- Published API docs (DocFX) alongside conceptual guides.

For details on the current domain types, see [Tenant abstractions](concepts/tenant-abstractions.md). For usage walkthroughs, start with [Getting started](getting-started.md) and the [Tenant context propagation](guides/tenant-context-propagation.md) guide.
