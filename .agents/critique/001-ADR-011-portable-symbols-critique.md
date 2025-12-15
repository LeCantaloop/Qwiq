# Plan Critique: ADR-011 Portable Debug Symbols with Symbol Packages

## Verdict

**APPROVED** - with documented caveats

## Summary

ADR-011 makes a reasonable technical decision to use portable symbols with separate `.snupkg` packages for QWIQ's v11.0.0 NuGet release. The analysis is largely sound, the industry comparison is fair, and the implementation is already verified in `Directory.Build.props`. However, the ADR has several blind spots and understates certain risks that should be acknowledged.

## Strengths

1. **Configuration Already Verified**: The ADR documents what is already implemented in `Directory.Build.props` (lines 102-119), reducing implementation risk.

2. **CI Pipeline Aligned**: The `release.yml` workflow (lines 100-121) already properly handles both `.nupkg` and `.snupkg` files separately, demonstrating the dual-package publishing works.

3. **Industry Survey is Accurate**: The comparison to dotnet/runtime, Serilog, and AutoMapper is factually correct - these libraries do use snupkg.

4. **Clear Consumer Documentation**: The ADR provides actionable IDE setup instructions that can be directly added to README.

5. **Proper DotNet.ReproducibleBuilds Override**: Correctly identifies the need to override the package's default of `embedded` to `portable`.

## Issues Found

### Critical (Must Fix)

None - the decision is technically sound for a public NuGet library.

### Important (Should Fix)

- [ ] **Missing Size Validation**: The ADR claims "20-30% size penalty" but provides **no actual measurements** for QWIQ packages. With 10 packable projects and 6 TFMs each, this should be measured, not assumed.

- [x] **~~Inconsistent Project Count~~**: ~~The ADR states "9 packable projects" but actual count from csproj analysis shows **10** IsPackable=true projects~~ **RESOLVED** - ADR updated to correct count of 10 packable projects:

  - `Qwiq.Core`
  - `Qwiq.Client.Rest`
  - `Qwiq.Client.Soap`
  - `Qwiq.Identity`
  - `Qwiq.Identity.Soap`
  - `Qwiq.Linq`
  - `Qwiq.Linq.Identity`
  - `Qwiq.Mapper`
  - `Qwiq.Mapper.Identity`
  - `Qwiq.Mocks` (test library intentionally packable - shipped as NuGet)

- [ ] **PublishRepositoryUrl/EmbedUntrackedSources Discrepancy**: The ADR's "Current Configuration (Verified)" section (lines 186-198) shows these properties explicitly set, but the actual `Directory.Build.props` (lines 97-105) has a comment stating "PublishRepositoryUrl and EmbedUntrackedSources are now handled by DotNet.ReproducibleBuilds" and does NOT set them explicitly. The ADR's code block is **stale/incorrect**.

### Minor (Consider)

- [ ] **"Professionalism" as Rationale**: Calling snupkg the "more professional" approach (line 133) is subjective. Embedded symbols are equally professional when appropriate for the use case.

- [ ] **Rider "Works Automatically" Claim**: This is partially misleading. Rider does work automatically with NuGet.org symbols, but only if the user has enabled external sources in decompiler settings - it is not truly zero-configuration.

- [ ] **No Rollback Plan**: The ADR lacks a rollback strategy if symbol publishing fails or causes issues. What happens if NuGet.org symbol server indexing fails for a release?

- [ ] **Missing GitHub Packages Consideration**: The ADR does not mention that if QWIQ ever publishes to GitHub Packages (e.g., for pre-release testing), snupkg is not supported there. This is mentioned in the analysis document but missing from the ADR.

## Questions for Planner

1. **Has the 20-30% size impact been measured for QWIQ specifically?** The claim is generic industry data. Given QWIQ's codebase size, the actual impact could be smaller or larger.

2. **~~Is Qwiq.Mocks intentionally packable?~~** **RESOLVED** - Yes, Qwiq.Mocks is intentionally packable as it's a test library shipped as a NuGet package for consumer use.

3. **What is the fallback if NuGet.org symbol server fails?** Source Link provides source browsing but not step-through debugging without PDBs.

## Blind Spots Identified

### 1. SOAP Client Windows-Only Scenario

The ADR does not consider that `Qwiq.Core.Soap` and `Qwiq.Identity.Soap` are **net472-only** Windows packages. Consumers of these packages are more likely to be in corporate/enterprise environments where:

- External symbol servers may be blocked
- Embedded symbols would actually be preferable

**Impact**: Medium - affects a subset of users, but potentially the most friction-sensitive subset.

### 2. No Measurement Baseline

The ADR accepts the decision to use snupkg without establishing:

- Current package sizes (no published packages yet for v11)
- Projected size difference
- Download count expectations (is bandwidth really a concern for QWIQ's audience?)

**Impact**: Low - the industry standard argument is sufficient, but data would strengthen the case.

### 3. CI Failure Modes

The release workflow pushes snupkg separately from nupkg (lines 105-121). If nupkg succeeds but snupkg fails:

- Packages are published without symbols
- No alerting mechanism in ADR
- Manual intervention required

**Impact**: Low - unlikely, but should be documented.

## Strongest Argument AGAINST This Decision

**The "corporate firewall" problem is understated for QWIQ's target audience.**

QWIQ is a library for Azure DevOps / TFS work item queries. Its primary users are:

1. Enterprise developers working with on-premises TFS (hence SOAP support)
2. Corporate environments with Azure DevOps
3. Teams often behind corporate proxies and firewalls

These users are **disproportionately likely** to be in environments where:

- External symbol servers are blocked
- Security policies prevent downloading arbitrary binaries
- IT controls prevent IDE configuration changes

For this specific library, the "one-time IDE configuration" assumption may not hold. The user may never be able to configure the symbol server at all.

**Counter-argument**: Source Link still enables source browsing even without PDB download. The debugging experience is degraded but not completely broken. Additionally, modern Azure DevOps (REST client) is increasingly used from less restrictive environments.

## Could This Decision Backfire?

### Scenario 1: Symbol Server Outage

NuGet.org symbol server experiences downtime during a critical debugging session. Users cannot step through QWIQ code.

**Likelihood**: Low (NuGet.org is highly available)
**Mitigation**: Source Link fallback, documented in ADR

### Scenario 2: Enterprise Adoption Friction

A large enterprise evaluates QWIQ but cannot debug due to symbol server blocks. They choose a different library or fork QWIQ.

**Likelihood**: Medium (enterprise environments are restrictive)
**Mitigation**: Document workaround (build from source), consider offering embedded variant

### Scenario 3: CI Publishing Drift

Future maintainers misunderstand the dual-package requirement. CI change breaks snupkg publishing silently.

**Likelihood**: Low (workflow is well-structured)
**Mitigation**: Add validation step to CI that verifies snupkg count matches nupkg count

## Recommendations

1. **Update Code Block**: Fix the "Current Configuration (Verified)" section to match actual `Directory.Build.props` content (remove explicit PublishRepositoryUrl/EmbedUntrackedSources that are handled by the package).

2. **Add Size Measurements**: Before v11.0.0 release, measure actual package sizes and document in the ADR for future reference.

3. **~~Correct Package Count~~**: ~~Update from "9 packable projects" to actual count (10, or 9 if Qwiq.Mocks is corrected)~~ **RESOLVED** - Updated to 10 packable projects.

4. **Document Enterprise Workaround**: Add a note that enterprise users who cannot access symbol servers can:

   - Use Source Link for source browsing
   - Clone the repository and build with embedded symbols locally
   - Request IT to whitelist `symbols.nuget.org`

5. **Add CI Validation**: Consider adding a step to release.yml that verifies snupkg count matches nupkg count before publishing.

## Approval Conditions

The ADR can proceed to implementation as-is because:

1. The core technical decision (portable + snupkg) is correct for a public NuGet library
2. Configuration is already implemented and verified
3. CI pipeline already handles the dual-package scenario
4. Issues identified are documentation quality, not fundamental approach problems

**Recommended**: Address Important issues before v11.0.0 release, but do not block the ADR approval.

## Comparison Fairness Assessment

The industry comparison table is **fair but incomplete**:

| Claim                          | Assessment                                         |
| ------------------------------ | -------------------------------------------------- |
| dotnet/runtime uses portable   | **TRUE** - verified in their Directory.Build.props |
| dotnet/aspnetcore uses snupkg  | **TRUE** - IncludeSymbols is set                   |
| Serilog uses snupkg            | **TRUE** - verified                                |
| AutoMapper uses snupkg         | **TRUE** - verified                                |
| Newtonsoft.Json has no symbols | **TRUE** - historical decision                     |

**Missing from comparison**:

- Libraries with embedded symbols by design (many smaller libraries)
- Libraries that switched from snupkg to embedded (rare but exists)
- Enterprise-focused libraries (may have different patterns)

The comparison is valid but selectively highlights libraries that support the decision. This is normal for ADRs advocating a position, but worth noting.

## Handoff

| Target          | When                   | Outcome                                                      |
| --------------- | ---------------------- | ------------------------------------------------------------ |
| **implementer** | Now                    | Proceed with v11.0.0 preparation using current configuration |
| **planner**     | If revisions requested | Address documentation issues identified above                |
| **qa**          | Post-release           | Verify debugging experience works as documented              |
