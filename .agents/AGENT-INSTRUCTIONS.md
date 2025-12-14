# Agent Instructions for Qwiq Development

> **CRITICAL**: Read this entire document before starting ANY work.
> This document governs how agents execute development tasks.

---

## Agent System Overview

This repository uses a coordinated multi-agent system. See `AGENT-SYSTEM.md` for:

- Full agent catalog and capabilities
- Workflow patterns and routing heuristics
- Memory system using `cloudmcp-manager`
- Handoff protocols and conflict resolution

**Quick Reference - Common Agents:**

| Agent          | Use When                               |
| -------------- | -------------------------------------- |
| `orchestrator` | Complex multi-step tasks               |
| `implementer`  | Writing C# code and tests              |
| `analyst`      | Research and investigation             |
| `architect`    | Design decisions and ADRs              |
| `planner`      | Breaking down work                     |
| `critic`       | Validating plans before implementation |
| `qa`           | Test strategy and verification         |

---

## Quick Start Checklist

Before starting work, complete these steps IN ORDER:

- [ ] Read this file completely
- [ ] Read `planning/modernize-TODO-index.md` for overview and navigation
- [ ] Read the appropriate wave file for your assigned tasks:
  - `planning/modernize-wave1.md` - Wave 0-1 tasks
  - `planning/modernize-wave2.md` - Wave 2 tasks
  - `planning/modernize-wave3-5.md` - Waves 3-5 tasks
- [ ] Read `planning/modernize-explainer.md` for architectural context
- [ ] Check `HANDOFF.md` for previous session notes
- [ ] Identify your assigned phase (e.g., "Phase 2A")
- [ ] Create a session log file: `.agents/sessions/YYYY-MM-DD-phase-XX.md`

---

## Document Hierarchy

| Document                           | Purpose                             | When to Update                   |
| ---------------------------------- | ----------------------------------- | -------------------------------- |
| `AGENT-INSTRUCTIONS.md`            | How to execute work (this file)     | Rarely - only if process changes |
| `planning/modernize-TODO-index.md` | Overview, metrics, session log      | After EVERY session              |
| `planning/modernize-wave1.md`      | Wave 0-1 task tracking              | After Wave 0-1 task completion   |
| `planning/modernize-wave2.md`      | Wave 2 task tracking                | After Wave 2 task completion     |
| `planning/modernize-wave3-5.md`    | Waves 3-5 task tracking             | After Wave 3-5 task completion   |
| `planning/modernize-explainer.md`  | Architecture, decisions, rationale  | When design decisions are made   |
| `HANDOFF.md`                       | Session-to-session context transfer | At END of every session          |
| `sessions/*.md`                    | Detailed session logs               | Throughout session               |

---

## Phase Execution Protocol

### 1. Session Initialization (MANDATORY)

```markdown
## Session Start Checklist

- [ ] Created session log: `.agents/sessions/YYYY-MM-DD-phase-XX.md`
- [ ] Read HANDOFF.md from previous session
- [ ] Identified all tasks in assigned phase
- [ ] Verified CI build passes: `dotnet build Qwiq.sln -c Release /p:ContinuousIntegrationBuild=true /p:UseSharedCompilation=false /m:1 /nodeReuse:false`
- [ ] Verified tests pass: `dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"`
- [ ] Noted starting git state: `git status`
```

### 2. Task Execution (FOR EACH TASK)

**Before starting a task:**

1. Read the full task description in the appropriate wave file:
   - `planning/modernize-wave1.md` - Wave 0-1 tasks
   - `planning/modernize-wave2.md` - Wave 2 tasks
   - `planning/modernize-wave3-5.md` - Waves 3-5 tasks
2. Understand acceptance criteria
3. Plan the implementation approach

**During task execution:**

1. Work incrementally - small, atomic changes
2. Commit frequently with conventional commit messages
3. Run `dotnet format` after code changes
4. Run build after each significant change
5. Run tests after each significant change

**After completing a task:**

1. ✅ Check off the task in the appropriate wave file
2. Update session log with:
   - What was done
   - Decisions made and why
   - Challenges encountered
   - How challenges were resolved
3. Commit the documentation update

### 3. Session Finalization (MANDATORY)

**Before ending ANY session, you MUST:**

```markdown
## Session End Checklist

