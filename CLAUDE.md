# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A mono repository of ~80 NuGet packages, each providing `IHealthCheck` implementations for
`Microsoft.Extensions.Diagnostics.HealthChecks`, targeting one external service/client library
(SQL Server, Redis, Kafka, Azure Blobs, AWS S3, etc.). Every package is configurable via code
(`Action<TOptions>`) and/or `appsettings.json` (`HealthChecks:<ServiceName>:<checkName>`).

Read `AGENTS.md` for the authoritative AI-assistant instructions (`.github/copilot-instructions.md`
just redirects to it). This repo previously carried a `decisions/` folder of architecture-decision
records and a `templates/adr.md` template, copied wholesale from an unrelated template project;
their content described a fictional "Spix Spreed" product with conventions that never applied here
(e.g. a "SpixSpreed." project-name prefix). That content was hallucinated, so the folder, template,
and the "Decision References" instructions pointing at them were removed entirely. If you encounter
any reference to "Spix Spreed" or a "SpixSpreed." prefix elsewhere, treat it as leftover noise, not
as a real convention — the actual conventions are whatever is reflected in this repo's code
(namespace root is `NetEvolve.HealthChecks`, no prefix is applied).

## Commands

Everything runs from the repo root against `HealthChecks.slnx`.

```powershell
dotnet restore HealthChecks.slnx
dotnet build HealthChecks.slnx
csharpier format .                 # required formatting; CI checks it
```

### Tests

Test runner is **Microsoft.Testing.Platform** (`global.json` pins `"runner": "Microsoft.Testing.Platform"`)
using **TUnit**, not xUnit/NUnit/MSTest. There are only three test projects for the whole repo
(not one per package):

- `tests/NetEvolve.HealthChecks.Tests.Unit` — one subfolder per service, mirrors `src/`
- `tests/NetEvolve.HealthChecks.Tests.Integration` — same layout, uses Testcontainers to spin up
  real service instances (e.g. `Testcontainers.MsSql`) — requires a running container runtime
  (see below)
- `tests/NetEvolve.HealthChecks.Tests.Architecture` — ArchUnitNET rules enforced across all
  package assemblies (naming, layering, etc.)

Run/build a single test project directly rather than the whole solution when iterating:

```powershell
dotnet test tests/NetEvolve.HealthChecks.Tests.Unit/NetEvolve.HealthChecks.Tests.Unit.csproj -f net10.0
```

Per repo memory, default to a **single TFM** (`net10.0`) unless the change is TFM-specific —
`Directory.Build.props` restricts `_TestTargetFrameworks` to `net10.0` automatically when
`BuildingInsideVisualStudio` is true; pass `-f net10.0` explicitly from the CLI to get the same effect.

Filtering: this runner uses TUnit's `--treenode-filter` (not `--filter`), e.g.
`--treenode-filter "/*/*/*SqlServer*/*"`. Tests are also grouped with a `[TestGroup(nameof(X))]`
attribute per service folder (from `NetEvolve.Extensions.TUnit`), matching CI's `.testgroup` files
(`scripts/Collect-TestProjects.ps1` collects these to build the CI test matrix).

Integration tests need Docker/a container runtime. `scripts/Run-LinuxTests.ps1` is a local-only
convenience that runs a test project inside a persistent Linux container — useful on Windows when
a non-ASCII Windows username breaks certain clients (e.g. RocketMQ), and to reproduce CI's Linux
environment. It does not affect CI itself.

### Docker / containers

If Docker connectivity fails locally, the runtime here is **Rancher Desktop**, not Docker Desktop —
restart that instead.

## Architecture

### Anatomy of a package (`src/NetEvolve.HealthChecks.<Service>/`)

Each package follows the same shape (see `src/NetEvolve.HealthChecks.SqlServer` as the canonical
example):

- **`<Service>Options.cs`** — plain options class (connection string/timeout/etc.).
- **`<Service>Configure.cs`** — `internal sealed class` implementing
  `IConfigureNamedOptions<TOptions>`, `IPostConfigureOptions<TOptions>`, and
  `IValidateOptions<TOptions>`. `Configure` binds from `IConfiguration` at
  `HealthChecks:<Service>:<name>`; `PostConfigure` fills in defaults; `Validate` returns
  `ValidateOptionsResult.Fail/Success`.
- **`<Service>HealthCheck.cs`** — the actual `IHealthCheck`, resolves its named `TOptions` via
  `IOptionsMonitor<TOptions>.Get(name)` and performs the real check.
- **`DependencyInjectionExtensions.cs`** — public `static partial class` with the `Add<Service>`
  extension method on `IHealthChecksBuilder`, annotated `[HealthCheckHelper]`. This attribute
  triggers `SourceGenerator.HealthChecks` (Roslyn incremental generator, see
  `src/SourceGenerator.HealthChecks/Generators/`) to generate the boilerplate `partial` members —
  notably `IsServiceTypeRegistered<TMarker>()` and `ThrowIfNameIsAlreadyUsed<THealthCheck>(name)` —
  and a private nested `<Service>CheckMarker` partial class used to guard against registering the
  shared singleton services (`ConfigureOptions<TConfigure>`, the health check itself) more than
  once per `IHealthChecksBuilder`.
- A generated `README.md` (do not hand-edit; see `scripts/Update-Readme.ps1` and the
  `nuget-package-readme-template` decision) and `.sarif` diagnostic files (build artifacts, ignore).

`src/SourceGenerator.Attributes` defines the marker attributes (`HealthCheckHelperAttribute`,
`ConfigurableHealthCheckAttribute`, `GenerateSqlHealthCheckAttribute`) consumed by
`src/SourceGenerator.HealthChecks`. When adding a new health check package, copy the shape of an
existing sibling package rather than writing the DI boilerplate by hand.

### Package naming

`NetEvolve.HealthChecks.<ServiceGroup?>.<ServiceName>.<ServiceVersion?>` — `ServiceGroup` groups
related cloud/platform services (`Apache`, `AWS`, `Azure`, `GCP`); `ServiceVersion` distinguishes
alternate or legacy client libraries for the same service (e.g. `SqlServer` vs `SqlServer.Legacy`,
`MySql` vs `MySql.Connector`). See the README's "Package naming explanation" section for the full
rationale before adding a new package or client-library variant.

### Central version/config management

- All package versions are pinned centrally in `Directory.Packages.props` — project files
  reference packages without a `<PackageVersion>`; add new dependencies there, not inline.
- `Directory.Build.props` / `Directory.Build.targets` / `.editorconfig` are shared, repo-wide
  config — do not change them unless explicitly asked to.
- Target frameworks: `net8.0`, `net9.0`, `net10.0` (see README's supported-.NET-version table);
  .NET Standard and pre-.NET-8 are not supported.

## Conventions worth knowing (beyond global git/commit rules)

- No `#region`/`#endregion` anywhere in `.cs`/`.razor` files — split large files into partial
  classes instead.
- Use `DateTimeOffset`, not `DateTime`, for all date/time values; obtain current time via an
  injected `TimeProvider` (never `DateTime.Now`/`UtcNow` directly). Tests use
  `Microsoft.Extensions.Time.Testing.FakeTimeProvider`.
- Conventional Commits are enforced repo-wide (see `CONTRIBUTING.md` and the global git rules) —
  GitVersion derives version bumps from commit type/`!`/`BREAKING CHANGE:`.
