# ADR-012: Embedded Debug Symbols

**Status**: Accepted  
**Date**: 2025-12-14  
**Deciders**: Multi-Agent Consensus (Architect, DevOps, Independent Thinker, QA)  
**Context**: v11.0.0 NuGet Release Preparation  
**Supersedes**: ADR-011 (Portable Debug Symbols with Symbol Packages)

## Context and Problem Statement

QWIQ is preparing for its v11.0.0 NuGet release. The project uses `DotNet.ReproducibleBuilds` for deterministic builds, which defaults to `embedded` debug symbols. After initial analysis in ADR-011 recommended portable symbols with .snupkg packages, a multi-agent consensus review reconsidered this decision based on QWIQ's specific constraints.

**Key Questions**:

- Should debug symbols be embedded in assemblies or distributed separately?
- What decision framework is appropriate for QWIQ's scale and audience?
- How do we balance industry best practices with maintainer sustainability?

## Decision Drivers

### Scale Reality

1. **Download Volume**: ~1 download/day (~365/year)
2. **Total Bandwidth Impact**: 200KB × 365 = ~73MB/year with embedded symbols
3. **Context**: This is negligible compared to a single Windows update (~1GB)
4. **Conclusion**: Bandwidth optimization is solving a problem that doesn't exist at QWIQ's scale

### Audience Characteristics

1. **Enterprise Environment**: QWIQ targets Azure DevOps/TFS users in corporate settings
2. **Firewall Reality**: Corporate networks often block external symbol servers
3. **Symbol Server Access**: Many potential users cannot access `symbols.nuget.org`
4. **Impact**: Portable symbols optimize for an access pattern that may not exist for QWIQ's audience

### Maintainer Constraints

1. **Team Size**: Single maintainer with limited time
2. **Scarce Resource**: Maintainer time, not consumer bandwidth
3. **Sustainability**: Simpler workflows support long-term project health
4. **CI/CD Complexity**: Dual-package publishing adds operational overhead

### Debugging Experience

1. **Embedded**: Zero configuration - "just works" in all IDEs
2. **Portable**: Requires one-time symbol server configuration
3. **Enterprise Friction**: Configuration may be insufficient if symbol server is blocked
4. **Contributor Experience**: Uniform debugging experience reduces onboarding friction

## Multi-Agent Consensus

A formal multi-agent review was conducted with 4 specialized agents:

| Agent                   | Vote        | Rationale                                                  |
| ----------------------- | ----------- | ---------------------------------------------------------- |
| **Architect**           | EMBEDDED ✓  | Maintainer time > bandwidth at QWIQ's scale                |
| **DevOps**              | EMBEDDED ✓  | CI/CD simplification saves 10-15 min/release               |
| **Independent Thinker** | EMBEDDED ✓  | Following Microsoft is cargo culting; decide pragmatically |
| **QA**                  | PORTABLE ⚠️ | Technically superior, but pragmatically unjustified        |

**Consensus Level**: Strong majority (3/4 agents favor embedded)

**QA's Caveat**: While portable+snupkg is the technical best practice for public libraries, QA acknowledges that embedded is pragmatically justified for QWIQ's specific constraints.

## Decision

**Adopt embedded debug symbols for v11.0.0 and future releases.**

### Rationale

1. **Scale Appropriateness**

   - At QWIQ's download volume, bandwidth optimization is premature
   - 73MB/year total bandwidth cost is background noise
   - Optimizing for bytes at this scale diverts attention from value delivery

2. **Enterprise Compatibility**

   - QWIQ's audience is disproportionately likely to be behind corporate firewalls
   - Symbol server blocks are common in enterprise environments
   - Embedded symbols guarantee debugging works regardless of network policy

3. **Maintainer Sustainability**

   - Single-package workflow reduces CI/CD complexity
   - Fewer failure modes (no partial publish scenarios)
   - More time for features and bug fixes vs. infrastructure maintenance

4. **Contributor Experience**

   - Zero-configuration debugging reduces onboarding friction
   - Consistent experience across Visual Studio, Rider, VS Code
   - Anyone debugging QWIQ issues gets symbols immediately

