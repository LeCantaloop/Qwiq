# Analyzer Debt Inventory

> **Purpose**: Comprehensive inventory of all suppressed analyzer rules in the Qwiq repository
> **Created**: December 5, 2025
> **Last Updated**: December 12, 2025 (Session 28)
> **Current Branch**: `feat/modernize-3` (commit: 767b30f0)
> **Status**: Updated with .NET 10.0 knowledge and production v11.0.0 context

---

## Executive Summary

### Current State (After Session 11)

| Rule | Status | Notes |
|------|--------|-------|
| CS1591 | 🔴 Suppressed | ~4200 violations, XML docs - large effort |
| CS0618 | 🔴 Suppressed | TimeZone obsolete - breaking API change |
| CA1707 | 🔴 Suppressed | 868 violations - test naming pattern |
| CA1716 | 🔴 Suppressed | 78 violations - keyword conflicts, intentional |
| CA1822 | 🔴 Suppressed | 36 violations - API compatibility |
| CA1859 | 🔴 Suppressed | 30 violations - intentional abstraction |
| CA1863 | 🔴 Suppressed | 20 violations - requires .NET 8+ API |
| CA2263 | 🟡 Scoped | Test-specific, scoped to test files |

### Rules Converted to Targeted Suppressions (Session 11)

| Rule | Action | Location |
|------|--------|----------|
| CA1036 | `[SuppressMessage]` | `IdentityDescriptor` class |
| CA1711 | `[SuppressMessage]` | `SaveFlags`, `WorkItemCopyFlags`, `ITfsTeamProjectCollection`, `MockTfsTeamProjectCollection` |
| CA1715 | `[SuppressMessage]` | `IIdentityValueConverter<T, U>` interface |
| CA1720 | `[SuppressMessage]` | `IProject.Guid`, `Project.Guid` properties |
| CA1725 | Fixed | Parameter renamed `id` → `relatedWorkItemId` |

### Rules Enabled via Polyfills (Session 11)

| Rule | Description | Polyfill |
|------|-------------|----------|
| CA1510 | Use `ArgumentNullException.ThrowIfNull` | `ArgumentNullExceptionPolyfill.cs` |
| CA1512 | Use `ArgumentOutOfRangeException.ThrowIfNegative/Zero` | `ArgumentOutOfRangeExceptionPolyfill.cs` |

---

## Historical Baseline (Before Phase 1D)

| Category | Count | Priority | Notes |
|----------|-------|----------|-------|
| Security (CA3xxx-CA5xxx) | 65 | 🔴 Critical | ✅ All enabled (0 violations) |
| Reliability (CA2xxx) | 66 | 🔴 High | Disposal, null checks |
| Performance (CA18xx) | 54 | 🟡 Medium | Allocations, boxing |
| Design (CA1xxx) | 81 | 🟢 Low | API design patterns |
| Naming (CA17xx) | 12 | 🟢 Low | Convention compliance |
| Globalization (CA13xx) | 7 | 🟢 Low | Culture-specific |
| Maintainability (CA15xx) | 11 | 🟡 Medium | Complexity metrics |
| IDE Rules (IDE0xxx) | 105 | 🟢 Low | Style preferences |
| Nullable (CS86xx) | 5 | 🟡 Medium | Remaining debt |
| Other CS | 22 | Various | CLS, XML docs, obsolete |
| SYSLIB | 3 | 🟢 Low | Framework obsoletions |
| **TOTAL** | **403** | | |

---

## Priority Matrix

### 🔴 Critical Priority (131 rules)
- Security rules: 65
- Reliability rules: 66

### 🟡 Medium Priority (70 rules)
- Performance rules: 54
- Maintainability rules: 11
- Nullable warnings: 5

### 🟢 Low Priority (202 rules)
- Design rules: 81
- IDE rules: 105
- Naming rules: 12
- Other rules: 4

---

## Security Rules (CA3xxx-CA5xxx) - 65 Rules

### Priority: 🔴 Critical
**Rationale**: Security vulnerabilities can lead to data breaches, unauthorized access, and system compromise.

### Suppressed Security Rules

