# ADR-011: Portable Debug Symbols with Symbol Packages

**Status**: Superseded by ADR-012
**Date**: 2025-12-14
**Superseded Date**: 2025-12-14
**Deciders**: Architecture Team
**Context**: v11.0.0 NuGet Release Preparation
**Superseded By**: [ADR-012: Embedded Debug Symbols](ADR-012-embedded-symbols.md)

> **⚠️ This ADR has been superseded by [ADR-012: Embedded Debug Symbols](./ADR-012-embedded-symbols.md)**
>
> After initial analysis recommended portable symbols, a multi-agent consensus review reconsidered this decision based on QWIQ's specific scale, audience, and maintainer constraints. ADR-012 documents the final decision to use embedded symbols.
>
> This document is preserved for historical context and to understand the analysis that led to the embedded symbols decision.

## Context and Problem Statement

QWIQ is preparing for its v11.0.0 NuGet release. The project uses `DotNet.ReproducibleBuilds` for deterministic builds, which defaults to `embedded` debug symbols. We need to decide whether to accept this default or override to `portable` symbols with separate `.snupkg` symbol packages.

**Key Questions**:

- Should debug symbols be embedded in assemblies or distributed separately?
- What impact does this have on package consumers?
- How does this align with industry practices for public NuGet libraries?

## Decision Drivers

### Technical Factors

1. **Package Size Impact**:

   - Embedded PDBs increase assembly size by approximately 20-30%
   - QWIQ has 10 packable projects with 6 target frameworks each
   - Size penalty applies to ALL consumers, not just debuggers

2. **Distribution Model**:

   - NuGet.org has a dedicated symbol server (`symbols.nuget.org`)
   - Symbol packages (`.snupkg`) are indexed automatically when pushed alongside `.nupkg`
   - Pay-to-play model: only developers who debug download symbols

3. **DotNet.ReproducibleBuilds Default**:

   - Defaults to `DebugType=embedded` for simplicity
   - Must be explicitly overridden for portable + snupkg approach

4. **Debugging Experience**:
   - Embedded: Zero configuration - symbols travel with assemblies
   - Portable: One-time IDE configuration to enable NuGet.org symbol server

### Industry Practices

Survey of popular .NET libraries:

| Library               | DebugType  | Symbol Distribution | Notes                                |
| --------------------- | ---------- | ------------------- | ------------------------------------ |
| **dotnet/runtime**    | `portable` | snupkg              | Microsoft's own base class libraries |
| **dotnet/aspnetcore** | (default)  | `IncludeSymbols`    | Uses symbol packages                 |
| **Serilog**           | (default)  | snupkg              | Popular logging library              |
| **AutoMapper**        | (default)  | snupkg              | Popular mapping library              |
| **Newtonsoft.Json**   | N/A        | No symbols          | Users have requested PDBs            |

**Key observation**: Microsoft's own libraries use `portable` + snupkg for public packages.

## Considered Options

### Option 1: Portable Symbols with snupkg (Recommended)

**Configuration**:

```xml
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

**Pros**:

- Smaller package size for all consumers
- Pay-to-play model respects consumer bandwidth
- Follows Microsoft guidance for public libraries
- Industry standard approach (Serilog, AutoMapper, etc.)
- NuGet.org symbol server is optimized for this workflow

**Cons**:

- Consumers must configure symbol server in IDE (one-time)
- Corporate firewalls may block symbol downloads
- Two packages to publish instead of one
- Must maintain `DebugType` override in `Directory.Build.props`
- CI must publish both `.nupkg` and `.snupkg`

### Option 2: Embedded Symbols

**Configuration**:

```xml
<PropertyGroup>
  <PublishRepositoryUrl>true</PublishRepositoryUrl>
  <EmbedUntrackedSources>true</EmbedUntrackedSources>
  <DebugType>embedded</DebugType>