5. **Context-Appropriate Decision**
   - Microsoft's portable symbol strategy optimizes for millions of downloads/day
   - QWIQ has ~1 download/day - fundamentally different constraints
   - Professional = making appropriate decisions for YOUR context, not copying Microsoft blindly

### Why This Differs from ADR-011

ADR-011 initially recommended portable symbols based on industry best practices and Microsoft guidance. The multi-agent consensus review challenged the assumption that Microsoft's optimization strategy applies to QWIQ's scale and audience. Key insights:

- **Cargo Culting**: Following Microsoft's approach without considering QWIQ's constraints
- **False Optimization**: Bandwidth savings are negligible at QWIQ's scale
- **Audience Mismatch**: Enterprise users often cannot access symbol servers anyway
- **Maintainer Reality**: Sustainability trumps theoretical best practices

## Considered Options

### Option 1: Portable Symbols with snupkg (ADR-011 Recommendation)

**Pros**:

- Smaller package size for all consumers
- Follows Microsoft guidance for public libraries
- Industry standard approach

**Cons**:

- Requires symbol server configuration (may fail in enterprise)
- Two packages to publish and validate
- Higher CI/CD complexity
- Support burden for "debugging doesn't work" issues
- Optimization doesn't matter at QWIQ's scale

### Option 2: Embedded Symbols (This Decision)

**Pros**:

- Zero-configuration debugging - "just works"
- No network dependency for symbols
- Single package to manage
- Simpler CI pipeline
- Works in corporate environments with symbol server blocks
- Appropriate for QWIQ's scale and audience

**Cons**:

- 20-30% larger packages (~200KB per package)
- Deviates from Microsoft's approach
- All consumers pay size penalty (even if they never debug)

## Implementation

### Current Configuration

`Directory.Build.props` is configured for embedded symbols:

```xml
<!--
  Symbol Configuration:
  - DebugType=embedded includes PDBs directly in assemblies
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

### CI Pipeline

The `release.yml` workflow publishes only `.nupkg` files (no `.snupkg` files):

```powershell
# Push nupkg files (contain embedded symbols)
foreach ($file in $nupkgFiles) {
    dotnet nuget push $file.FullName --api-key "$env:NUGET_API_KEY" `
        --source https://api.nuget.org/v3/index.json --skip-duplicate
}
```

### Package Size Validation

CI validates that packages remain within acceptable size thresholds:

```yaml
- name: Validate Package Sizes
  run: |
    $maxSize = 2MB
    Get-ChildItem artifacts/package/**/*.nupkg | ForEach-Object {
      if ($_.Length -gt $maxSize) {
        Write-Error "Package exceeds size threshold: $($_.Name) ($($_.Length) bytes)"
      }
    }
```

## Consequences

### Benefits We Gain

1. **Zero-Configuration Debugging**

   - Step into QWIQ source works immediately in any IDE
   - No symbol server setup required
   - No network dependency

2. **Enterprise Compatibility**

   - Works regardless of corporate firewall policies
   - No symbol server access required
   - Guaranteed debugging experience

3. **Maintainer Simplicity**

   - Single package type to publish and validate
   - Fewer CI/CD failure modes
   - Reduced support burden for debugging issues

4. **Contributor Experience**

   - Consistent debugging across all IDEs
   - No onboarding friction for symbol configuration
   - Immediate productivity when debugging issues

5. **Appropriate Optimization**
   - Resources focused on features/fixes, not theoretical bandwidth savings
   - Decision matches QWIQ's actual constraints

### Trade-offs We Accept

1. **Package Size**

   - ~20-30% larger packages (~200KB per package)
   - Total bandwidth impact: ~73MB/year at current download rate
   - **Assessment**: Negligible cost at QWIQ's scale

2. **Industry Standard Deviation**

   - Differs from Microsoft's approach for public libraries
   - **Assessment**: Microsoft's constraints ≠ QWIQ's constraints; context matters

3. **Symbols in Production**
   - Debug symbols embedded in deployed assemblies
   - **Assessment**: Not a security concern for public library; source is already public

### Monitoring

Track the following metrics post-v11.0.0 release:

- **Package Size**: Should be ~200-300KB per package
- **User Feedback**: Monitor for "package too large" complaints
- **Debugging Issues**: Track "debugging doesn't work" support requests (should be zero)
- **Download Trends**: If scale increases significantly, reevaluate

## Reevaluation Triggers

This decision should be reconsidered if:

1. **Download volume exceeds 100/day** - scaling assumptions change
2. **Multiple users report package size as a problem** - optimization becomes meaningful
3. **Microsoft publishes guidance for low-volume libraries** - new information available
4. **Symbol server access becomes universal** - enterprise firewall assumptions change

## Related Decisions

- **ADR-011**: Portable Debug Symbols with Symbol Packages (Superseded)
- **ADR-005**: Central Package Management
- **DECISION-SUMMARY-embedded-symbols.md**: Executive summary of multi-agent consensus
- **002-symbols-consensus-recommendation.md**: Full multi-agent analysis and voting record

## References

- [Multi-Agent Consensus Document](./002-symbols-consensus-recommendation.md)
- [Decision Summary](./DECISION-SUMMARY-embedded-symbols.md)
- [DotNet.ReproducibleBuilds](https://github.com/dotnet/reproducible-builds)
- [Microsoft Learn: Symbol packages](https://learn.microsoft.com/en-us/nuget/create-packages/symbol-packages-snupkg)
- [Ken Muse: What Every Developer Should Know About PDBs](https://www.kenmuse.com/blog/what-every-developer-should-know-about-pdbs/)

## Documentation

### README.md Section

Added debugging documentation to README.md:

```markdown
## Debugging

QWIQ packages include embedded debug symbols for a seamless debugging experience.

### Visual Studio, Rider, or VS Code

Step into QWIQ source code works immediately - no symbol server configuration required.

1. Set a breakpoint in your code that calls QWIQ
2. Press F11 to step into QWIQ methods
3. Source code will be retrieved via Source Link from GitHub

### Why Embedded Symbols?

QWIQ uses embedded debug symbols instead of separate symbol packages for:

- **Zero configuration**: Debugging works immediately in any IDE
- **Enterprise compatibility**: No dependency on external symbol servers that may be blocked
- **Maintainer simplicity**: Single package to manage and publish
- **Contributor experience**: Consistent debugging for everyone contributing to QWIQ

_Note: Package size is approximately 20-30% larger due to embedded symbols. For QWIQ's scale (~1 download/day), this is a negligible bandwidth cost compared to the debugging convenience._
```

## Success Criteria

- [x] `Directory.Build.props` configured for embedded symbols
- [x] Multi-agent consensus documented
- [x] ADR-012 created with comprehensive rationale
- [x] ADR-011 marked as superseded
- [ ] v11.0.0 packages published with embedded symbols
- [ ] Package size within expected range (~200-300KB increase per package)
- [ ] Zero "debugging doesn't work" issues related to symbol access
- [ ] README.md updated with debugging documentation

## Validation

### Build Validation

```bash
# Verify DebugType configuration
dotnet build -c Release -v:minimal

# Check package contents for embedded PDBs
dotnet pack -c Release
unzip -l artifacts/package/release/Qwiq.Core.*.nupkg | grep -i "\.pdb"
```

### CI Validation

- Package size regression test (max 2MB per package)
- No `.snupkg` files generated
- Release workflow publishes only `.nupkg` files

## Conclusion

This decision prioritizes:

1. **Maintainer sustainability** over theoretical optimization
2. **User debugging experience** over package size
3. **Enterprise compatibility** over industry patterns
4. **Context-appropriate decisions** over cargo culting

For a library with QWIQ's scale (~1 download/day) and audience (enterprise Azure DevOps/TFS users), embedded symbols are the pragmatic choice that maximizes value delivery while respecting maintainer constraints.
