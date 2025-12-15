# Explainer: DotNet.ReproducibleBuilds Integration

## Introduction/Overview

This PRD describes the integration of the `DotNet.ReproducibleBuilds` NuGet package into the QWIQ library to enhance build reproducibility, improve CI platform detection, and align with .NET Foundation best practices for supply chain security.

**Problem Statement**: QWIQ currently uses manual CI detection via a single environment variable (`CI=true`), which only works reliably on GitHub Actions. As the library may be built across different CI platforms (Azure DevOps, GitLab CI, Jenkins, TeamCity, etc.), this creates inconsistent build behavior. The `DotNet.ReproducibleBuilds` package provides automatic detection for 11 CI platforms and encapsulates community-standard reproducibility configurations.

**Current State**:

- Manual CI detection: `<ContinuousIntegrationBuild Condition="'$(CI)' == 'true'">true</ContinuousIntegrationBuild>`
- Existing reproducibility settings already configured:
  - `Deterministic=true`
  - `PublishRepositoryUrl=true`
  - `EmbedUntrackedSources=true`
  - `DebugType=embedded` (Release) - See ADR-012
- SLSA Level 3 provenance generation already in place
- SBOM generation already configured

**Proposed Solution**: Add the `DotNet.ReproducibleBuilds` package as a development dependency to centralize reproducibility configuration and gain automatic CI platform detection across all supported platforms.

## Goals

1. **Broaden CI Platform Support**: Automatically detect CI environments across 11 platforms (GitHub Actions, Azure DevOps, GitLab CI, Jenkins, TeamCity, Bitbucket Pipelines, CircleCI, Travis CI, AppVeyor, Drone, and Bamboo)

2. **Align with .NET Foundation Standards**: Adopt the official .NET Foundation package for reproducible builds, used by 798+ projects including major .NET libraries

3. **Reduce Maintenance Burden**: Delegate reproducibility configuration to a community-maintained package that tracks evolving best practices

4. **Strengthen Supply Chain Security**: Complement existing SLSA provenance and SBOM generation with standardized reproducibility guarantees

5. **Enable Future Extensibility**: Position QWIQ to automatically benefit from future reproducibility enhancements without code changes

## Non-Goals (Out of Scope)

1. **Changing DebugType**: QWIQ explicitly sets `DebugType=embedded` for Release builds per ADR-012. The package defaults to `embedded`, so this aligns naturally and no override is required. We will NOT change this behavior.

2. **Retaining Redundant Explicit Settings**: The `DotNet.ReproducibleBuilds` package automatically provides `Deterministic=true`, `PublishRepositoryUrl=true`, and `EmbedUntrackedSources=true`. These redundant explicit settings were removed from `Directory.Build.props` to avoid duplication and let the package manage them.

3. **Modifying CI Workflows**: The GitHub Actions workflows will not be modified as part of this integration. The `CI=true` detection remains as a fallback.

4. **Adding Additional SourceLink Packages**: QWIQ already includes `Microsoft.SourceLink.GitHub`. The package's SourceLink auto-configuration is a bonus but not a primary goal.

5. **Verifying Build Reproducibility**: While the package enables reproducibility, this PRD does not include verification that builds are byte-for-byte reproducible across machines.

6. **Multi-CI Testing**: We will not set up testing infrastructure for all 11 supported CI platforms.

## User Stories

### Primary Users: Library Maintainers

**US-1**: As a library maintainer, I want builds to automatically enable CI-specific settings regardless of which CI platform is used, so that I don't need to manually configure each platform.

**US-2**: As a library maintainer, I want reproducibility best practices to be managed by a community-maintained package, so that I automatically benefit from evolving standards without tracking changes myself.

**US-3**: As a library maintainer, I want to ensure packages built on different machines are binary-identical, so that consumers can verify package integrity.

### Secondary Users: Library Consumers

**US-4**: As a library consumer, I want to verify that QWIQ packages were built from the claimed source code, so that I can trust the supply chain.

**US-5**: As a library consumer building from source, I want the same reproducibility behavior regardless of whether I build locally or on my organization's CI system.

### Tertiary Users: Security Auditors

