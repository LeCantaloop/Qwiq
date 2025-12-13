# Wave 4: CRAP Score Reduction Plan

**Status**: Draft
**Owner**: Engineering Team
**Created**: 2025-12-13
**Related**: [WAVE4-TEST-IMPROVEMENT-PLAN.md](WAVE4-TEST-IMPROVEMENT-PLAN.md), [WAVE4-TASKS.md](../.agents/WAVE4-TASKS.md)

---

## Executive Summary

This plan targets **CRAP (Change Risk Anti-Patterns) score reduction** as a key quality metric for Wave 4. CRAP scores identify code that is both complex and poorly tested - the most risky code in the codebase.

### CRAP Formula

```text
CRAP(m) = comp(m)² × (1 - cov(m))³ + comp(m)
```

Where:

- `comp(m)` = cyclomatic complexity of a method
- `cov(m)` = code coverage percentage (0.0 to 1.0)

### Key Threshold

**CRAP > 30 = Problematic code** that needs either:

1. More test coverage, OR
2. Reduced complexity, OR
3. Both

### Impact of Coverage on CRAP

| Coverage | Multiplier on comp² | Example (comp=30) |
| -------- | ------------------- | ----------------- |
| 0%       | 1.000               | CRAP = 930        |
| 50%      | 0.125               | CRAP = 142        |
| 80%      | 0.008               | CRAP = 37         |
| 90%      | 0.001               | CRAP = 31         |
| 100%     | 0.000               | CRAP = 30         |

**Key Insight**: The cubic term `(1-cov)³` means early coverage gains have enormous impact. Moving from 0% to 50% coverage reduces CRAP by ~87%.

---

## Current State Analysis

### High-CRAP Classes (All at 0% Coverage)

| Rank | Class                           | File                                         | Complexity | CRAP Score | Status   |
| ---- | ------------------------------- | -------------------------------------------- | ---------- | ---------- | -------- |
| 1    | `IdentityFieldValue`            | src/Qwiq.Core/IdentityFieldValue.cs          | 80         | **6,480**  | Critical |
| 2    | `WorkItemStore` (REST)          | src/Qwiq.Core.Rest/WorkItemStore.cs          | 72         | **5,256**  | Critical |
| 3    | `Query` (REST)                  | src/Qwiq.Core.Rest/Query.cs                  | 59         | **3,540**  | Critical |
| 4    | GenericComparer\<T\>            | src/Qwiq.Core/GenericComparer.cs             | 53         | **2,862**  | High     |
| 5    | `Extensions` (IFieldDefinition) | src/Qwiq.Core/IFieldDefinition.Extensions.cs | 46         | **2,162**  | High     |
| 6    | `FieldCollection`               | src/Qwiq.Core/FieldCollection.cs             | 42         | **1,806**  | High     |
| 7    | `WorkItem` (REST)               | src/Qwiq.Core.Rest/WorkItem.cs               | 38         | **1,482**  | Medium   |
| 8    | ReadOnlyObjectCollection\<T\>   | src/Qwiq.Core/ReadOnlyObjectCollection.cs    | 37         | **1,406**  | Medium   |
| 9    | `IdentityDescriptor`            | src/Qwiq.Core/IdentityDescriptor.cs          | 32         | **1,056**  | Medium   |
| 10   | `LinkCollection` (REST)         | src/Qwiq.Core.Rest/LinkCollection.cs         | 32         | **1,056**  | Medium   |

**Total CRAP Score (Top 10)**: 27,106
**Classes with CRAP > 30**: All 10 (100%)

### Package-Level Complexity

| Package          | Total Complexity | Coverage | Risk Level   |
| ---------------- | ---------------- | -------- | ------------ |
| Qwiq.Core        | 1,548            | ~51%     | High         |
| Qwiq.Client.Rest | 401              | 0%       | **Critical** |
| Qwiq.Linq        | ~200             | 90.9%    | Low          |
| Qwiq.Mapper      | ~150             | 73.9%    | Medium       |

---

## CRAP Reduction Strategy

### Approach 1: Increase Test Coverage (Primary)

For most high-CRAP classes, adding tests is more efficient than refactoring:

