# Executive Summary: Debug Symbol Strategy Consensus

**Decision Date**: 2025-12-14  
**Decision**: **Switch to EMBEDDED symbols** for v11.0.0 release  
**Consensus**: 3 of 4 agents (Strong majority)

---

## One-Sentence Summary

For a library with ~1 download/day serving enterprise users often behind firewalls, maintainer simplicity and "just works" debugging outweigh the 200KB package size penalty.

---

## Voting Results

| Agent                   | Vote        | Rationale                                                              |
| ----------------------- | ----------- | ---------------------------------------------------------------------- |
| **Architect**           | EMBEDDED ✓  | Maintainer time > bandwidth at QWIQ's scale                            |
| **DevOps**              | EMBEDDED ✓  | CI/CD simplification saves 10-15 min/release                           |
| **Independent Thinker** | EMBEDDED ✓  | Following Microsoft is cargo culting; make pragmatic decisions         |
| **QA**                  | PORTABLE ⚠️ | Technically superior, but pragmatically unjustified (accepts embedded) |

---

## Why Embedded Wins

### HIGH Impact Factors (Favor Embedded)

✓ **Maintainer Time**: Single package vs dual-package complexity  
✓ **Debugging UX**: "Just works" vs requires symbol server config  
✓ **Enterprise Compat**: No firewall issues vs blocked symbol servers  
✓ **CI/CD Simplicity**: One package type vs two  
✓ **Test Complexity**: Simple validation vs multi-IDE testing

### MEDIUM Impact Factors (Favor Portable)

✗ **Package Size**: 200KB larger per package  
✗ **Industry Alignment**: Deviates from Microsoft approach

### Scale-Adjusted Analysis

- At 1 download/day: 200KB × 365 = ~73MB/year total bandwidth
- This is **background noise** compared to a single Windows update (~1GB)
- Optimizing for 73MB/year when maintainer time is scarce = **premature optimization**

---

## Implementation Checklist

- [ ] Update `Directory.Build.props`: `DebugType=embedded`, remove `SymbolPackageFormat`
- [ ] Update `release.yml`: Remove snupkg push loop
- [ ] Add CI validation: Package size threshold (max 2MB)
- [ ] Update README: Document embedded symbols decision
- [ ] Create ADR-012: Formal decision record
- [ ] Add QA regression test: Package size monitoring

---

## Key Quote

> "Following Microsoft is cargo culting for a library of QWIQ's scale. Make pragmatic decisions for YOUR constraints." - Independent Thinker Agent

---

## Reevaluation Trigger

Reconsider if download count exceeds **100/day** (scaling assumption changes).

---

## Full Documentation

See `.agents/architecture/002-symbols-consensus-recommendation.md` for complete analysis.