- [ ] All assigned tasks checked off in the appropriate wave file
- [ ] Session log complete with all details
- [ ] HANDOFF.md updated with:
  - [ ] What was completed
  - [ ] What's next
  - [ ] Any blockers or concerns
  - [ ] Commands to verify state
- [ ] Linting passes (run autofix before committing):
  - [ ] `npx markdownlint-cli2 --fix "**/*.md"` - Fix markdown issues
  - [ ] `dotnet format` - Fix C# formatting
  - [ ] `dotnet pprettier --write .` - Fix general formatting
- [ ] All files committed (including .agents/ files)
- [ ] Build passes
- [ ] Tests pass
- [ ] Git status is clean (or intentionally dirty with explanation)
```

---

## Commit Message Format

Use conventional commits:

```text
<type>(<scope>): <short description>

<optional body with details>

<optional footer with references>
```

**Types:**

- `feat` - New feature
- `fix` - Bug fix
- `docs` - Documentation only
- `chore` - Maintenance (CI, build, etc.)
- `refactor` - Code restructuring
- `test` - Adding/fixing tests

**Examples:**

```text
chore(ci): pin GitHub Actions to SHA

- actions/checkout@v4 → @b4ffde65f46336ab88eb53be808477a3936bae11
- actions/setup-dotnet@v4 → @6bd8b7f7774af54e05809fcc5431931b3eb1ddee
- Configured Dependabot for SHA updates

Refs: W2.15
```

```text
docs(adr): add ADR-001 for factory pattern

Document the WorkItemStoreFactory design decision including:
- Problem context
- Decision drivers
- Considered alternatives
- Consequences

Refs: W2.5
```

---

## Markdown Formatting Standards

**CRITICAL**: All markdown files must pass linting. The pre-commit hook auto-fixes most issues, but some require manual attention.

### Code Block Language Identifiers (MD040)

**ALWAYS** add a language identifier to code blocks:

```markdown
<!-- ❌ WRONG - triggers MD040 -->
` ` `
some code
` ` `

<!-- ✅ CORRECT -->
` ` `text
some code
` ` `
```

Common language identifiers:

| Content Type | Language ID |
|--------------|-------------|
| C# code | `csharp` |
| PowerShell/shell commands | `powershell` or `bash` |
| JSON/JSON-like | `json` |
| Markdown templates | `markdown` |
| Plain text, pseudo-code, diagrams | `text` |
| Tool calls (cloudmcp-manager) | `text` |
| Workflow diagrams (→ arrows) | `text` |

### Pre-commit Hook

The repository has a pre-commit hook that auto-fixes linting issues. It will:

1. Auto-fix markdown with `markdownlint-cli2 --fix`
2. Auto-fix C# with `dotnet format`
3. Auto-fix JSON/YAML with `dotnet pprettier --write`
4. Re-stage corrected files automatically

If issues can't be auto-fixed (like missing language identifiers), the commit will fail with instructions.

---

## Session Log Template

Create this file at session start: `.agents/sessions/YYYY-MM-DD-phase-XX.md`

````markdown
# Session Log: Phase XX - [Date]

## Session Info

- **Date**: YYYY-MM-DD
- **Phase**: 2A (or whichever phase)
- **Branch**: `chore/modernize-wave-2`
- **Starting Commit**: [SHA]

## Pre-Flight Checks

- [ ] Build passes
- [ ] Tests pass (X/X)
- [ ] Read HANDOFF.md
- [ ] Identified tasks: W2.XX, W2.YY, W2.ZZ

## Tasks Completed

### W2.XX - [Task Name]

**Status**: ✅ Complete | 🔄 In Progress | ❌ Blocked

**What was done**:

- [Specific changes made]

**Decisions made**:

- [Decision]: [Rationale]

**Challenges**:

- [Challenge]: [Resolution]

**Files changed**:

- `path/to/file.cs` - [description]

**Commits**:

- `abc1234` - [commit message]

---

### W2.YY - [Task Name]

[Same structure]

---

## Session Summary

**Completed**: X/Y tasks
**Time spent**: ~X hours
**Next up**: [What the next session should do]

## Verification Commands

```powershell
# Verify CI build (ALWAYS use CI flags before pushing)
dotnet build Qwiq.sln -c Release /p:ContinuousIntegrationBuild=true /p:UseSharedCompilation=false /m:1 /nodeReuse:false