| Class                       | Current Coverage | Target Coverage | CRAP Before | CRAP After | Reduction |
| --------------------------- | ---------------- | --------------- | ----------- | ---------- | --------- |
| IdentityFieldValue          | 0%               | 80%             | 6,480       | 533        | 92%       |
| GenericComparer\<T\>        | 0%               | 85%             | 2,862       | 362        | 87%       |
| IFieldDefinition.Extensions | 0%               | 100%            | 2,162       | 46         | 98%       |
| IdentityDescriptor          | 0%               | 90%             | 1,056       | 96         | 91%       |

### Approach 2: Reduce Complexity (Where Warranted)

Some methods have excessive complexity that should be refactored:

| Class         | Method       | Complexity | Recommendation                |
| ------------- | ------------ | ---------- | ----------------------------- |
| WorkItemStore | GetLinks     | 30         | Extract to `ILinkTypeBuilder` |
| Query         | FetchResults | 20+        | Extract to `IWorkItemFetcher` |
| Query         | ExtractAsOf  | 15         | Extract to `IWiqlParser`      |

### Approach 3: Combined Strategy (Best for REST Layer)

The REST client layer (0% coverage, 401 total complexity) requires both:

1. **Testability improvements** (extract interfaces for DI)
2. **WireMock-based integration tests**

---

## Prioritized Implementation Plan

### Phase 1: Quick Wins - Pure Unit Tests (Week 1)

**Target**: Classes with no external dependencies, testable with existing mocks.

#### Task W4.CRAP.1: IdentityFieldValue Tests

- **CRAP Impact**: 6,480 → 533 (92% reduction)
- **Effort**: S (3-4 hours)
- **Tests to Add**: 15-20 tests covering:
  - Constructor with `ITeamFoundationIdentity`
  - VSID format parsing: `[Scope]\Name<id:GUID>`
  - Domain account format: `Name <DOMAIN\user>`
  - UPN format: `Name <user@domain.com>`
  - Null/empty handling
  - Implicit string conversion

#### Task W4.CRAP.2: GenericComparer\<T\> Tests

- **CRAP Impact**: 2,862 → 362 (87% reduction)
- **Effort**: M (4-5 hours)
- **Tests to Add**: 20-25 tests covering:
  - IEnumerable comparison (equal, different length, different content)
  - Nullable\<T\> handling
  - IComparable\<T\> implementations
  - IComparable (non-generic)
  - IEquatable\<T\> implementations
  - Equals/GetHashCode consistency

#### Task W4.CRAP.3: IFieldDefinition.Extensions Tests

- **CRAP Impact**: 2,162 → 46 (98% reduction)
- **Effort**: XS (2 hours)
- **Tests to Add**: 12-15 tests covering:
  - IsCloneable for each CoreField type
  - IsEditable for various field types
  - IsComputed field identification

#### Task W4.CRAP.4: IdentityDescriptor Tests

- **CRAP Impact**: 1,056 → 96 (91% reduction)
- **Effort**: S (3 hours)
- **Tests to Add**: 12-15 tests covering:
  - Constructor validation (null, empty, max length)
  - IdentityType mapping
  - CompareTo implementation
  - Equals/GetHashCode

#### Task W4.CRAP.5: ReadOnlyObjectCollection\<T\> Tests

- **CRAP Impact**: 1,406 → 231 (84% reduction)
- **Effort**: M (4-5 hours)
- **Tests to Add**: 15-18 tests covering:
  - Construction variations (Func, IList, IEnumerable)
  - Lazy loading behavior
  - Indexer bounds checking
  - Contains/IndexOf with GenericComparer

**Phase 1 Total**: ~75 tests, CRAP reduction from 13,966 to 1,268 (91%)

---

### Phase 2: WireMock Integration Tests (Weeks 2-3)

**Target**: REST client classes requiring HTTP mocking.

#### Task W4.CRAP.6: WorkItemStore (REST) Tests

- **CRAP Impact**: 5,256 → 1,734 (67% reduction)
- **Effort**: L (8-10 hours)
- **Dependencies**: WireMock infrastructure (W4.11-W4.13)
- **Tests to Add**: 20-25 tests covering:
  - Query methods (WIQL, IDs, single ID)
  - QueryLinks execution
  - GetLinks for various link types
  - Property lazy loading (FieldDefinitions, Projects)
  - Dispose pattern

