# Analysis: Embedded vs Portable Debug Symbols for QWIQ NuGet Packages

**Date**: 2025-12-14
**Analyst**: Claude Code (Analyst Agent)
**Status**: Complete

## Executive Summary

This analysis evaluates switching QWIQ from `DebugType=portable` with separate `.snupkg` symbol packages to `DebugType=embedded` symbols. Based on research of Microsoft guidance, popular library practices, and tradeoffs, **the recommendation is to keep the current `portable` + `snupkg` approach** for a public NuGet library like QWIQ.

## Current Configuration

From `Directory.Build.props`:

```xml
<!-- Source Link Configuration -->
<PropertyGroup>
  <PublishRepositoryUrl>true</PublishRepositoryUrl>
  <EmbedUntrackedSources>true</EmbedUntrackedSources>
  <IncludeSymbols>true</IncludeSymbols>
  <SymbolPackageFormat>snupkg</SymbolPackageFormat>
</PropertyGroup>

<PropertyGroup Condition=" '$(Configuration)' == 'Release' ">
  <DebugType>portable</DebugType>
  <!-- ... -->
</PropertyGroup>
```

---

## 1. Tradeoffs: Embedded vs Portable Symbols

### Size Impact

| Approach              | DLL Size | Package Count | Total Download                   |
| --------------------- | -------- | ------------- | -------------------------------- |
| **Portable + snupkg** | Base     | 2 packages    | Base (nupkg only for most users) |
| **Embedded**          | +20-30%  | 1 package     | Larger for ALL users             |

**Key finding**: Embedded PDBs increase package size by approximately 20-30% for SDK-style projects ([Microsoft Learn][ms-nuget], [Ken Muse][kenmuse]).

### Debugging Experience

| Scenario                | Portable + snupkg                                          | Embedded          |
| ----------------------- | ---------------------------------------------------------- | ----------------- |
| **Visual Studio**       | Requires symbol server config (one-time)                   | Works immediately |
| **JetBrains Rider**     | Works (ignores `CopyDebugSymbolFilesFromPackages`)         | Works             |
| **VS Code**             | Requires `CopyDebugSymbolFilesFromPackages=true` (.NET 7+) | Works             |
| **Source Link**         | Full support                                               | Full support      |
| **Corporate firewalls** | May block symbol server                                    | No issues         |

### Symbol Server Requirements

- **Portable**: Consumers add `https://symbols.nuget.org/download/symbols` to IDE symbol sources
- **Embedded**: No symbol server configuration needed - symbols travel with the assembly

---

## 2. Impact on Package Consumers

### Portable + snupkg (Current)

**Pros**:

- Smaller package for restore - only debuggers download symbols
- Pay-to-play model - most consumers never need debug symbols
- Standard approach for public libraries

**Cons**:

- Requires one-time IDE configuration for debugging
- Symbol server may be slow or blocked in some environments
- .NET 7+ requires `CopyDebugSymbolFilesFromPackages=true` if PDBs are in the nupkg (not applicable to snupkg)

### Embedded

**Pros**:

- Zero configuration debugging - "just works"
- No network dependency for symbols
- Single package to manage

**Cons**:

- ALL consumers pay the 20-30% size penalty, even if they never debug
- Slower restore times for large dependency graphs
- Exposes debug info in production deployments (minor security consideration)

### Consumer Experience Quote

> "Why make the restore slower and take more space on disk by downloading symbols for all packages when they will not likely be used? Why should a developer wait longer for restore when they might not even need to debug thru 3rd party code?" - [NuGet GitHub Wiki][nuget-wiki]

---

## 3. Build/Packaging Simplification

### With Portable + snupkg

- **Files produced**: `.nupkg` + `.snupkg`
- **CI steps**: Build, pack, push nupkg, push snupkg
- **NuGet.org**: Handles snupkg automatically when pushed alongside nupkg

### With Embedded

- **Files produced**: `.nupkg` only
- **CI steps**: Build, pack, push nupkg
- **Simplification**: One fewer file, one fewer push operation

### Size Comparison for QWIQ

Given QWIQ has 9 packable projects with multi-targeting (6 TFMs each):

| Metric        | Portable + snupkg       | Embedded              |
| ------------- | ----------------------- | --------------------- |
| Package files | 18 (9 nupkg + 9 snupkg) | 9 (nupkg only)        |
| Total size    | Base + symbols separate | Base + 20-30% per DLL |

---

## 4. Popular .NET Library Practices

### Survey Results

