# CRAP Score Baseline - Validated

**Generated**: 2025-12-13
**Test Filter**: `TestCategory!=Benchmark` (all other tests included)
**Coverage Tool**: XPlat Code Coverage (Coverlet)

---

## CRAP Formula

```text
CRAP(m) = comp² × (1 - cov)³ + comp
```

Where:

- `comp` = cyclomatic complexity
- `cov` = line coverage (0.0 to 1.0)

**Threshold**: CRAP > 30 = Problematic code

---

## Validated CRAP Scores (Production Code Only)

### Critical Priority (CRAP > 1000)

| Class                       | File                                         | Complexity | Coverage | CRAP Score |
| --------------------------- | -------------------------------------------- | ---------- | -------- | ---------- |
| WorkItemStore (REST)        | src/Qwiq.Core.Rest/WorkItemStore.cs          | 72         | 22.2%    | **2,514**  |
| IFieldDefinition.Extensions | src/Qwiq.Core/IFieldDefinition.Extensions.cs | 46         | 0%       | **2,162**  |
| Query (REST)                | src/Qwiq.Core.Rest/Query.cs                  | 86         | 40.3%    | **1,661**  |
| WorkItem (SOAP)             | src/Qwiq.Core.Soap/WorkItem.cs               | 50         | 14.3%    | **1,623**  |
| LinkCollection (REST)       | src/Qwiq.Core.Rest/LinkCollection.cs         | 32         | 0%       | **1,056**  |

### High Priority (CRAP 500-1000)

| Class           | File                            | Complexity | Coverage | CRAP Score |
| --------------- | ------------------------------- | ---------- | -------- | ---------- |
| WorkItem (REST) | src/Qwiq.Core.Rest/WorkItem.cs  | 38         | 16.1%    | **891**    |
| WorkItemCommon  | src/Qwiq.Core/WorkItemCommon.cs | 36         | 18.9%    | **728**    |
| WorkItem (Core) | src/Qwiq.Core/WorkItem.cs       | 54         | 40.8%    | **661**    |

### Medium Priority (CRAP 100-500)

| Class                 | File                                               | Complexity | Coverage | CRAP Score |
| --------------------- | -------------------------------------------------- | ---------- | -------- | ---------- |
| AuthenticationOptions | src/Qwiq.Core/Credentials/AuthenticationOptions.cs | 43         | 38.5%    | **474**    |
| IdentityFieldValue    | src/Qwiq.Core/IdentityFieldValue.cs                | 80         | 73.8%    | **195**    |
| WorkItemLinkType      | src/Qwiq.Core/WorkItemLinkType.cs                  | 36         | 51.4%    | **185**    |
| TypeParser            | src/Qwiq.Core/TypeParser.cs                        | 95         | 85.3%    | **124**    |
| `GenericComparer<T>`  | src/Qwiq.Core/GenericComparer.cs                   | 53         | 71.8%    | **116**    |

### Low Priority (CRAP 30-100)

| Class                                    | File                                                                 | Complexity | Coverage | CRAP Score |
| ---------------------------------------- | -------------------------------------------------------------------- | ---------- | -------- | ---------- |
| QueryRewriter                            | src/Qwiq.Linq/Visitors/QueryRewriter.cs                              | 84         | 94.5%    | **85**     |
| BulkIdentityAwareAttributeMapperStrategy | src/Qwiq.Mapper.Identity/BulkIdentityAwareAttributeMapperStrategy.cs | 59         | 82.2%    | **79**     |
| `ReadOnlyObjectCollection<T>`            | src/Qwiq.Core/ReadOnlyObjectCollection.cs                            | 37         | 74.1%    | **61**     |
| FieldCollection                          | src/Qwiq.Core/FieldCollection.cs                                     | 42         | 87.0%    | **46**     |

### Acceptable (CRAP ≤ 30 or 100% coverage)

| Class              | File                                | Complexity | Coverage | CRAP Score |
| ------------------ | ----------------------------------- | ---------- | -------- | ---------- |
| IdentityDescriptor | src/Qwiq.Core/IdentityDescriptor.cs | 32         | 100%     | **32**     |
| Comparer           | src/Qwiq.Core/Comparer.cs           | 23         | 100%     | **23**     |
| FieldDefinition    | src/Qwiq.Core/FieldDefinition.cs    | 21         | 100%     | **21**     |
| ExternalLink       | src/Qwiq.Core/ExternalLink.cs       | 26         | 100%     | **26**     |

