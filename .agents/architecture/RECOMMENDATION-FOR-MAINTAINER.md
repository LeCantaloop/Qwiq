# Debug Symbol Strategy: Final Recommendation

**Date**: December 14, 2025  
**For**: @rjmurillo (QWIQ Maintainer)  
**Subject**: Consensus Recommendation on Embedded vs Portable Debug Symbols

---

## TL;DR

### Recommendation: Switch to embedded symbols

Your instinct about the "pain in the ass factor" is validated. After multi-agent consultation, the consensus is **embedded symbols are the right choice for QWIQ**.

---

## Consensus Results

**Voting Record**: 3 of 4 agents favor embedded (strong majority)

| Agent                   | Vote        | Key Insight                                                |
| ----------------------- | ----------- | ---------------------------------------------------------- |
| **Architect**           | EMBEDDED ✓  | "Maintainer time is the scarce resource, not bandwidth"    |
| **DevOps**              | EMBEDDED ✓  | "CI/CD simplification saves 10-15 minutes per release"     |
| **Independent Thinker** | EMBEDDED ✓  | "Following Microsoft is cargo culting at QWIQ's scale"     |
| **QA**                  | PORTABLE ⚠️ | "Technically best practice, but pragmatically unjustified" |

---

## Why Your Instinct is Right

### 1. Scale Mismatch

- **Microsoft**: Millions of downloads/day → bandwidth costs matter
- **QWIQ**: ~1 download/day → bandwidth is negligible (~73MB/year total)
- Optimizing for 200KB savings when you have limited maintainer time is **premature optimization**

### 2. Enterprise Audience Reality

- QWIQ users are enterprise Azure DevOps/TFS developers
- Disproportionately likely behind corporate firewalls
- Symbol server access may be **blocked by policy**, not choice
- Portable+snupkg optimizes for an access pattern that **may not exist** for your users

### 3. Maintainer Sustainability

- Single maintainer (you) with limited time
- Dual-package publishing has **ongoing cognitive overhead**
- Simpler CI/CD = **more time for features/fixes**
- Project longevity > following "best practices" from different contexts

### 4. "Just Works" Debugging

- Contributors debugging issues get symbols **automatically**
- No onboarding friction ("add symbol server to Visual Studio")
- Consistent experience across VS, Rider, VS Code

---

## What You're NOT Sacrificing

**The "professional standard" argument is flawed**:

- Professional = Making **appropriate decisions for YOUR constraints**
- NOT = Blindly copying Microsoft's patterns from different contexts
- Microsoft optimizes for millions of downloads; you have dozens
- Different constraints → different optimal decisions

**The bandwidth argument is a red herring**:

- 200KB × 365 downloads = **73MB/year** total bandwidth
- This is **background noise** (one Windows update = ~1GB)
- You're not "wasting" user bandwidth at this scale

---

## Implementation Path

To switch to embedded symbols:

**1. Update `Directory.Build.props`**:

```xml
<!-- Symbol Configuration -->
<PropertyGroup Condition=" '$(Configuration)' == 'Release' ">
  <!-- Use embedded symbols for simplicity and enterprise firewall compatibility -->
  <DebugType>embedded</DebugType>
  <Optimize>true</Optimize>
  <DefineConstants>$(DefineConstants);TRACE</DefineConstants>
</PropertyGroup>

<!-- Remove symbol package generation -->
<PropertyGroup>
  <!-- Symbols are embedded in assemblies, no separate .snupkg needed -->
  <IncludeSymbols>false</IncludeSymbols>
  <!-- Remove SymbolPackageFormat property -->
</PropertyGroup>
```

**2. Update `.github/workflows/release.yml`**:

Remove the separate snupkg push loop (keep only nupkg push).

**3. Add CI Validation** (per QA request):

Package size threshold check to prevent unexpected bloat:

```powershell
# In Validate-PackageOutput.ps1
$maxNupkgSize = 2MB
foreach ($nupkg in $nupkgFiles) {
    if ($nupkg.Length -gt $maxNupkgSize) {
        Write-Error "Package exceeds size threshold - investigate bloat"
    }
}
```

**4. Update README.md**:

```markdown
## Debugging

QWIQ packages include embedded debug symbols for seamless debugging.

**Visual Studio, Rider, or VS Code**: Press F11 to step into QWIQ source - no configuration needed.

_Why embedded?_ QWIQ serves enterprise users who may be behind firewalls blocking external symbol servers. Embedded symbols ensure debugging "just works" for everyone.
```

---

## Addressing QA's Concern

QA noted embedded is a **"deliberate tradeoff"** of technical best practices for pragmatism. This is **accurate and acceptable**:

**Tradeoffs are not failures** - they're **informed decisions**:

- You're trading ~200KB package size for simpler maintenance
- You're trading "industry alignment" for "just works" user experience
- You're trading theoretical best practices for practical sustainability

**QA's conditions for acceptance** (all reasonable):

1. ✓ Package size regression test → Prevents accidental bloat
2. ✓ Document decision rationale → ADR-012 will explain the "why"
3. ✓ Monitor user feedback → Watch for complaints post-v11.0.0
4. ✓ Reevaluation trigger → Reconsider if downloads scale 10x (to ~10/day)

---

## When to Reconsider

Monitor and reevaluate if:

- Download count exceeds **100/day** (scaling assumption changes)
- Multiple users complain about package size
- Microsoft publishes specific guidance for low-volume libraries

Until then, embedded is the **right decision** for QWIQ's reality.

---

## Bottom Line

**Your "pain in the ass factor" is a legitimate architectural constraint.**

The multi-agent consensus validates that:

1. Simpler CI/CD matters for single-maintainer projects
2. Enterprise firewall reality matters for QWIQ's audience
3. Scale-appropriate decisions matter more than cargo culting "best practices"
4. Maintainer sustainability matters for project longevity

**Recommendation**: Switch to embedded. Document the decision clearly. Monitor feedback. Don't second-guess yourself.

---

## Full Documentation

- **Executive Summary**: `.agents/architecture/DECISION-SUMMARY-embedded-symbols.md`
- **Complete Analysis**: `.agents/architecture/002-symbols-consensus-recommendation.md` (16KB, all agent perspectives)
- **Session Log**: `.agents/sessions/2025-12-14-debug-symbol-consensus.md`

---

**Next Step**: Route to implementer to execute the changes, or approve for your own implementation.