| Library                       | DebugType            | Symbol Distribution   | Notes                                                                              |
| ----------------------------- | -------------------- | --------------------- | ---------------------------------------------------------------------------------- |
| **dotnet/runtime**            | `portable`           | snupkg                | "Always pass portable to override arcade sdk which uses embedded for local builds" |
| **dotnet/aspnetcore**         | (not explicitly set) | `IncludeSymbols=true` | Uses symbol packages                                                               |
| **Serilog**                   | (default)            | `snupkg`              | `<SymbolPackageFormat>snupkg</SymbolPackageFormat>`                                |
| **AutoMapper**                | (default)            | `snupkg`              | `<SymbolPackageFormat>snupkg</SymbolPackageFormat>`                                |
| **Newtonsoft.Json**           | N/A                  | No symbols in package | [Issue #881][newtonsoft-issue] requesting PDBs                                     |
| **DotNet.ReproducibleBuilds** | `embedded` (default) | Embedded              | Recommends embedded for simplicity                                                 |

### Key Observation

**Microsoft's own libraries (runtime, ASP.NET Core) use `portable` with snupkg**, not embedded. This is the recommended approach for widely-used public NuGet packages to avoid impacting restore performance for all consumers.

---

## 5. Gotchas and Limitations

### Gotchas with Embedded Symbols

1. **Size penalty for everyone**: All package consumers download symbols, even if they never debug
2. **Disclosure concerns**: Debug info embedded in production binaries may expose source paths
3. **No stripping option**: Cannot remove symbols post-publish without rebuilding

### Gotchas with Portable + snupkg

1. **IDE configuration**: One-time setup required for debugging (add NuGet.org symbol server)
2. **Network dependency**: Symbol download requires internet access
3. **Corporate environments**: Symbol server may be blocked by firewalls
4. **.NET 7+ behavior change**: `CopyDebugSymbolFilesFromPackages` required for PDBs in nupkg (not snupkg)

### Native AOT Considerations

- **Embedded or local file required**: Native AOT cannot use symbol servers
- **QWIQ relevance**: Low - QWIQ is a library, not typically AOT-published

### GitHub Packages Limitation

- **GitHub Packages does not support snupkg**: If publishing to GitHub Packages, embedded is required
- **QWIQ relevance**: Publishing to NuGet.org, so not a concern

---

## 6. Recommendation

### Keep Current Configuration (Portable + snupkg)

**Rationale**:

1. **Follows Microsoft guidance**: Microsoft's own libraries use portable + snupkg for public packages
2. **Respects consumer bandwidth**: Only debuggers pay the symbol download cost
3. **Industry standard**: Serilog, AutoMapper, and other popular libraries use snupkg
4. **NuGet.org optimized**: Symbol server is tightly integrated with NuGet.org
5. **Professional approach**: Separating concerns (code vs. debug info) is cleaner

### When to Consider Embedded

Switch to embedded only if:

- Publishing to GitHub Packages (no snupkg support)
- Targeting corporate environments with strict firewall rules
- Building internal tools where simplicity trumps download size
- Package is rarely consumed as a dependency (low restore frequency)

### Configuration to Keep

```xml
<!-- In Directory.Build.props -->
<PropertyGroup>
  <PublishRepositoryUrl>true</PublishRepositoryUrl>
  <EmbedUntrackedSources>true</EmbedUntrackedSources>
  <IncludeSymbols>true</IncludeSymbols>
  <SymbolPackageFormat>snupkg</SymbolPackageFormat>
</PropertyGroup>

<PropertyGroup Condition=" '$(Configuration)' == 'Release' ">
  <DebugType>portable</DebugType>
</PropertyGroup>
```

---

## Sources

- [Microsoft Learn: NuGet and .NET libraries][ms-nuget]
- [Microsoft Learn: Symbol packages (.snupkg)][ms-snupkg]
- [NuGet Package Debugging & Symbols Improvements][nuget-wiki]
- [dotnet/sdk Issue #2679: Discussion on embedded default][sdk-2679]
- [DotNet.ReproducibleBuilds README][repro-builds]
- [Ken Muse: What Every Developer Should Know About PDBs][kenmuse]
- [dotnet/runtime Directory.Build.props][runtime-props]
- [Serilog Directory.Build.props][serilog-props]
- [AutoMapper Issue #3342: snupkg symbols][automapper-issue]
- [Newtonsoft.Json Issue #881: PDBs][newtonsoft-issue]

[ms-nuget]: https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/nuget
[ms-snupkg]: https://learn.microsoft.com/en-us/nuget/create-packages/symbol-packages-snupkg
[nuget-wiki]: https://github.com/NuGet/Home/wiki/NuGet-Package-Debugging-&-Symbols-Improvements
[sdk-2679]: https://github.com/dotnet/sdk/issues/2679
[repro-builds]: https://github.com/dotnet/reproducible-builds/blob/main/README.md
[kenmuse]: https://www.kenmuse.com/blog/what-every-developer-should-know-about-pdbs/
[runtime-props]: https://github.com/dotnet/runtime/blob/main/Directory.Build.props
[serilog-props]: https://github.com/serilog/serilog/blob/dev/Directory.Build.props
[automapper-issue]: https://github.com/AutoMapper/AutoMapper/issues/3342
[newtonsoft-issue]: https://github.com/JamesNK/Newtonsoft.Json/issues/881

---

## Handoff

**Next Agent**: architect (if design decision escalation needed) or implementer (if proceeding with current config)

**Action Items**:

- None required - current configuration is optimal for NuGet.org publishing
- Document this decision in an ADR if formal record desired