# Verify tests
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

# Verify specific changes
[any specific verification commands]
```
````

## Notes for Next Session

- [Important context]
- [Gotchas discovered]
- [Recommendations]

`````markdown
---

## HANDOFF.md Template

Update this file at session end:

````markdown
# Handoff Document

> **Last Updated**: YYYY-MM-DD by [Agent/Session ID] > **Current Phase**: 2A (or current)
> **Branch**: `chore/modernize-wave-2`

## Current State

**Build Status**: ✅ Passing | ❌ Failing
**Test Status**: ✅ X/Y Passing | ❌ Failing

**Last Commit**: `abc1234` - [message]

## What Was Completed

### Phase 2A (or current phase)

- [x] W2.XX - [Brief description of what was done]
- [x] W2.YY - [Brief description]
- [ ] W2.ZZ - [Not started / In progress]

## What's Next

The next session should:

1. [Specific first action]
2. [Specific second action]
3. [etc.]

## Blockers & Concerns

| Issue   | Impact   | Mitigation   |
| ------- | -------- | ------------ |
| [Issue] | [Impact] | [What to do] |

## Quick Verification

```powershell
# Run these commands to verify state
git log --oneline -5
# IMPORTANT: Use CI build flags to catch analyzer errors early
dotnet build Qwiq.sln -c Release /p:ContinuousIntegrationBuild=true /p:UseSharedCompilation=false /m:1 /nodeReuse:false
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"
```

---

## Session History

| Date       | Phase | Tasks      | Status         |
| ---------- | ----- | ---------- | -------------- |
| YYYY-MM-DD | 2A    | W2.5, W2.2 | ✅ Complete    |
| YYYY-MM-DD | 2A    | W2.15      | 🔄 In Progress |

## Files to Review

If you need context, read these files in order:

1. `.agents/AGENT-INSTRUCTIONS.md` (this process)
2. `.agents/planning/modernize-TODO-index.md` (overview and navigation)
3. The appropriate wave file for your tasks:
   - `.agents/planning/modernize-wave1.md` - Wave 0-1 tasks
   - `.agents/planning/modernize-wave2.md` - Wave 2 tasks
   - `.agents/planning/modernize-wave3-5.md` - Waves 3-5 tasks
4. `.agents/sessions/YYYY-MM-DD-phase-XX.md` (last session details)

---

## Phase Definitions

### Phase 2A: Release Automation (CRITICAL)

**Tasks**: W2.5, W2.2, W2.15, W2.18, W2.11
**Goal**: Establish foundational documentation and release infrastructure

### Phase 2B: Supply Chain Security (CRITICAL)

**Tasks**: W2.17, W2.13, W2.14
**Goal**: Implement supply chain security measures

### Phase 2C: Testing Enhancements

**Tasks**: W2.16, W2.3, W2.4
**Goal**: Add unit test coverage for REST/SOAP clients

### Phase 2D: Security Hardening

**Tasks**: W2.19, W2.20
**Goal**: Add security scanning to CI

### Phase 2E: Documentation

