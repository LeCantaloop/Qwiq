---
name: nullable-migration
description: Nullable reference type migration tracking and analysis for QWIQ. Use when measuring CS8xxx warning counts, planning nullable migration work, or tracking progress across projects.
---

# Nullable Migration Skill

## Purpose

This skill helps track and manage the nullable reference type migration in QWIQ. It provides tools to measure current warning counts, identify high-priority projects, and track progress over time.

## When To Use

- Measuring current CS8xxx warning baseline
- Planning nullable migration sprints
- Tracking progress after fixes
- Identifying which projects need the most work
- Generating migration status reports

## Instructions

### 1. Measure Current Baseline

Run the warning counter script:

```powershell
./scripts/Count-NullableWarnings.ps1
```

This will:

- Build each source project with CS8xxx warnings enabled
- Count warnings by type (CS8600, CS8601, etc.)
- Generate a markdown report at `.agents/CS8xxx-baseline.md`

### 2. Interpret Warning Types

| Warning | Meaning                                 | Fix Strategy                       |
| ------- | --------------------------------------- | ---------------------------------- |
| CS8600  | Converting null literal to non-nullable | Add `?` or ensure non-null         |
| CS8601  | Possible null reference assignment      | Add null check or `?`              |
| CS8602  | Dereference of possibly null reference  | Add null check before use          |
| CS8603  | Possible null reference return          | Return `T?` or ensure non-null     |
| CS8604  | Possible null argument                  | Add null check or `?` parameter    |
| CS8618  | Non-nullable field not initialized      | Initialize in constructor          |
| CS8619  | Nullability mismatch in type            | Align interface and implementation |
| CS8625  | Cannot convert null to non-nullable     | Add `?` or ensure non-null         |

### 3. Prioritize Projects

Current migration status (run script for latest):

| Project        | Priority | Reason                    |
| -------------- | -------- | ------------------------- |
| Qwiq.Core      | ✅ Done  | Foundation for all others |
| Qwiq.Core.Rest | High     | Most used client          |
| Qwiq.Identity  | Medium   | Identity resolution       |
| Qwiq.Linq      | Lower    | Complex expression trees  |
| Qwiq.Core.Soap | Lower    | Legacy, Windows-only      |

### 4. Migration Workflow

For each project:

1. **Baseline**: Run `Count-NullableWarnings.ps1`
2. **Analyze**: Review warning types and locations
3. **Plan**: Group related warnings (same class/method)
4. **Fix**: Apply TDD - write test, fix warning, verify test
5. **Verify**: Re-run script to confirm reduction
6. **Commit**: Small, atomic commits per logical fix

## Decision Trees

### Choosing the Right Fix for a Warning

```text
CS8618 (Field not initialized)
├─ Should field ever be null?
│  ├─ YES → Make nullable: `MyType? _field`
│  └─ NO → Can initialize at declaration?
│          ├─ YES → `private readonly MyType _field = new();`
│          └─ NO → Initialize in ALL constructors

CS8602 (Dereference of possibly null)
├─ Is the null state a bug?
│  ├─ YES → Add null check + throw
│  └─ NO → Use null-conditional: `obj?.Property`

CS8604 (Null argument to non-nullable parameter)
├─ Can the value legitimately be null?
│  ├─ YES → Change parameter to nullable
│  └─ NO → Add null guard before call

CS8603 (Null return from non-nullable method)
├─ Can the method legitimately return null?
│  ├─ YES → Change return type to `T?`
│  └─ NO → Fix the code path that returns null
```

### Project Migration Priority

```text
Start Here
    │
    ▼
Is this a core/foundation project?
├─ YES → HIGH PRIORITY (blocks others)
│        Examples: Qwiq.Core
└─ NO → Is it heavily used by other projects?
        ├─ YES → MEDIUM PRIORITY
        │        Examples: Qwiq.Core.Rest, Qwiq.Identity
        └─ NO → LOWER PRIORITY
                Examples: Qwiq.Core.Soap (legacy), Qwiq.Linq (complex)
```

### 5. Custom Report Location

```powershell
./scripts/Count-NullableWarnings.ps1 -ReportPath ./my-report.md
```

## Helper Scripts

### scripts/Count-NullableWarnings.ps1

**Purpose:** Count CS8xxx warnings across all source projects

**Input:** None (scans `src/` directory)

**Output:**

- Console: Warning counts per project with breakdown by code
- File: Markdown report at specified path (default: `.agents/CS8xxx-baseline.md`)

**Usage:**

```powershell
# Default output
./scripts/Count-NullableWarnings.ps1

# Custom output path
./scripts/Count-NullableWarnings.ps1 -ReportPath ./reports/nullable-status.md
```

**Interpretation:**

- ✅ 0 warnings = Project fully migrated
- ⚠️ >0 warnings = Work remaining
- Group by CS8xxx code to identify patterns

## Examples

### Example 1: Starting Migration on a Project

**User goal:** Begin nullable migration on Qwiq.Identity

**Process:**

1. Run `./scripts/Count-NullableWarnings.ps1`
2. Note current count for Qwiq.Identity
3. Open project, find highest-count warning type
4. Fix warnings in one class at a time using TDD
5. Re-run script to verify progress
6. Commit with `fix(identity): add nullable annotations to IdentityService`

**Expected output:** Reduced warning count, tests still passing

### Example 2: Tracking Progress Over Time

**User goal:** Show migration progress to stakeholders

**Process:**

1. Run script weekly, save reports with dates
2. Compare warning counts across reports
3. Calculate percentage reduction per project
4. Identify projects that need more attention

**Expected output:** Trend showing decreasing warnings over time

## Related Resources

- See [../qwiq-csharp/SKILL.md](../qwiq-csharp/SKILL.md) for nullable coding patterns and troubleshooting
- See [../qwiq-testing/SKILL.md](../qwiq-testing/SKILL.md) for TDD requirements (mandatory for refactoring)
- See [../qwiq-build/SKILL.md](../qwiq-build/SKILL.md) for build commands and CI behavior