---

## Summary Statistics

| Metric                   | Value      |
| ------------------------ | ---------- |
| Classes with CRAP > 1000 | **5**      |
| Classes with CRAP > 500  | **8**      |
| Classes with CRAP > 100  | **13**     |
| Classes with CRAP > 30   | **17**     |
| Total CRAP (Top 10)      | **11,965** |
| Average CRAP (Top 10)    | **1,197**  |

---

## Comparison: Original Estimates vs Validated

| Class                         | Original CRAP  | Validated CRAP        | Difference |
| ----------------------------- | -------------- | --------------------- | ---------- |
| IdentityFieldValue            | 6,480 (0% cov) | **195** (73.8% cov)   | -97%       |
| WorkItemStore (REST)          | 5,256 (0% cov) | **2,514** (22.2% cov) | -52%       |
| Query (REST)                  | 3,540 (0% cov) | **1,661** (40.3% cov) | -53%       |
| `GenericComparer<T>`          | 2,862 (0% cov) | **116** (71.8% cov)   | -96%       |
| IFieldDefinition.Extensions   | 2,162 (0% cov) | **2,162** (0% cov)    | 0%         |
| FieldCollection               | 1,806 (0% cov) | **46** (87.0% cov)    | -97%       |
| WorkItem (REST)               | 1,482 (0% cov) | **891** (16.1% cov)   | -40%       |
| `ReadOnlyObjectCollection<T>` | 1,406 (0% cov) | **61** (74.1% cov)    | -96%       |
| IdentityDescriptor            | 1,056 (0% cov) | **32** (100% cov)     | -97%       |
| LinkCollection (REST)         | 1,056 (0% cov) | **1,056** (0% cov)    | 0%         |

**Key Finding**: The independent review was correct. Many classes already have significant coverage. The original estimates assumed 0% coverage but actual coverage is much higher for most classes.

---

## Revised Priority for CRAP Reduction

Based on validated data, the **actual priorities** should be:

### Tier 1: Critical (0% coverage, high complexity)

1. **IFieldDefinition.Extensions** - CRAP 2,162 (0% coverage, easy to test)
2. **LinkCollection (REST)** - CRAP 1,056 (0% coverage, requires WireMock)

### Tier 2: High Impact (Low coverage, high CRAP)

1. **WorkItemStore (REST)** - CRAP 2,514 (22% coverage, complex dependencies)
2. **Query (REST)** - CRAP 1,661 (40% coverage, moderate effort)
3. **WorkItem (REST)** - CRAP 891 (16% coverage, moderate effort)

### Tier 3: Medium Impact (Moderate coverage gaps)

1. **WorkItemCommon** - CRAP 728 (19% coverage)
2. **WorkItem (Core)** - CRAP 661 (41% coverage)
3. **AuthenticationOptions** - CRAP 474 (39% coverage)

### Already Well-Covered (Lower priority)

- IdentityFieldValue (CRAP 195, 74% cov) - Only needs edge case tests
- `GenericComparer<T>` (CRAP 116, 72% cov) - Only needs edge case tests
- TypeParser (CRAP 124, 85% cov) - Minor gaps
- FieldCollection (CRAP 46, 87% cov) - Nearly complete
- IdentityDescriptor (CRAP 32, 100% cov) - Done!

---

## SOAP Client Note

`WorkItem (SOAP)` has CRAP 1,623, but per ADR-010, SOAP client is being deprecated. Do not invest in SOAP coverage improvement.

---

## Recommended Actions

1. **W4.CRAP.3**: IFieldDefinition.Extensions tests (Tier 1, XS effort, 2,162 CRAP reduction)
2. **W4.CRAP.9**: LinkCollection tests (Tier 1, S effort, 1,056 CRAP reduction)
3. **W4.CRAP.6**: WorkItemStore tests (Tier 2, L effort, requires WireMock)
4. **W4.CRAP.7**: Query tests (Tier 2, M effort)
5. **W4.CRAP.8**: WorkItem (REST) tests (Tier 2, M effort)

**Deprioritize**:

- W4.CRAP.1 (IdentityFieldValue) - Already 74% covered
- W4.CRAP.2 (GenericComparer) - Already 72% covered
- W4.CRAP.4 (IdentityDescriptor) - Already 100% covered
- W4.CRAP.5 (ReadOnlyObjectCollection) - Already 74% covered