**US-6**: As a security auditor, I want QWIQ to use .NET Foundation-approved packages for build reproducibility, so that I can reference established security guidance.

## Functional Requirements

### FR-1: Package Installation

The system must add `DotNet.ReproducibleBuilds` version 1.2.39 (or latest stable) to `Directory.Packages.props` using Central Package Management.

**Acceptance Criteria**:

- Package version is specified in `Directory.Packages.props`
- Package is referenced in `Directory.Build.props` with `PrivateAssets="All"`
- Package is not included in published NuGet packages (development dependency only)

### FR-2: CI Platform Auto-Detection

The system must automatically set `ContinuousIntegrationBuild=true` when building on any of the following CI platforms:

| Platform            | Detection Variable       |
| ------------------- | ------------------------ |
| GitHub Actions      | `GITHUB_ACTIONS`         |
| Azure DevOps        | `TF_BUILD`               |
| GitLab CI           | `GITLAB_CI`              |
| Jenkins             | `JENKINS_URL`            |
| TeamCity            | `TEAMCITY_VERSION`       |
| Bitbucket Pipelines | `BITBUCKET_BUILD_NUMBER` |
| CircleCI            | `CIRCLECI`               |
| Travis CI           | `TRAVIS`                 |
| AppVeyor            | `APPVEYOR`               |
| Drone               | `DRONE`                  |
| Bamboo              | `bamboo_planKey`         |

**Acceptance Criteria**:

- Builds on GitHub Actions continue to set `ContinuousIntegrationBuild=true`
- Existing `CI=true` detection remains as fallback
- No manual configuration required for any supported platform

### FR-3: Reproducibility Settings Preservation

The system must preserve existing explicit reproducibility settings that take precedence over package defaults.

**Acceptance Criteria**:

- `Deterministic=true` remains explicitly set
- `DebugType=embedded` remains explicitly set for Release builds (per ADR-012)
- `PublishRepositoryUrl=true` remains explicitly set
- `EmbedUntrackedSources=true` remains explicitly set
- Build output is unchanged from current behavior

### FR-4: SourceLink Integration

The system must continue to use `Microsoft.SourceLink.GitHub` for GitHub repository linking.

**Acceptance Criteria**:

- Existing SourceLink verification continues to pass
- PDB files contain correct source links (embedded in assemblies per ADR-012)

### FR-5: Documentation Update

The system must document the integration in `CLAUDE.md` and relevant build documentation.

**Acceptance Criteria**:

- `CLAUDE.md` documents the reproducible builds configuration
- Comments in `Directory.Build.props` explain the package purpose
- Any changes are reflected in build troubleshooting documentation

## Design Considerations

### Integration Approach

The package should be added with minimal changes to existing configuration:

```xml
<!-- In Directory.Packages.props -->
<PackageVersion Include="DotNet.ReproducibleBuilds" Version="1.2.39" />

<!-- In Directory.Build.props -->
<ItemGroup>
  <PackageReference Include="DotNet.ReproducibleBuilds" PrivateAssets="All" />
</ItemGroup>
```

### Configuration Layering

The MSBuild property evaluation order ensures explicit settings take precedence:

1. SDK defaults
2. DotNet.ReproducibleBuilds package defaults
3. Directory.Build.props explicit settings (QWIQ)
4. Individual project settings

QWIQ explicitly sets `DebugType=embedded` per ADR-012, which aligns with the package's `DebugType=embedded` default (no override needed).

### Backward Compatibility

The integration should be transparent to consumers. No public API or behavior changes are expected.

## Technical Considerations

### Prerequisites

- **MSBuild 17.8+**: Required by `DotNet.ReproducibleBuilds` 1.2.x. QWIQ uses .NET 10 SDK which includes MSBuild 17.x (compatible).
- **Central Package Management**: QWIQ uses CPM; package must be added to `Directory.Packages.props`

### Package Characteristics

| Property     | Value                               |
| ------------ | ----------------------------------- |
| Package ID   | `DotNet.ReproducibleBuilds`         |
| Version      | 1.2.39 (latest as of December 2025) |
| Maintainer   | .NET Foundation                     |
| License      | MIT                                 |
| Dependencies | None                                |
| Size         | ~15 KB                              |

