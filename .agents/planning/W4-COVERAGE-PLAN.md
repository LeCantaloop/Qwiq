# W4.1 Code Coverage Improvement Plan

**Version:** 1.0 (Multi-Agent Consensus)
**Created:** 2025-12-13
**Status:** APPROVED FOR EXECUTION

## Executive Summary

This plan increases Qwiq test coverage from **51.1% to 70%** with a focus on **behavior-driven testing** rather than coverage inflation. It consolidates input from 5 specialized agents:

- **C# Expert**: Priority-ordered class list, testing patterns
- **Architecture Review**: Infrastructure, file organization, helpers
- **High-Level Advisor**: Strategic guidance, anti-patterns
- **Feature Request Review**: Gap analysis, risk identification
- **Independent Thinker**: Critical review, blind spots

## Key Consensus Points

| Topic             | Consensus                                    |
| ----------------- | -------------------------------------------- |
| Primary focus     | Qwiq.Core (50.9% → 70%)                      |
| Test pattern      | Keep ContextSpecification (BDD style)        |
| REST/SOAP testing | Defer (requires WireMock infrastructure)     |
| Mutation testing  | Add after coverage baseline                  |
| Exception testing | Test behavior, NOT constructors for coverage |

## Coverage Targets by Project

| Project              | Current | Target | Gap    | Priority     |
| -------------------- | ------- | ------ | ------ | ------------ |
| Qwiq.Linq            | 87.1%   | 90%    | +2.9%  | Maintain     |
| Qwiq.Core            | 50.9%   | 70%    | +19.1% | **CRITICAL** |
| Qwiq.Mapper          | 69.5%   | 75%    | +5.5%  | Medium       |
| Qwiq.Identity        | 70.1%   | 70%    | 0%     | Maintain     |
| Qwiq.Mapper.Identity | 66%     | 70%    | +4%    | Low          |

**Note:** REST/SOAP clients deferred to W4.2 (WireMock infrastructure required)

---

## Phase 1: Quick Wins (~8-10% gain)

**Goal:** Test simple, high-value classes with no dependencies

### 1.1 Link Classes (~3.5%)

**Files to create:**

```text
test/Qwiq.Core.Tests/Links/ExternalLinkTests.cs
test/Qwiq.Core.Tests/Links/RelatedLinkTests.cs
```

**Test scenarios:**

- Valid construction
- ArgumentNullException for null parameters
- ArgumentException for reserved link type names
- Equals/GetHashCode semantics

### 1.2 Query Classes (~3%)

**Files to create:**

```text
test/Qwiq.Core.Tests/Query/QueryDefinitionTests.cs
test/Qwiq.Core.Tests/Query/QueryFolderTests.cs
```

**Test scenarios:**

- ArgumentOutOfRangeException for Guid.Empty
- ArgumentException for null/empty name, wiql, path
- ToString format verification
- Equals delegates to comparer

### 1.3 Comparers (~3%)

**Files to create:**

```text
test/Qwiq.Core.Tests/Comparers/QueryDefinitionComparerTests.cs
test/Qwiq.Core.Tests/Comparers/QueryFolderComparerTests.cs
test/Qwiq.Core.Tests/Comparers/WorkItemLinkInfoComparerTests.cs
```

**Standard test matrix per comparer:**

- Same reference → true
- Both null → true
- First null, second not → false
- Equal values → true, hash codes match
- GetHashCode(null) → 0

### Phase 1 Acceptance Criteria

- [ ] All test files created and passing
- [ ] Coverage ≥ 60%
- [ ] Zero flaky tests
- [ ] All tests use ContextSpecification pattern

---

## Phase 2: Core Behavior (~5-7% gain)

### 2.1 Exception Behavior (NOT constructor testing)

**File to update:**

```text
test/Qwiq.Core.Tests/Exceptions/CustomExceptionTests.cs
```

**Test BEHAVIOR, not constructors:**

- Test code paths that THROW exceptions
- Verify exception properties when caught
- Test serialization roundtrip (if applicable)

**Do NOT add:**

- Tests that just instantiate exceptions
- Tests for unused constructors

### 2.2 Field Infrastructure

**Files to create:**