- `CA3001`
- `CA3002`
- `CA3003`
- `CA3004`
- `CA3005`
- `CA3006`
- `CA3007`
- `CA3008`
- `CA3009`
- `CA3010`
- `CA3011`
- `CA3012`
- `CA3061`
- `CA3075`
- `CA3076`
- `CA3077`
- `CA3147`
- `CA5350`
- `CA5351`
- `CA5358`
- `CA5359`
- `CA5360`
- `CA5361`
- `CA5362`
- `CA5363`
- `CA5364`
- `CA5365`
- `CA5366`
- `CA5367`
- `CA5368`
- `CA5369`
- `CA5370`
- `CA5371`
- `CA5372`
- `CA5373`
- `CA5374`
- `CA5375`
- `CA5376`
- `CA5377`
- `CA5378`
- `CA5379`
- `CA5380`
- `CA5381`
- `CA5382`
- `CA5383`
- `CA5384`
- `CA5385`
- `CA5386`
- `CA5387`
- `CA5388`
- `CA5389`
- `CA5390`
- `CA5391`
- `CA5392`
- `CA5393`
- `CA5394`
- `CA5395`
- `CA5396`
- `CA5397`
- `CA5398`
- `CA5399`
- `CA5400`
- `CA5401`
- `CA5402`
- `CA5403`

**Common Security Issues**:
- SQL injection vulnerabilities (CA2100)
- Cryptographic weaknesses (CA5xxx)
- Path traversal risks (CA3xxx)
- Authentication/authorization bypasses (CA5xxx)
- XSS vulnerabilities (CA3xxx)

**Recommendation**: Enable ALL security rules immediately and fix violations. No security rule should be permanently suppressed without explicit security review and justification.

---

## Reliability Rules (CA2xxx) - 66 Rules

### Priority: 🔴 High
**Rationale**: Reliability issues cause crashes, data loss, and unpredictable behavior.

### Suppressed Reliability Rules

- `CA2000`
- `CA2002`
- `CA2007`
- `CA2008`
- `CA2009`
- `CA2011`
- `CA2012`
- `CA2013`
- `CA2014`
- `CA2015`
- `CA2016`
- `CA2017`
- `CA2018`
- `CA2100`
- `CA2101`
- `CA2109`
- `CA2119`
- `CA2153`
- `CA2200`
- `CA2201`
- `CA2207`
- `CA2208`
- `CA2211`
- `CA2213`
- `CA2214`
- `CA2215`
- `CA2216`
- `CA2217`
- `CA2219`
- `CA2225`
- `CA2226`
- `CA2227`
- `CA2229`
- `CA2231`
- `CA2234`
- `CA2235`
- `CA2237`
- `CA2241`
- `CA2242`
- `CA2243`
- `CA2244`
- `CA2245`
- `CA2246`
- `CA2247`
- `CA2248`
- `CA2249`
- `CA2250`
- `CA2251`
- `CA2252`
- `CA2253`
- `CA2254`
- `CA2300`
- `CA2301`
- `CA2302`
- `CA2305`
- `CA2310`
- `CA2311`
- `CA2312`
- `CA2315`
- `CA2321`
- `CA2322`
- `CA2326`
- `CA2327`
- `CA2328`
- `CA2329`
- `CA2330`

**Common Reliability Issues**:
- CA1062: Validate arguments of public methods
- CA2000: Dispose objects before losing scope
- CA2007: Consider calling ConfigureAwait
- CA2008: Do not create tasks without passing TaskScheduler
- CA2012: Use ValueTasks correctly
- CA2201: Do not raise reserved exception types
- CA2213: Disposable fields should be disposed
- CA2214: Do not call overridable methods in constructors
- CA2227: Collection properties should be read only

**Recommendation**: Enable reliability rules incrementally, starting with disposal patterns (CA2000, CA2213) and argument validation (CA1062 - pairs with nullable).

---

## Performance Rules (CA18xx) - 54 Rules

### Priority: 🟡 Medium
**Rationale**: Performance issues affect user experience but don't cause failures.

### Suppressed Performance Rules

- `CA1801`
- `CA1802`
- `CA1805`
- `CA1806`
- `CA1810`
- `CA1812`
- `CA1813`
- `CA1814`
- `CA1815`
- `CA1816`
- `CA1819`
- `CA1820`
- `CA1821`
- `CA1822`
- `CA1823`
- `CA1824`
- `CA1825`
- `CA1826`
- `CA1827`
- `CA1828`
- `CA1829`
- `CA1830`
- `CA1831`
- `CA1832`
- `CA1833`
- `CA1834`
- `CA1835`
- `CA1836`
- `CA1837`
- `CA1838`
- `CA1839`
- `CA1840`
- `CA1841`
- `CA1842`
- `CA1843`
- `CA1844`
- `CA1845`
- `CA1846`
- `CA1847`
- `CA1848`
- `CA1849`
- `CA1850`
- `CA1851`
- `CA1852`
- `CA1853`
- `CA1854`
- `CA1855`
- `CA1856`
- `CA1857`
- `CA1858`
- `CA1859`
- `CA1860`
- `CA1861`
- `CA1863`