### Risk Assessment

| Risk                           | Likelihood | Impact     | Mitigation                                                 |
| ------------------------------ | ---------- | ---------- | ---------------------------------------------------------- |
| Package sets unwanted defaults | Low        | Low        | Explicit settings in Directory.Build.props take precedence |
| CI detection conflicts         | Low        | Low        | Existing `CI=true` fallback remains                        |
| Future breaking changes        | Low        | Medium     | Pin to specific version; update intentionally              |
| Build time increase            | Negligible | Negligible | Package only adds MSBuild targets, no runtime cost         |

### Compatibility Matrix

| Component                      | Compatibility              |
| ------------------------------ | -------------------------- |
| .NET 10 SDK                    | Compatible (MSBuild 17.x)  |
| .NET 8.0 TFM                   | Compatible                 |
| .NET Framework 4.7.2/4.8/4.8.1 | Compatible                 |
| Central Package Management     | Compatible                 |
| SourceLink                     | Compatible (complementary) |
| Nerdbank.GitVersioning         | Compatible (no conflicts)  |

## Success Metrics

### Quantitative Metrics

1. **CI Detection Coverage**: Builds on any of the 11 supported CI platforms should automatically enable `ContinuousIntegrationBuild=true` (measurable via build logs)

2. **Build Parity**: Debug symbols and package metadata should be identical whether built locally (with CI env var) or on CI

3. **Zero Regressions**: All existing tests continue to pass; SourceLink verification continues to pass

### Qualitative Metrics

1. **Alignment**: QWIQ follows .NET Foundation recommended practices

2. **Maintainability**: Reduced cognitive load for understanding CI-specific build configuration

3. **Discoverability**: Other maintainers can identify reproducibility configuration via well-known package name

## Implementation Tasks

### Phase 1: Package Integration (Estimated: 1-2 hours)

1. Add package version to `Directory.Packages.props`
2. Add package reference to `Directory.Build.props`
3. Add explanatory comments

### Phase 2: Verification (Estimated: 1-2 hours)

1. Build locally and verify no behavior change
2. Build on GitHub Actions and verify CI detection
3. Run SourceLink verification
4. Review build logs for expected property values

### Phase 3: Documentation (Estimated: 30 minutes)

1. Update `CLAUDE.md` build configuration section
2. Add inline comments explaining integration

## Open Questions

1. **Q**: Should we remove the explicit `<ContinuousIntegrationBuild Condition="'$(CI)' == 'true'">` line after integration?
   **Recommendation**: Keep it for clarity and as documentation, even though it becomes redundant. The package handles this automatically, but explicit is better than implicit.

2. **Q**: Should we update `DebugType` to `embedded` to match the package default?
   **Recommendation**: Already using `embedded` per ADR-012. This aligns naturally with the package default and provides just-works debugging without symbol server configuration.

3. **Q**: Should we verify byte-for-byte reproducibility across machines?
   **Recommendation**: Out of scope for initial integration. Can be a follow-up investigation if needed.

4. **Q**: Should we add verification that the package is correctly detecting CI platforms?
   **Recommendation**: Add a build log check step that outputs `ContinuousIntegrationBuild` property value on CI builds.

---

## Epic: Reproducible Builds Integration

### Title

Integrate DotNet.ReproducibleBuilds Package for Enhanced CI Detection and Build Reproducibility

### Description

As a library maintainer, I want to integrate the `DotNet.ReproducibleBuilds` NuGet package so that QWIQ builds automatically enable reproducibility settings across all major CI platforms without manual configuration.

This epic covers adding the .NET Foundation-maintained `DotNet.ReproducibleBuilds` package to QWIQ, which will:

- Automatically detect 11 CI platforms and set `ContinuousIntegrationBuild=true`
- Align QWIQ with community-standard reproducibility practices
- Complement existing SLSA provenance and SBOM supply chain security measures
- Reduce maintenance burden by delegating reproducibility configuration to a community-maintained package

