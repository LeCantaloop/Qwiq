# Epic: Reproducible Builds Integration

**Created:** 2025-12-14
**Priority:** P1 (Important)
**Target Release:** v11.0.0

## Summary

Integrate DotNet.ReproducibleBuilds v1.2.39 from .NET Foundation to standardize QWIQ's reproducible build configuration.

## Key Points

- Low effort: 2 lines of configuration (Directory.Packages.props + Directory.Build.props)
- Complements existing SLSA provenance and SBOM generation
- Enables byte-for-byte build verification for supply chain trust
- Enterprise security audits increasingly require reproducibility evidence
- Analyst report completed 2025-12-14 with Option A (Full Integration) recommendation

## Dependencies

- Prerequisite: Analyst report (COMPLETE)
- Parallel: W2.22 SHA Pinning
- Blocks: W2.33 NuGet v11.0.0 Publish

## Success Criteria

- Package added with PrivateAssets="All"
- Existing explicit settings preserved (DebugType=portable, SourceLink)
- CI build passes with 0 errors, 0 warnings
- Package validation succeeds

## Location

- Roadmap: `.agents/roadmap/product-roadmap.md`
- Analysis: `.agents/analysis/001-dotnet-reproducible-builds-analysis.md`
