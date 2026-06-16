# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

GauTracker is an ASP.NET Core Web API on **.NET 10** (C#) backed by PostgreSQL via EF Core. It currently exposes one bounded service (`WebApi`) structured with Clean Architecture, plus a shared `Domain` utility library. The solution file is `GauTracker.slnx` (XML-based slnx format).

## Branching & Workflow

This repo follows **GitFlow** — see [CONTRIBUTING.md](CONTRIBUTING.md) for the full guide.

- **`main`** and **`develop`** are protected; **never commit to them directly.** All changes go through a pull request, and the `build` CI check must pass before merge.
- **`develop`** is the default branch and the everyday integration target.
- Branch off `develop` with `feature/*` for new work; PR back into `develop`.
- `release/*` (off `develop`) and `hotfix/*` (off `main`) merge into **both** `main` and `develop`.
- When doing work here, create an appropriately-named branch and open a PR — do not push to `main`/`develop`.

## Build & Run

All commands run from the solution root (`/GauTracker`).

```bash
dotnet build                                             # build whole solution
dotnet run --project Src/Services/WebApi/WebApi.Presentation   # run the API (OpenAPI at /openapi in Development)
docker compose up                                        # Postgres + API together (see compose.yaml)
```

- **Warnings are errors.** `Directory.Build.props` sets `TreatWarningsAsErrors`, `AnalysisMode=All`, and `EnforceCodeStyleInBuild` for every project. A build fails on any analyzer or code-style violation, not just compile errors — fix them rather than suppressing.
- Target framework, `ImplicitUsings`, and `Nullable` are set centrally in `Directory.Build.props`; do not re-declare them per-project.
- NuGet versions are centrally managed in `Directory.Packages.props` (`ManagePackageVersionsCentrally`). Reference packages with `<PackageReference Include="X" />` (no `Version`); add new versions to `Directory.Packages.props`.

### Tests
There is no test project yet (the `/Tests/` solution folder is empty). When adding one, place it under `Tests/` and register it in `GauTracker.slnx`.

### Pre-commit hooks
Commits are scanned for secrets via [pre-commit](https://pre-commit.com) (config in `.pre-commit-config.yaml`): a `gitleaks` secret scan plus `detect-private-key`, `check-added-large-files`, and `check-merge-conflict`. These hooks are intentionally non-mutating — formatting stays the job of `.editorconfig`/analyzers. Run `pre-commit install` once per clone; see `Docs/PreCommit.md` for the full reference.

## Database & Migrations

- Provider: Npgsql / PostgreSQL. Connection string key is `GauDB` (see `appsettings.json` / `ConnectionStrings__GauDB` env var).
- Default query behavior is **`NoTracking`** (`InfrastructureDI.cs`) — call `.AsTracking()` explicitly when you need change tracking.
- Application tables live in the `public` schema; the EF migrations history table lives in a separate `migrations` schema (`SchemaConstants`).
- **Migrations are applied automatically on startup** via `app.ApplyDatabaseMigrationsAsync()` in `Program.cs`. Migration files live in `WebApi.infrastructure/Data/Migrations/GauMigrations/`.
- The migrations assembly is `WebApi.infrastructure` and the startup project is `WebApi.Presentation`. See `Docs/EfMigrations.md` for the full `dotnet ef` command reference (requires `dotnet tool install --global dotnet-ef`).

## Architecture

Layered dependency flow (each references only the one below it):

```
WebApi.Presentation  →  WebApi.infrastructure  →  WebApi.Application  →  WebApi.Domain  →  Utilities/Domain
```

- **`Src/Utilities/Domain`** (namespace `Domain.*`) — solution-wide building blocks reused across services: `IEntity`/`IEntity<TKey>` contracts, the abstract `Entity<T>` base class (provides `Id` with a protected setter and `DateCreated`), and `GuardClauseExtensions` (e.g. `Guard.Against.ValidString(...)`, built on Ardalis.GuardClauses). This is intentionally separate from `WebApi.Domain`.
- **`WebApi.Domain`** — service-specific entities (`Card`, `User`). Entities inherit `Entity<long>`, use **private setters**, and are mutated only through **static `Create` factory methods and instance `Update` methods** that validate via guard clauses. Keep domain types persistence-ignorant: no EF/ORM **attributes** and no mapping config on the entity — all mapping lives in the `IEntityTypeConfiguration<T>`. This rule is about coupling to the ORM, **not** about banning provider value types: when a feature genuinely needs a database-native type (e.g. an `NpgsqlTsVector` property backing PostgreSQL full-text search), put the property on the entity and map it the idiomatic provider way (`HasGeneratedTsVectorColumn` in the config). **Prefer the idiomatic EF Core/Npgsql mapping over contortions** (shadow properties + `EF.Property<>(...)`, hand-written `HasComputedColumnSql`) whose only purpose is to keep such a type off the entity.
- **`WebApi.Application`** — application/use-case layer (currently a placeholder).
- **`WebApi.infrastructure`** (note the lowercase `infrastructure` in namespace and folder — match it) — `GauContext` (`DbContext`), EF entity configurations, and DI wiring. `AddDatabase(...)` in `InfrastructureDI.cs` registers the context. `EnableSensitiveDataLogging` is on only in Development/Staging.
- **`WebApi.Presentation`** — composition root. `Program.cs` is minimal; service registration is grouped into extension methods under `Common/DIExtensions/` (`AddApplicationServices`, `AddDatabase`, `ApplyDatabaseMigrationsAsync`). Add new wiring as extension methods there rather than inline in `Program.cs`.

### Conventions to follow when extending

- **New EF mapping:** add an `IEntityTypeConfiguration<T>` class under `WebApi.infrastructure/Configuration/`. `GauContext.OnModelCreating` calls `ApplyConfigurationsFromAssembly`, so configurations are discovered automatically — no `DbSet`/manual registration needed. Map tables via `ToTable(nameof(Entity), SchemaConstants.Default)`.
- **New entity:** inherit `Entity<TKey>`, keep setters private, expose `Create`/`Update` factory methods, and validate inputs with the shared guard-clause extensions.
- After model changes, add a migration (it will auto-apply on next startup).

## Docker

`compose.yaml` runs `postgres:17` (data persisted to the git-ignored `DockerData/Database/`) and the API. The API Dockerfile is at `Src/Services/WebApi/WebApi.Presentation/Dockerfile`. The Postgres credentials in `appsettings.json` and `compose.yaml` are local-dev defaults only — use real secrets/`.env` for any non-local environment.