**Background**: QWIQ currently detects CI via the `CI` environment variable, which works for GitHub Actions but may not work on other platforms. The `DotNet.ReproducibleBuilds` package provides broader detection and encapsulates evolving best practices.

**Scope**:

- Add `DotNet.ReproducibleBuilds` v1.2.39 to Central Package Management
- Reference package in `Directory.Build.props` as development dependency
- Remove redundant explicit settings now provided by the package (`Deterministic`, `PublishRepositoryUrl`, `EmbedUntrackedSources`)
- Preserve `DebugType=embedded` (per ADR-012, aligns with package default)
- Verify no regression in build behavior or SourceLink functionality
- Document the integration

**Out of Scope**:

- Changing DebugType (already `embedded` per ADR-012, aligns with package default)
- Setting up multi-CI platform testing infrastructure
- Verifying byte-for-byte build reproducibility

### Acceptance Criteria

1. **Package Installed**: `DotNet.ReproducibleBuilds` version 1.2.39 is listed in `Directory.Packages.props`

2. **Package Referenced**: `Directory.Build.props` includes `<PackageReference Include="DotNet.ReproducibleBuilds" PrivateAssets="All" />`

3. **CI Detection Works**: GitHub Actions builds show `ContinuousIntegrationBuild=true` in build output (existing behavior preserved)

4. **Redundant Settings Removed**: The following were removed from `Directory.Build.props` since `DotNet.ReproducibleBuilds` now provides them:

   - `Deterministic=true` (package default)
   - `PublishRepositoryUrl=true` (package default)
   - `EmbedUntrackedSources=true` (package default)

   **Note**: `DebugType=embedded` (Release) is preserved per ADR-012 and aligns with the package default.

5. **All Tests Pass**: CI build succeeds with no test regressions

6. **SourceLink Verified**: SourceLink verification step passes on push builds

7. **Documentation Updated**: `CLAUDE.md` and `Directory.Build.props` contain explanatory comments about the integration

8. **Package Not Published**: The package is marked with `PrivateAssets="All"` so it is not included in QWIQ's published NuGet packages

### Story Points

**Estimate**: 2 Story Points

**Rationale**:

- Low complexity (adding a package with known behavior)
- Low risk (explicit settings take precedence)
- Minimal code changes (2 files)
- Standard verification (existing CI pipeline)
- Well-documented package behavior

**Breakdown**:

| Task                                    | Effort                |
| --------------------------------------- | --------------------- |
| Add package to Directory.Packages.props | 5 min                 |
| Add reference to Directory.Build.props  | 10 min                |
| Local build verification                | 15 min                |
| CI build verification                   | 30 min (pipeline run) |
| SourceLink verification                 | 10 min                |
| Documentation updates                   | 20 min                |
| **Total**                               | ~1.5 hours            |

### Labels

- `enhancement`
- `build`
- `supply-chain`
- `low-risk`

### Priority

**Medium** - Enhances build infrastructure and supply chain security but does not address immediate functional gaps or bugs.

---

## Appendix: Package Research Summary

### DotNet.ReproducibleBuilds Package Details

**Source**: [GitHub - dotnet/reproducible-builds](https://github.com/dotnet/reproducible-builds)
**NuGet**: [DotNet.ReproducibleBuilds](https://www.nuget.org/packages/DotNet.ReproducibleBuilds)

**What the package does**:

1. **CI Auto-Detection**: Sets `ContinuousIntegrationBuild=true` when any of 11 CI platform environment variables are detected

2. **SourceLink Auto-Configuration**: Automatically adds appropriate SourceLink package based on repository URL (GitHub, GitLab, Azure DevOps, Bitbucket)

3. **Default Settings**: Sets recommended defaults:
   - `PublishRepositoryUrl=true`
   - `EmbedUntrackedSources=true`
   - `DebugType=embedded` (can be overridden)

**Adoption**: Used by 798+ projects including:

- ASP.NET Core
- Entity Framework Core
- xUnit
- NuGet Client
- MSBuild

**Version History**:

- 1.2.39 (current) - Requires MSBuild 17.8+
- 1.1.x - Legacy versions for older MSBuild