**Tasks**: W2.7
**Goal**: Update contribution guidelines

---

## Tools & Commands Reference

### Build & Test

```powershell
# CI build - ALWAYS USE THIS BEFORE PUSHING (matches CI pipeline)
dotnet build Qwiq.sln -c Release /p:ContinuousIntegrationBuild=true /p:UseSharedCompilation=false /m:1 /nodeReuse:false

# Run tests with standard filters
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

# Format code
dotnet format Qwiq.sln
```
`````

### Git Operations

```powershell
# Check status
git status

# Stage specific files (including .agents/)
git add .agents/
git add [other files]

# Commit with conventional message
git commit -m "type(scope): description"

# Force add .agents files if needed
git add -f .agents/sessions/*.md
```

### Verification

```powershell
# Count warnings
dotnet build Qwiq.sln -c Release 2>&1 | Select-String "warning"

# Check for specific patterns
Select-String -Path ".github/workflows/*.yml" -Pattern "uses:"
```

---

## Lessons Learned

> **Note**: This section captures real issues discovered during modernization work.
> Read these carefully to avoid repeating past mistakes.

### Session 39: CI Build Command Mismatch (2025-12-13)

**Issue**: Session 38 introduced tests that passed local builds but failed CI with CA1711, CA1001, and CA1861 analyzer errors.

**Root Cause**: Local builds used `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false` which does NOT enable CI-specific analyzer strictness. The CI pipeline uses:

```powershell
dotnet build Qwiq.sln -c Release /p:ContinuousIntegrationBuild=true /p:UseSharedCompilation=false /m:1 /nodeReuse:false
```

**Key Differences**:

- `/p:ContinuousIntegrationBuild=true` - Enables stricter warnings/errors
- `/p:UseSharedCompilation=false` - Prevents shared compiler state issues

**Prevention**:

1. **ALWAYS** use the CI build command locally before pushing:

   ```powershell
   dotnet build Qwiq.sln -c Release /p:ContinuousIntegrationBuild=true /p:UseSharedCompilation=false /m:1 /nodeReuse:false
   ```

2. Check for these common test class issues:
   - **CA1711**: Test class names ending in "Collection", "Dictionary", "Queue", etc.
   - **CA1001**: Test classes owning disposable fields without implementing IDisposable
   - **CA1861**: Inline array allocations that should be `static readonly` fields

**Fix Applied**:

- Added `#pragma warning disable CA1001` with explanation comment
- Renamed classes ending in "Collection" to use "ToWIC" abbreviation
- Extracted inline arrays to `static readonly` fields in a `TestArrays` class

---

## Critical Reminders

### DO

- ✅ Read ALL instructions before starting
- ✅ Work incrementally with small commits
- ✅ Update documentation as you go
- ✅ Check off tasks immediately when complete
- ✅ Run build/tests frequently
- ✅ Use `dotnet format` after code changes
- ✅ Update HANDOFF.md before session ends
- ✅ Force-add `.agents/` files if needed

### DON'T

- ❌ Skip the pre-flight checklist
- ❌ Make large commits with multiple unrelated changes
- ❌ Forget to update modernize-TODO.md checkboxes
- ❌ Leave session without updating HANDOFF.md
- ❌ Assume the next session has context you didn't document
- ❌ Skip verification steps

---

## Emergency Recovery

If something goes wrong:

1. **Build fails**: Check last working commit with `git log --oneline`
2. **Tests fail**: Run specific test to isolate: `dotnet test --filter "FullyQualifiedName~TestName"`
3. **Lost context**: Read session logs in `.agents/sessions/`
4. **Unclear what to do**: Re-read `modernize-TODO.md` task description

---

## Document Control

| Version | Date       | Changes                                                  |
| ------- | ---------- | -------------------------------------------------------- |
| 1.0     | 2025-12-06 | Initial agent instructions                               |
| 1.1     | 2025-12-13 | Added Lessons Learned section; updated CI build commands |