**Common Performance Issues**:
- CA1812: Avoid uninstantiated internal classes
- CA1822: Mark members as static
- CA1826: Use property instead of Linq Enumerable method
- CA1845: Use span-based string.Concat
- CA1846: Prefer AsSpan over Substring
- CA1852: Seal internal types
- CA1859: Use concrete types when possible for improved performance

**Recommendation**: Enable performance rules selectively, focusing on hot paths and frequently called code. Some rules (like CA1812) may have many false positives in DI scenarios.

---

## Design Rules (CA1xxx excluding CA18xx) - 81 Rules

### Priority: 🟢 Low
**Rationale**: Design rules improve API quality but don't affect runtime behavior.

### Suppressed Design Rules

- `CA1000`
- `CA1001`
- `CA1002`
- `CA1003`
- `CA1008`
- `CA1010`
- `CA1012`
- `CA1014`
- `CA1016`
- `CA1017`
- `CA1018`
- `CA1024`
- `CA1027`
- `CA1028`
- `CA1030`
- `CA1031`
- `CA1032`
- `CA1033`
- `CA1034`
- `CA1036`
- `CA1040`
- `CA1041`
- `CA1043`
- `CA1044`
- `CA1045`
- `CA1046`
- `CA1047`
- `CA1050`
- `CA1051`
- `CA1052`
- `CA1054`
- `CA1055`
- `CA1056`
- `CA1058`
- `CA1060`
- `CA1061`
- `CA1062`
- `CA1063`
- `CA1064`
- `CA1065`
- `CA1066`
- `CA1067`
- `CA1068`
- `CA1069`
- `CA1070`
- `CA1200`
- `CA1303`
- `CA1304`
- `CA1305`
- `CA1307`
- `CA1308`
- `CA1309`
- `CA1310`
- `CA1401`
- `CA1416`
- `CA1417`
- `CA1418`
- `CA1419`
- `CA1501`
- `CA1502`
- `CA1505`
- `CA1506`
- `CA1507`
- `CA1508`
- `CA1509`
- `CA1510`
- `CA1511`
- `CA1512`
- `CA1513`
- `CA1700`
- `CA1707`
- `CA1708`
- `CA1710`
- `CA1711`
- `CA1712`
- `CA1715`
- `CA1716`
- `CA1720`
- `CA1721`
- `CA1724`
- `CA1725`

**Common Design Issues**:
- CA1008: Enums should have zero value
- CA1010: Collections should implement generic interface
- CA1031: Do not catch general exception types
- CA1032: Implement standard exception constructors
- CA1054: Uri parameters should not be strings
- CA1056: Uri properties should not be strings
- CA1062: Validate arguments of public methods (also in Reliability)

**Recommendation**: Enable design rules after security and reliability are addressed. Many can be fixed with automated refactorings.

---

## Naming Rules (CA17xx) - 12 Rules

### Priority: 🟢 Low
**Rationale**: Naming conventions improve code readability but don't affect functionality.

### Suppressed Naming Rules

- `CA1700`
- `CA1707`
- `CA1708`
- `CA1710`
- `CA1711`
- `CA1712`
- `CA1715`
- `CA1716`
- `CA1720`
- `CA1721`
- `CA1724`
- `CA1725`

**Common Naming Issues**:
- CA1700: Do not name enum values 'Reserved'
- CA1707: Identifiers should not contain underscores
- CA1708: Identifiers should differ by more than case
- CA1710: Identifiers should have correct suffix
- CA1711: Identifiers should not have incorrect suffix
- CA1715: Identifiers should have correct prefix
- CA1716: Identifiers should not match keywords

**Recommendation**: Low priority. Enable after all functionality rules are addressed. Some may be intentional deviations from conventions.

---

## Globalization Rules (CA13xx) - 7 Rules

### Priority: �� Low
**Rationale**: Globalization rules ensure international compatibility but most are low risk.

### Suppressed Globalization Rules

- `CA1303`
- `CA1304`
- `CA1305`
- `CA1307`
- `CA1308`
- `CA1309`
- `CA1310`

**Common Globalization Issues**:
- CA1303: Do not pass literals as localized parameters
- CA1304: Specify CultureInfo
- CA1305: Specify IFormatProvider
- CA1307: Specify StringComparison
- CA1308: Normalize strings to uppercase
- CA1309: Use ordinal string comparison
- CA1310: Specify StringComparison for correctness

**Recommendation**: Enable CA1307, CA1309, CA1310 (string comparison) as they prevent bugs. CA1303-CA1305 (localization) are optional unless internationalization is required.

