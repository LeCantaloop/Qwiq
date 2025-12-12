# QWIQ Build Reference Documentation

## Project Layout

### Source Projects (`src/`)

| Project              | Target Frameworks            | Description                         |
| -------------------- | ---------------------------- | ----------------------------------- |
| `Qwiq.Core`          | net472;netstandard2.0;net8.0 | Core interfaces and abstractions    |
| `Qwiq.Core.Rest`     | net472;netstandard2.0;net8.0 | REST API client implementation      |
| `Qwiq.Core.Soap`     | net472                       | SOAP client (Windows only)          |
| `Qwiq.Linq`          | net472;net8.0                | LINQ query provider                 |
| `Qwiq.Mapper`        | net472;net8.0                | Object mapping layer                |
| `Qwiq.Identity`      | net472;net8.0                | Identity management                 |
| `Qwiq.Identity.Soap` | net472                       | Identity SOAP client (Windows only) |

### Test Projects (`test/`)

| Directory                | Project Name              | Target Frameworks | Description              |
| ------------------------ | ------------------------- | ----------------- | ------------------------ |
| `Qwiq.Core.Tests`        | `Qwiq.Core.UnitTests`     | net472;net8.0     | Core unit tests          |
| `Qwiq.Linq.Tests`        | `Qwiq.Linq.UnitTests`     | net472;net8.0     | LINQ provider tests      |
| `Qwiq.Mapper.Tests`      | `Qwiq.Mapper.UnitTests`   | net472;net8.0     | Mapper tests             |
| `Qwiq.Identity.Tests`    | `Qwiq.Identity.UnitTests` | net472;net8.0     | Identity tests           |
| `Qwiq.Integration.Tests` | `Qwiq.IntegrationTests`   | net472            | Full integration tests   |
| `Qwiq.Mocks`             | `Qwiq.Mocks`              | net472;net8.0     | Mock implementations     |
| `Qwiq.Tests.Common`      | `Qwiq.Tests.Common`       | net472;net8.0     | Shared test utilities    |
| `Qwiq.Package.Tests`     | `Qwiq.Package.Tests`      | net8.0            | NuGet package validation |

## InternalsVisibleTo Configuration

| Source Project                | Visible To                                    |
| ----------------------------- | --------------------------------------------- |
| `Qwiq.Core.csproj`            | `Qwiq.Core.UnitTests`, `Qwiq.Mocks`           |
| `Qwiq.Client.Rest.csproj`     | `Qwiq.IntegrationTests`                       |
| `Qwiq.Client.Soap.csproj`     | `Qwiq.Identity.Soap`, `Qwiq.IntegrationTests` |
| `Qwiq.Identity.Soap.csproj`   | `Qwiq.IntegrationTests`                       |
| `Qwiq.Mapper.Identity.csproj` | `Qwiq.Identity.UnitTests`                     |

## SDK and Tool Versions

| Tool                   | Version      | Configuration               |
| ---------------------- | ------------ | --------------------------- |
| .NET SDK               | 8.0.100      | `global.json`               |
| Nerdbank.GitVersioning | See manifest | `.config/dotnet-tools.json` |

## MSBuild Properties

### PedanticMode

Controls `TreatWarningsAsErrors`:

- `true` (default on CI): Warnings are errors
- `false`: Warnings allowed (for diagnosing)

```powershell
dotnet build /p:PedanticMode=false
```

### ContinuousIntegrationBuild

Set automatically by CI. Enables:

- Deterministic builds
- PedanticMode=true by default

## Package Metadata

Defined in `Directory.Build.props`:

- Authors
- Copyright
- License
- Repository URL
- Package icon

Project-specific metadata in individual `.csproj`:

- Description
- PackageId

## Artifacts Output

With `ArtifactsPath` enabled:

- Packages: `artifacts/package/{Configuration}/`
- Logs: `artifacts/logs/{Configuration}/`
- Test results: `artifacts/TestResults/`

Query actual path:

```powershell
dotnet msbuild -getProperty:PackageOutputPath
```
