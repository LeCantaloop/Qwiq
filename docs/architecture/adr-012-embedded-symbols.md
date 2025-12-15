# ADR-012: Embedded Debug Symbols

**Status**: Accepted  
**Date**: 2025-12-14  
**Deciders**: Multi-agent consensus (Architect, DevOps, Independent Thinker, QA)

## Context

QWIQ previously used portable debug symbols distributed via separate .snupkg packages uploaded to NuGet's symbol server. This followed Microsoft's recommended best practice for public NuGet packages.

However, this approach introduced complexity:

- Dual-package publishing in CI/CD workflows
- Additional release validation steps
- Symbol server configuration required for debugging
- Potential firewall/proxy issues for enterprise users

## Decision

**Switch to embedded debug symbols** where PDB data is embedded directly in the assembly DLLs.

### Configuration

```xml
<!-- Directory.Build.props -->
<PropertyGroup Condition=" '$(Configuration)' == 'Release' ">
  <DebugType>embedded</DebugType>
</PropertyGroup>

<PropertyGroup>
  <IncludeSymbols>false</IncludeSymbols>
</PropertyGroup>
```

## Rationale

### 1. Scale-Appropriate Decision

QWIQ averages ~1 download/day:

- Portable symbols save ~200KB per download
- Annual bandwidth impact: ~73MB (negligible)
- Optimization targets wrong constraint (bandwidth vs. maintainer time)

### 2. Enterprise Firewall Reality

QWIQ's target audience (Azure DevOps/TFS developers) disproportionately work in enterprise environments:

- Corporate firewalls often block external symbol servers
- Symbol server access requires IT approval/configuration
- Embedded symbols eliminate this friction entirely

### 3. Maintainer Sustainability

Single-maintainer project with limited time:

- Simpler CI/CD (one package type vs. two)
- Reduced cognitive overhead during releases
- More time for feature development and bug fixes

### 4. "Just Works" Developer Experience

Contributors and users get debugging automatically:

- No symbol server configuration needed
- Works consistently across VS, Rider, VS Code
- Reduces onboarding friction

## Consequences

### Positive

- **Simpler CI/CD**: Single package type to build, validate, and publish
- **Universal debugging**: Works behind any firewall, no configuration
- **Reduced maintenance**: Fewer moving parts in release process
- **Faster releases**: 10-15 minutes saved per release (no dual-package handling)

### Negative

- **Larger packages**: ~200KB increase per .nupkg file
- **Bandwidth**: Marginal increase (73MB/year at current scale)
- **Industry alignment**: Deviates from Microsoft's guidance for high-volume libraries

### Mitigation

- **Package size validation**: CI script checks packages don't exceed 2MB threshold
- **Monitoring**: Watch for user feedback on package sizes post-v11.0.0
- **Reevaluation trigger**: Reconsider if downloads scale 10x (to ~10/day)

## Alternatives Considered

### Portable Symbols (status quo)

**Pros**: Industry standard, smaller packages, bandwidth-efficient at scale  
**Cons**: CI/CD complexity, firewall issues, requires configuration

**Rejection rationale**: Optimizes for constraints that don't apply to QWIQ's scale and audience.

### PDB-only (no symbols)

**Pros**: Smallest packages  
**Cons**: No debugging support at all

**Rejection rationale**: Debugging support is valuable for library consumers.

## Validation

1. ✅ Build succeeds with `<DebugType>embedded</DebugType>`
2. ✅ Package size validation script in place (`scripts/Validate-PackageSize.ps1`)
3. ✅ CI/CD workflow simplified (no snupkg handling)
4. ✅ README documents debugging experience

## References

- **Full Analysis**: `.agents/architecture/002-symbols-consensus-recommendation.md`
- **Recommendation**: `.agents/architecture/RECOMMENDATION-FOR-MAINTAINER.md`
- **Decision Summary**: `.agents/architecture/DECISION-SUMMARY-embedded-symbols.md`
- **Microsoft Guidance**: [Publish symbol packages](https://learn.microsoft.com/en-us/nuget/create-packages/symbol-packages-snupkg)

## Notes

This is a **pragmatic decision** for QWIQ's context:

- Scale: ~1 download/day
- Audience: Enterprise developers
- Maintenance: Single maintainer

Different constraints would justify different decisions. This ADR documents why embedded symbols are **right for QWIQ**, not universally superior.