---

## Maintainability Rules (CA15xx) - 11 Rules

### Priority: 🟡 Medium
**Rationale**: Maintainability rules prevent code from becoming too complex.

### Suppressed Maintainability Rules

- `CA1501`
- `CA1502`
- `CA1505`
- `CA1506`
- `CA1507`
- `CA1508`
- `CA1509`
- `CA1510`
- `CA1511`
- `CA1512`
- `CA1513`

**Common Maintainability Issues**:
- CA1501: Avoid excessive inheritance
- CA1502: Avoid excessive complexity
- CA1505: Avoid unmaintainable code
- CA1506: Avoid excessive class coupling
- CA1507: Use nameof instead of string
- CA1508: Avoid dead conditional code
- CA1509: Invalid entry in code metrics configuration

**Recommendation**: Enable CA1507 (use nameof) as it's easy to fix. CA1502/CA1505/CA1506 (complexity) should be evaluated per case.

---

## IDE Rules (IDE0xxx) - 105 Rules

### Priority: 🟢 Low
**Rationale**: IDE rules are code style preferences that don't affect functionality.

### Suppressed IDE Rules (First 20)

- `IDE0001`
- `IDE0002`
- `IDE0003`
- `IDE0004`
- `IDE0005`
- `IDE0007`
- `IDE0008`
- `IDE0009`
- `IDE0010`
- `IDE0011`
- `IDE0016`
- `IDE0017`
- `IDE0018`
- `IDE0019`
- `IDE0020`
- `IDE0021`
- `IDE0022`
- `IDE0023`
- `IDE0024`
- `IDE0025`

*... and 85 more IDE rules*

**Common IDE Issues**:
- Code simplification suggestions
- Expression body preferences
- Pattern matching suggestions
- Modern C# feature usage
- var vs explicit type

**Recommendation**: Very low priority. These are style preferences and many may be intentional deviations. Enable only if team wants to enforce specific C# idioms.

---

## Nullable Reference Type Warnings (CS86xx) - 5 Rules

### Priority: 🟡 Medium
**Rationale**: Remaining nullable debt after Phase 1C completion.

### Suppressed Nullable Rules

- `CS8605`
- `CS8618`
- `CS8619`
- `CS8620`
- `CS8629`

**Context**: Phase 1C (W1.9-W1.14) was completed via PR #52, which eliminated all CS8xxx warnings. These 5 suppressions remain as a safety net.

**Rules**:
- CS8605: Unboxing a possibly null value
- CS8618: Non-nullable field must contain non-null value when exiting constructor
- CS8619: Nullability of reference types in value doesn't match target type
- CS8620: Argument cannot be used for parameter due to differences in nullability
- CS8629: Nullable value type may be null
- CS8764-CS8769: Various nullability mismatches

**Recommendation**: Keep these suppressions as a safety net for now. Re-evaluate removal in Wave 2 after codebase stabilizes.

---

## Other CS Rules - 22 Rules

### Priority: Various
**Rationale**: Mix of documentation, obsolescence, and CLS compliance warnings.

### Suppressed Other CS Rules

- `CS0618`
- `CS1574`
- `CS1591`
- `CS1710`
- `CS3001`
- `CS3002`
- `CS3003`
- `CS3005`
- `CS3006`
- `CS3008`
- `CS3009`
- `CS3014`
- `CS3015`
- `CS3016`
- `CS3024`
- `CS3026`
- `CS3027`
- `CS8764`
- `CS8765`
- `CS8766`
- `CS8767`
- `CS8769`

**Categories**:
- **CS1591, CS1574, CS1710**: XML documentation warnings (3 rules)
- **CS0618**: Obsolete API usage (1 rule)
- **CS3xxx**: CLS compliance warnings (13 rules)

**CLS Compliance Rules**:
CLS compliance warnings are suppressed because the library targets non-CLS-compliant APIs (TFS Client OM). These can remain suppressed indefinitely.

**Documentation Rules**:
XML documentation is incomplete. Low priority unless generating API docs.

**Obsolete APIs**:
CS0618 should be reviewed - may indicate use of deprecated APIs that need migration.

---

## SYSLIB Rules - 3 Rules

### Priority: 🟢 Low
**Rationale**: .NET platform obsoletions that may require significant refactoring.

### Suppressed SYSLIB Rules

- `SYSLIB0021`
- `SYSLIB0050`
- `SYSLIB0051`

**Rules**:
- SYSLIB0021: MD5CryptoServiceProvider is obsolete
- SYSLIB0050: Formatter-based serialization is obsolete
- SYSLIB0051: Legacy serialization support is obsolete