```text
test/Qwiq.Core.Tests/Fields/FieldDefinitionTests.cs
test/Qwiq.Core.Tests/Fields/FieldTests.cs
```

### 2.3 Identity Comparers

**File to create:**

```text
test/Qwiq.Core.Tests/Identity/IdentityDescriptorComparerTests.cs
```

### Phase 2 Acceptance Criteria

- [ ] Coverage ≥ 65%
- [ ] Branch coverage ≥ 55%
- [ ] Exception tests verify throwing behavior

---

## Phase 3: Integration Seams (~3-5% gain)

### 3.1 Credentials (via test seam)

**File to create:**

```text
test/Qwiq.Core.Tests/Credentials/AuthenticationOptionsTests.cs
```

**Approach:** Use existing `credentialsFactory` delegate for injection

### 3.2 Collections

**File to create:**

```text
test/Qwiq.Core.Tests/Collections/ReadOnlyCollectionWithIdTests.cs
```

### Phase 3 Acceptance Criteria

- [ ] Coverage ≥ 68%
- [ ] No live service calls required

---

## Phase 4: Target Achievement

### 4.1 Mapper Edge Cases

**File to create:**

```text
test/Qwiq.Mapper.Tests/FieldMapperEdgeCaseTests.cs
```

### 4.2 Mutation Testing Setup

```powershell
dotnet tool install -g dotnet-stryker
dotnet stryker --project Qwiq.Core.csproj --reporters "['html', 'json']"
```

**Target:** 60% mutation score on comparers (revised from 65%)

### Phase 4 Acceptance Criteria

- [ ] 70% line coverage achieved
- [ ] 60% branch coverage achieved
- [ ] Stryker baseline documented

---

## Anti-Patterns to Avoid

1. **Coverage inflation** - Testing exception constructors adds lines, not quality
2. **Testing implementation** - Test behavior through public APIs
3. **Over-mocking** - Use real objects when practical
4. **Skipping edge cases** - Null, empty, boundary values matter

---

## Test Pattern Reference

```csharp
[TestClass]
public class When_creating_external_link_with_null_uri : ContextSpecification
{
    private Exception? _exception;

    public override void When()
    {
        _exception = Record.Exception(() => new ExternalLink(null!, "Build"));
    }

    [TestMethod]
    public void Then_throws_ArgumentNullException()
    {
        _exception.ShouldBeOfType<ArgumentNullException>();
    }
}
```

---

## Risk Mitigations (from reviews)

| Risk                      | Mitigation                                 |
| ------------------------- | ------------------------------------------ |
| Timeline slip             | Start with quick wins, build momentum      |
| Coverage gaming           | Add mutation testing in Phase 4            |
| REST client complexity    | Defer to W4.2 with WireMock                |
| Fixture maintenance       | Not applicable until W4.2                  |
| ContextSpecification debt | Acknowledge; defer refactoring post-Wave 4 |

---

## Success Metrics

| Metric                   | Current | Target |
| ------------------------ | ------- | ------ |
| Line Coverage            | 51.1%   | 70%    |
| Branch Coverage          | 36.7%   | 60%    |
| Flaky Tests              | 0%      | 0%     |
| Mutation Score (Phase 4) | N/A     | 60%    |

---

## Files Created This Session

Already in progress:

- `test/Qwiq.Core.Tests/Exceptions/CustomExceptionTests.cs` (in git status)
- `test/Qwiq.Core.Tests/Links/HyperlinkTests.cs` (in git status)
- `test/Qwiq.Core.Tests/Extensions/ExtensionsTests.cs` (in git status)
- `test/Qwiq.Core.Tests/Comparers/ComparerTests.cs` (in git status)
- `test/Qwiq.Core.Tests/Collections/CollectionComparerTests.cs` (in git status)
- `test/Qwiq.Core.Tests/WorkItemStore/WorkItem/WorkItemLinkInfoTests.cs` (in git status)
- `test/Qwiq.Core.Tests/WorkItemStore/WorkItemLinkTypeTests.cs` (in git status)

---

## Document History

- 2025-12-13: Initial creation from multi-agent consensus
- Input agents: csharp-expert, csharp-pod, high-level-advisor, feature-request-review, independent-thinker