#### Task W4.CRAP.7: Query (REST) Tests

- **CRAP Impact**: 3,540 → 1,330 (62% reduction)
- **Effort**: M (6-7 hours)
- **Tests to Add**: 18-22 tests covering:
  - RunQuery with IDs vs WIQL
  - ExtractAsOf date parsing
  - FetchResults pagination
  - LookUpWorkItemType resolution

#### Task W4.CRAP.8: WorkItem (REST) Tests

- **CRAP Impact**: 1,482 → 306 (79% reduction)
- **Effort**: M (4-5 hours)
- **Tests to Add**: 12-15 tests covering:
  - Link count properties (Attached, External, Hyperlink, Related)
  - Fields lazy initialization
  - GetValue/SetValue operations

#### Task W4.CRAP.9: LinkCollection (REST) Tests

- **CRAP Impact**: 1,056 → 166 (84% reduction)
- **Effort**: S (3-4 hours)
- **Tests to Add**: 10-12 tests covering:
  - Constructor with various link types
  - ICollection implementation
  - NotSupported methods

**Phase 2 Total**: ~65 tests, CRAP reduction from 11,334 to 3,536 (69%)

---

### Phase 3: Refactoring for Testability (Weeks 4-5)

**Target**: Extract abstractions to enable unit testing of complex logic.

#### Task W4.CRAP.10: Extract IWiqlParser

- **File**: src/Qwiq.Core/Parsing/IWiqlParser.cs
- **Purpose**: Isolate ASOF extraction and query analysis
- **Methods**:
  - `DateTime? ExtractAsOf(string wiql)`
  - `bool IsLinkQuery(string wiql)`
  - `IEnumerable<string> ExtractFields(string wiql)`

#### Task W4.CRAP.11: Extract ILinkTypeBuilder

- **File**: src/Qwiq.Core/Links/ILinkTypeBuilder.cs
- **Purpose**: Isolate GetLinks complexity from WorkItemStore
- **Methods**:
  - `IWorkItemLinkTypeCollection Build(IEnumerable<WorkItemRelationType> types)`

#### Task W4.CRAP.12: Add InternalsVisibleTo

- **File**: src/Qwiq.Core.Rest/Qwiq.Core.Rest.csproj
- **Purpose**: Enable testing of internal classes without making them public
- **Target Assembly**: Qwiq.Core.Tests

---

## Success Metrics

### Primary CRAP Metrics

| Metric                   | Baseline | Target | Verification    |
| ------------------------ | -------- | ------ | --------------- |
| Classes with CRAP > 1000 | 10       | 3      | Coverage report |
| Classes with CRAP > 30   | 10+      | <5     | Coverage report |
| Average CRAP (Top 10)    | 2,711    | <500   | Coverage report |
| Total CRAP (Top 10)      | 27,106   | <5,000 | Coverage report |

### Coverage Improvement (Required for CRAP Reduction)

| Package          | Current | Target | Impact                    |
| ---------------- | ------- | ------ | ------------------------- |
| Qwiq.Core        | 51%     | 75%    | Reduces core CRAP by 80%+ |
| Qwiq.Client.Rest | 0%      | 60%    | Eliminates critical risk  |
| Overall          | 51.1%   | 70%    | Wave 4 target             |

---

## ROI Analysis

### Highest ROI (CRAP Reduction per Test Written)

| Class                       | Tests Needed | CRAP Reduction | ROI (reduction/test) |
| --------------------------- | ------------ | -------------- | -------------------- |
| IFieldDefinition.Extensions | 12           | 2,116          | **176**              |
| IdentityFieldValue          | 17           | 5,947          | **350**              |
| WorkItemStore (REST)        | 22           | 3,522          | **160**              |
| GenericComparer\<T\>        | 22           | 2,500          | 114                  |
| IdentityDescriptor          | 14           | 960            | 69                   |

### Effort vs Impact Matrix

```text
                    HIGH IMPACT
                        |
    IFieldDef.Ext   IdentityFieldValue
    (XS effort)     (S effort)
                        |
LOW EFFORT ─────────────┼───────────── HIGH EFFORT
                        |
    IdentityDescriptor  WorkItemStore
    (S effort)          (L effort)
                        |
                    LOW IMPACT
```