**Context**: These are .NET framework obsoletions that may require significant refactoring:
- MD5 usage should be reviewed for security implications
- Serialization changes may require BinaryFormatter migration

**Recommendation**: Evaluate in Wave 2 or later. May require breaking changes.

---

## Phased Enablement Plan

### Phase 1D (Current - W1.15-W1.18)

**Week 1: W1.15 - Inventory** ✅
- [x] Create this inventory
- [x] Categorize all rules by priority
- [x] Document recommendations

**Week 2: W1.16 - Security Rules** (Next)
- [ ] Enable CA3xxx-CA5xxx rules
- [ ] Review each violation
- [ ] Fix or justify suppression with comment
- [ ] Zero unsuppressed security violations

**Week 3: W1.17 - Reliability Rules**
- [ ] Enable high-priority CA2xxx rules:
  - CA1062 (argument validation - pairs with nullable)
  - CA2000 (dispose objects)
  - CA2007 (ConfigureAwait)
  - CA2213 (dispose fields)
- [ ] Fix violations incrementally
- [ ] Document justified suppressions

**Week 4: W1.18 - Performance Rules**
- [ ] Enable high-impact CA18xx rules:
  - CA1812 (uninstantiated classes - review DI scenarios)
  - CA1822 (mark static)
  - CA1826 (property vs LINQ)
  - CA1852 (seal types)
- [ ] Fix low-hanging fruit
- [ ] Document performance tradeoffs

### Phase 1E (W1.19-W1.22)
- Quality gates and testing documentation
- No additional analyzer enablement

### Wave 2 (Future)
- Design rules (CA1xxx)
- Globalization rules (CA13xx)
- Maintainability rules (CA15xx)
- IDE style rules (IDE0xxx)
- SYSLIB obsoletions

---

## Approach: One Rule at a Time

**Process for Enabling Rules**:

1. **Select One Rule**
   ```bash
   # Example: Enable CA2000
   # Change in .editorconfig: severity = none → severity = warning
   ```

2. **Build and Identify Violations**
   ```bash
   dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
   # Review all CA2000 warnings
   ```

3. **Fix or Suppress with Justification**
   ```csharp
   // Option A: Fix the violation
   using (var disposable = new DisposableResource())
   {
       // use resource
   }

   // Option B: Suppress with clear justification
   #pragma warning disable CA2000 // Disposed by container via DI
   var service = new Service();
   container.Register(service);
   #pragma warning restore CA2000
   ```

4. **Verify Zero Warnings**
   ```bash
   dotnet build Qwiq.sln -c Release | grep CA2000
   # Should return nothing
   ```

5. **Commit Atomically**
   ```bash
   git commit -m "chore(analyzers): enable CA2000 disposal rule"
   ```

6. **Repeat for Next Rule**

**Key Principles**:
- ONE rule at a time
- NEVER commit with active warnings for enabled rules
- ALWAYS document why a suppression is justified
- PREFER fixing over suppressing

---

## Success Metrics

### Definition of Done (Phase 1D)

- [x] **W1.15**: Complete inventory created
- [ ] **W1.16**: All security rules (65) reviewed, enabled where safe
- [ ] **W1.17**: High-priority reliability rules enabled (CA1062, CA2000, CA2007, CA2213)
- [ ] **W1.18**: High-impact performance rules enabled (CA1812, CA1822, CA1826, CA1852)
- [ ] Zero unsuppressed warnings for enabled rules
- [ ] All suppressions have inline justifications
- [ ] Documentation updated with decisions

### Target Reduction

| Category | Baseline | Target (Phase 1D) | Target (Wave 2) |
|----------|----------|-------------------|-----------------|
| Security | 65 | 0 unsuppressed | 0 unsuppressed |
| Reliability | 66 | ~60 (enable 6) | ~50 |
| Performance | 54 | ~50 (enable 4) | ~40 |
| Design | 81 | 81 (no change) | ~60 |
| Other | 165 | 165 (no change) | ~140 |
| **Total** | **403** | **~356** | **~290** |

**Note**: Target is to enable ~47 rules in Phase 1D (all security + 10 reliability/performance), reducing suppressions by ~12%.

---

## Document Control

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | Dec 5, 2025 | Copilot Agent | Initial inventory for W1.15 |

---

## Related Documents

- [modernize-TODO.md](./modernize-TODO.md) - Master task list
- [modernize-explainer.md](./modernize-explainer.md) - Context and rationale
- [copilot-instructions.md](../copilot-instructions.md) - Repository guidelines
- `.editorconfig` - Current analyzer suppressions