</PropertyGroup>
```

**Pros**:

- Zero configuration debugging - "just works"
- No network dependency for symbols
- Single package to manage
- Simpler CI pipeline (one package type)
- Works in corporate environments with symbol server blocks

**Cons**:

- ALL consumers pay 20-30% size penalty, even if they never debug
- Slower restore times for large dependency graphs
- Contradicts Microsoft guidance for public libraries
- Exposes debug info in production deployments

## Decision

Adopt Option 1: Portable Symbols with snupkg.

### Rationale

1. **Microsoft Guidance**: Microsoft's own `dotnet/runtime` explicitly uses `portable` to "override arcade sdk which uses embedded for local builds" - they deliberately choose portable for public distribution.

2. **Bandwidth Respect**: Most QWIQ consumers will never debug into QWIQ source code. Making everyone download embedded symbols violates the principle of not wasting consumer resources.

3. **Industry Standard**: Popular libraries (Serilog, AutoMapper) use snupkg. Developers expect this pattern and have symbol servers configured.

4. **NuGet.org Optimization**: The NuGet.org symbol server is specifically designed for this workflow, with tight integration into `dotnet nuget push`.

5. **Professionalism**: Separating concerns (code distribution vs. debug info) is the more architecturally sound approach for a public library.

## Consequences

### Headaches We Accept

1. **Consumer IDE Configuration**:

   - Visual Studio: Tools > Options > Debugging > Symbols > Add `https://symbols.nuget.org/download/symbols`
   - JetBrains Rider: Works automatically
   - VS Code: Requires `CopyDebugSymbolFilesFromPackages=true` for .NET 7+
   - **Mitigation**: Document in README; one-time setup

2. **Corporate Firewall Blocks**:

   - Some enterprises block external symbol servers
   - **Mitigation**: Document workarounds in README - Source Link for source browsing, build from source with embedded symbols, or request IT whitelist `symbols.nuget.org`

3. **Dual Package Publishing**:

   - CI must push both `.nupkg` and `.snupkg`
   - **Mitigation**: Explicit dual-push in `release.yml` with `--skip-duplicate` for idempotency

4. **Configuration Override Maintenance**:

   - Must maintain `<DebugType>portable</DebugType>` override in `Directory.Build.props`
   - **Mitigation**: Document in ADR; unlikely to conflict with future changes

5. **Symbol Server Availability**:
   - NuGet.org symbol server downtime affects debugging
   - **Mitigation**: Source Link provides fallback to GitHub source browsing

### Benefits We Gain

1. **Smaller Packages**: Base size without 20-30% symbol overhead
2. **Faster Restores**: Consumers download less data during `dotnet restore`
3. **Professional Standard**: Aligns with Microsoft and popular library practices
4. **Pay-to-Play Model**: Only debuggers incur symbol download cost
5. **Source Link Integration**: Full debugging experience with GitHub source navigation

### Neutral Outcomes

1. **Build Time**: No significant difference between portable and embedded
2. **CI Complexity**: Minimal - `dotnet pack` produces both formats automatically
3. **NuGet.org Storage**: Microsoft hosts symbol packages at no additional cost

## Implementation

### Current Configuration (Verified) - SUPERSEDED

> **IMPORTANT**: The configuration below documents what portable+snupkg **would have been**. The actual implementation uses **embedded symbols** instead. See `Directory.Build.props` lines 97-119 for the current embedded symbols configuration.

The following is what portable+snupkg configuration would look like:

```xml
<!--
  Source Link Configuration:
  - PublishRepositoryUrl and EmbedUntrackedSources are now handled by DotNet.ReproducibleBuilds
  - We only need to configure symbol package generation (not covered by the package)
-->
<PropertyGroup>
  <IncludeSymbols>true</IncludeSymbols>
  <SymbolPackageFormat>snupkg</SymbolPackageFormat>
</PropertyGroup>

<PropertyGroup Condition=" '$(Configuration)' == 'Release' ">
  <!-- Override DotNet.ReproducibleBuilds default of 'embedded' to 'portable' for separate .snupkg symbol packages -->
  <DebugType>portable</DebugType>
  <Optimize>true</Optimize>
  <DefineConstants>$(DefineConstants);TRACE</DefineConstants>
</PropertyGroup>
```

> **Note**: `PublishRepositoryUrl=true` and `EmbedUntrackedSources=true` are automatically set by `DotNet.ReproducibleBuilds` package. We do not set these explicitly to avoid configuration duplication.

**Actual Implementation (Embedded Symbols)**:

```xml
<!--
  Symbol Configuration:
  - Use embedded symbols for simplicity and enterprise firewall compatibility
  - Symbols are embedded in assemblies, no separate .snupkg needed
  - See ADR-012 for rationale
-->
<PropertyGroup>
  <IncludeSymbols>false</IncludeSymbols>
</PropertyGroup>

<PropertyGroup Condition=" '$(Configuration)' == 'Release' ">
  <!-- Use embedded symbols (DotNet.ReproducibleBuilds default) for just-works debugging -->
  <DebugType>embedded</DebugType>
  <Optimize>true</Optimize>
  <DefineConstants>$(DefineConstants);TRACE</DefineConstants>
</PropertyGroup>
```

### CI Publishing

The `release.yml` workflow explicitly pushes both package types for reliability:

```powershell
# Push nupkg files
foreach ($file in $nupkgFiles) {
    dotnet nuget push $file.FullName --api-key "$env:NUGET_API_KEY" `
        --source https://api.nuget.org/v3/index.json --skip-duplicate
}

# Push snupkg files separately for explicit control
foreach ($file in $snupkgFiles) {
    dotnet nuget push $file.FullName --api-key "$env:NUGET_API_KEY" `
        --source https://api.nuget.org/v3/index.json --skip-duplicate
}
```

> **Note**: While NuGet V3 API supports automatic `.snupkg` discovery when pushing `.nupkg`, explicit dual-push is preferred in CI for reliability and visibility. The `--skip-duplicate` flag provides idempotency for retries.

### Consumer Documentation

Add the following debugging section to README.md or package README:

#### Debugging Header

```markdown
## Debugging

QWIQ packages include Source Link support for full debugging with source code navigation.
```

#### Visual Studio Setup (one-time)

1. Tools > Options > Debugging > Symbols
2. Check "NuGet.org Symbol Server"
3. Enable "Load only specified modules" for faster debugging (optional)

#### JetBrains Rider

Works automatically when external sources are enabled in decompiler settings.

#### VS Code Setup

For .NET 7+ projects, add to your `.csproj`:

```xml
<PropertyGroup>
  <CopyDebugSymbolFilesFromPackages>true</CopyDebugSymbolFilesFromPackages>
</PropertyGroup>
```

Then configure `launch.json`:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/bin/Debug/net8.0/YourApp.dll",
      "justMyCode": false,
      "symbolOptions": {
        "searchMicrosoftSymbolServer": true,
        "searchNuGetOrgSymbolServer": true
      }
    }
  ]
}
```

#### Corporate Firewall Workaround

If `symbols.nuget.org` is blocked, you can:

1. Use Source Link for source browsing (requires GitHub access)
2. Clone the repository and build locally with embedded symbols
3. Request IT to whitelist `symbols.nuget.org` and `raw.githubusercontent.com`

## Rollback Strategy

If symbol packages fail validation post-publish or cause significant user friction:

### Detection

- Monitor GitHub issues for debugging complaints within 7 days of release
- Track NuGet.org download metrics for unexpected patterns
- Review symbol server indexing status via NuGet.org package page

### Triage

1. Reproduce issue locally with same package version
2. Verify `.snupkg` was correctly indexed on NuGet.org symbol server
3. Check if issue is configuration (consumer-side) vs. package (producer-side)

### Recovery Options

| Scenario                          | Action                                                   |
| --------------------------------- | -------------------------------------------------------- |
| Minor fix needed                  | Patch release (v11.0.x) with corrected symbols           |
| Major issue with portable symbols | Patch release with embedded symbols (revert to Option 2) |
| Symbol server indexing failed     | Re-push `.snupkg` files to NuGet.org                     |

### Reverting to Embedded Symbols

If portable symbols prove problematic for QWIQ's audience:

1. Update `Directory.Build.props`:

   ```xml
   <PropertyGroup Condition=" '$(Configuration)' == 'Release' ">
     <DebugType>embedded</DebugType>
   </PropertyGroup>
   <PropertyGroup>
     <IncludeSymbols>false</IncludeSymbols>
     <!-- Remove SymbolPackageFormat -->
   </PropertyGroup>
   ```

2. Publish patch release (e.g., v11.0.1)
3. Update README to remove symbol server configuration instructions

### Limitations

- Symbol packages already on `symbols.nuget.org` cannot be removed
- Cached symbols in developer IDEs persist until cache is cleared
- Unlisting packages prevents new installs but does not remove existing installations

## Validation

### Success Criteria

- [x] `Directory.Build.props` configured for portable + snupkg
- [ ] v11.0.0 packages publish successfully to NuGet.org
- [ ] Symbol packages indexed on NuGet.org symbol server
- [ ] Debugging into QWIQ source works in Visual Studio with symbol server configured
- [ ] Package size is baseline (no embedded symbol bloat)

### Monitoring

- NuGet.org package size metrics (compare to previous versions with embedded)
- GitHub issues tagged with `debugging` or `symbols`
- User feedback on debugging experience

## Related Decisions

- **ADR-005**: Central Package Management - Package versioning approach
- **Analysis-001**: Embedded vs Portable Symbols Analysis (`.agents/analysis/001-embedded-vs-portable-symbols-analysis.md`)
- **DotNet.ReproducibleBuilds**: External package providing deterministic build defaults

## References

- [Microsoft Learn: NuGet and .NET libraries](https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/nuget)
- [Microsoft Learn: Symbol packages (.snupkg)](https://learn.microsoft.com/en-us/nuget/create-packages/symbol-packages-snupkg)
- [NuGet Package Debugging & Symbols Improvements](https://github.com/NuGet/Home/wiki/NuGet-Package-Debugging-&-Symbols-Improvements)
- [dotnet/sdk Issue #2679: Discussion on embedded default](https://github.com/dotnet/sdk/issues/2679)
- [Ken Muse: What Every Developer Should Know About PDBs](https://www.kenmuse.com/blog/what-every-developer-should-know-about-pdbs/)
- [dotnet/runtime Directory.Build.props](https://github.com/dotnet/runtime/blob/main/Directory.Build.props)

## Consensus Review

**Review Date**: 2025-12-14

This ADR underwent multi-agent consensus review with 5 specialized agents:

| Agent                   | Verdict                  | Key Observations                                             |
| ----------------------- | ------------------------ | ------------------------------------------------------------ |
| **Architect**           | ACCEPT with observations | Format compliant, well-researched, minor code drift noted    |
| **Critic**              | APPROVED with caveats    | Enterprise audience considerations, documentation quality    |
| **DevOps**              | ACCEPT with revisions    | CI workflow robust, auto-push statement corrected            |
| **Independent Thinker** | RECONSIDER               | Scale mismatch with Microsoft patterns (noted, not blocking) |
| **QA**                  | NEEDS REVISION           | Validation gaps addressed with rollback strategy             |

**Consensus Achieved**: 4/5 agents approved the core technical decision. Revisions incorporated:

1. Fixed stale code block to match actual `Directory.Build.props`
2. Added rollback strategy section
3. Corrected auto-push mitigation to document explicit dual-push
4. Expanded VS Code debugging documentation
5. Added corporate firewall workarounds

**Contrarian View Acknowledged**: The independent-thinker noted QWIQ's low adoption (~1 download/day) may not justify portable symbols. This is acknowledged as an aspirational decision aligning QWIQ with professional public library standards for v11.0.0.

**Review Documents**:

- `.agents/critique/001-ADR-011-portable-symbols-critique.md`
- `.agents/qa/011-ADR-011-symbols-review.md`

---

## Superseded Decision

**Superseded Date**: 2025-12-14 (same day as original decision)

After the initial consensus review above, a **second consensus session** was held specifically on the embedded vs portable choice. The outcome was to **reverse this decision** and switch to embedded symbols.

**Final Consensus Vote**: 3 of 4 agents voted for embedded symbols (architect, devops, independent-thinker). QA acknowledged embedded is pragmatically justified despite technical preference for portable.

**Key Deciding Factors for Reversal**:

1. **Scale Mismatch**: At ~1 download/day, optimizing for 200KB bandwidth savings is premature optimization
2. **Enterprise Reality**: QWIQ's target audience (Azure DevOps/TFS users) disproportionately likely to be behind corporate firewalls that block symbol servers
3. **Maintainer Sustainability**: Single maintainer with limited time; CI/CD simplicity saves 10-15 minutes per release
4. **"Just Works" Debugging**: Embedded symbols work immediately in all IDEs without symbol server configuration

**Outcome**: The current implementation in `Directory.Build.props` uses `DebugType=embedded` and `IncludeSymbols=false`.

**Decision Documents**:

- `.agents/architecture/DECISION-SUMMARY-embedded-symbols.md` - Executive summary
- `.agents/architecture/002-symbols-consensus-recommendation.md` - Full multi-agent analysis

This ADR is retained for historical context, showing the analysis that led to the portable symbols decision before it was reconsidered and superseded by the embedded symbols approach.
