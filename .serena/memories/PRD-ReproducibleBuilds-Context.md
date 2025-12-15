# PRD: Reproducible Builds Integration Context

## Overview

Created: 2025-12-14
Location: `.agents/planning/PRD-reproducible-builds.md`

## Summary

This PRD describes integrating `DotNet.ReproducibleBuilds` v1.2.39 into QWIQ for enhanced CI platform detection and build reproducibility.

## Key Points

- **Package**: DotNet.ReproducibleBuilds v1.2.39 (.NET Foundation maintained)
- **Primary Goal**: Automatic CI detection across 11 platforms (vs current single CI=true)
- **Current State**: Manual CI detection, explicit Deterministic/SourceLink settings already in place
- **Risk Level**: Low - explicit settings take precedence over package defaults
- **Effort**: 2 story points (~1.5 hours)

## Files to Modify

1. `Directory.Packages.props` - Add package version
2. `Directory.Build.props` - Add package reference with PrivateAssets="All"
3. `CLAUDE.md` - Document the integration

## Out of Scope

- Changing DebugType from portable to embedded
- Removing existing explicit reproducibility settings
- Multi-CI platform testing infrastructure
- Byte-for-byte reproducibility verification

## Acceptance Criteria (Epic)

1. Package version in Directory.Packages.props
2. Package reference in Directory.Build.props with PrivateAssets="All"
3. CI detection works on GitHub Actions
4. Existing settings (Deterministic, PublishRepositoryUrl, EmbedUntrackedSources) preserved
5. All tests pass
6. SourceLink verification passes
7. Documentation updated
8. Package not included in published NuGet packages

## Related Entities

- **Epic**: Integrate DotNet.ReproducibleBuilds Package for Enhanced CI Detection
- **Priority**: Medium
- **Labels**: enhancement, build, supply-chain, low-risk