---

## Implementation Recommendations

### From C# Expert Analysis

1. **IdentityFieldValue**: Highly testable, pure parsing logic - no refactoring needed
2. **GenericComparer\<T\>**: All paths testable with parameterized tests
3. **WorkItemStore**: Extract `ILinkTypeBuilder` to reduce GetLinks complexity
4. **Query**: Add `InternalsVisibleTo` or extract `IWiqlParser`

### From Architecture Review

1. **Add `InternalsVisibleTo`** for REST assembly to enable internal class testing
2. **Extract `IWorkItemTrackingClient`** abstraction for SDK independence
3. **Use Builder pattern** for mock setup (`MockWorkItemStoreBuilder`)
4. **Strategy pattern** for eager/lazy loading in Query

### Bug Fix Required

**RelatedLinkCount in REST WorkItem** (src/Qwiq.Core.Rest/WorkItem.cs):

```csharp
// BUG: Uses ExternalLinkCount field instead of RelatedLinkCount
var fv = GetValue<int?>(CoreFieldRefNames.ExternalLinkCount);
```

This should be fixed and a regression test added.

---

## Integration with Wave 4

This CRAP reduction plan integrates with the existing Wave 4 phases:

| Wave 4 Phase         | CRAP Tasks                         |
| -------------------- | ---------------------------------- |
| Phase 1 (Weeks 1-2)  | W4.CRAP.1-5 (Quick Wins)           |
| Phase 2 (Weeks 3-6)  | W4.CRAP.6-9 (WireMock Tests)       |
| Phase 3 (Weeks 7-8)  | W4.CRAP.10-12 (Refactoring)        |
| Phase 4 (Weeks 9-12) | Mutation testing on improved areas |

### Updated Wave 4 Success Metrics

| Metric                    | Original Target | Updated Target |
| ------------------------- | --------------- | -------------- |
| Line Coverage             | 70%             | 70%            |
| Mutation Score            | 65%             | 65%            |
| **CRAP > 30 Classes**     | (new)           | <5             |
| **Average CRAP (Top 10)** | (new)           | <500           |

---

## References

- [CRAP Metric - Google Testing Blog](https://testing.googleblog.com/2011/02/this-code-is-crap.html)
- [Wave 4 Task List](WAVE4-TASKS.md)
- [Wave 4 Test Improvement Plan](WAVE4-TEST-IMPROVEMENT-PLAN.md)
- [ADR-010: SOAP Deprecation](../docs/adr/ADR-010-soap-client-deprecation-strategy.md)

---

## Independent Review Findings

The plan was reviewed by an independent-thinker agent. Key findings:

### Critical Issue: Baseline Validation Required

> "The plan's foundation is built on incorrect data... Some classes may already have significant coverage."

**Action**: Added W4.CRAP.0 task to regenerate and validate actual CRAP scores before proceeding.

### Recommendations Incorporated

1. **Verify baselines** - Must regenerate coverage reports before starting
2. **InternalsVisibleTo first** - Added W4.CRAP.12 as a blocker for GenericComparer tests
3. **Interleave refactoring** - Consider extracting interfaces earlier, not just Phase 3
4. **Contract tests for mocks** - Verify mock fidelity matches production behavior

### Risks Identified

| Risk                         | Mitigation                                           |
| ---------------------------- | ---------------------------------------------------- |
| Baseline data incorrect      | W4.CRAP.0 validates before work begins               |
| REST classes internal        | W4.CRAP.12 adds InternalsVisibleTo                   |
| Mock fidelity unknown        | Add contract tests (see docs/CONTRACT-TESTS-PLAN.md) |
| Thread safety in collections | Add concurrency tests for ReadOnlyObjectCollection   |

### Adjusted Expectations

- Phase 1 CRAP reduction may be 40-60% (not 91%) after baseline validation
- Some classes may already be well-covered, changing priorities
- Timeline remains 5 weeks but scope may shift based on verified data

---

## Approval

**Prepared by**: Claude Code (Session 31)
**Reviewed by**: Independent-thinker agent (2025-12-13)
**Approved by**: _Pending_
**Date**: 2025-12-13
