# Copilot Agent Instructions for Qwiq Modernization

> **CRITICAL**: Read this entire document before starting ANY work.
> This document governs how Copilot agents execute modernization phases.

---

## Quick Start Checklist

Before starting work, complete these steps IN ORDER:

- [ ] Read this file completely
- [ ] Read `modernize-TODO.md` to understand current state
- [ ] Read `modernize-explainer.md` for architectural context
- [ ] Check `HANDOFF.md` for previous session notes
- [ ] Identify your assigned phase (e.g., "Phase 2A")
- [ ] Create a session log file: `.agents/sessions/YYYY-MM-DD-phase-XX.md`

---

## Document Hierarchy

| Document | Purpose | When to Update |
|----------|---------|----------------|
| `AGENT-INSTRUCTIONS.md` | How to execute work (this file) | Rarely - only if process changes |
| `modernize-TODO.md` | Task tracking, checkboxes, progress | After EVERY task completion |
| `modernize-explainer.md` | Architecture, decisions, rationale | When design decisions are made |
| `HANDOFF.md` | Session-to-session context transfer | At END of every session |
| `sessions/*.md` | Detailed session logs | Throughout session |

---

## Phase Execution Protocol

### 1. Session Initialization (MANDATORY)

```markdown
## Session Start Checklist
- [ ] Created session log: `.agents/sessions/YYYY-MM-DD-phase-XX.md`
- [ ] Read HANDOFF.md from previous session
- [ ] Identified all tasks in assigned phase
- [ ] Verified build passes: `dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false`
- [ ] Verified tests pass: `dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"`
- [ ] Noted starting git state: `git status`
```

### 2. Task Execution (FOR EACH TASK)

**Before starting a task:**
1. Read the full task description in `modernize-TODO.md`
2. Understand acceptance criteria
3. Plan the implementation approach

**During task execution:**
1. Work incrementally - small, atomic changes
2. Commit frequently with conventional commit messages
3. Run `dotnet format` after code changes
4. Run build after each significant change
5. Run tests after each significant change

**After completing a task:**
1. ✅ Check off the task in `modernize-TODO.md`
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
- [ ] All assigned tasks checked off in modernize-TODO.md
- [ ] Session log complete with all details
- [ ] HANDOFF.md updated with:
  - [ ] What was completed
  - [ ] What's next
  - [ ] Any blockers or concerns
  - [ ] Commands to verify state
- [ ] All files committed (including .agents/ files)
- [ ] Build passes
- [ ] Tests pass
- [ ] Git status is clean (or intentionally dirty with explanation)
```

---

## Commit Message Format

Use conventional commits:

```
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
```
chore(ci): pin GitHub Actions to SHA

- actions/checkout@v4 → @b4ffde65f46336ab88eb53be808477a3936bae11
- actions/setup-dotnet@v4 → @6bd8b7f7774af54e05809fcc5431931b3eb1ddee
- Configured Dependabot for SHA updates

Refs: W2.15
```

```
docs(adr): add ADR-001 for factory pattern

Document the WorkItemStoreFactory design decision including:
- Problem context
- Decision drivers
- Considered alternatives
- Consequences

Refs: W2.5
```

---

## Session Log Template

Create this file at session start: `.agents/sessions/YYYY-MM-DD-phase-XX.md`

```markdown
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
# Verify build
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false

# Verify tests
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

# Verify specific changes
[any specific verification commands]
```

## Notes for Next Session
- [Important context]
- [Gotchas discovered]
- [Recommendations]
```

---

## HANDOFF.md Template

Update this file at session end:

```markdown
# Handoff Document

> **Last Updated**: YYYY-MM-DD by [Agent/Session ID]
> **Current Phase**: 2A (or current)
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

| Issue | Impact | Mitigation |
|-------|--------|------------|
| [Issue] | [Impact] | [What to do] |

## Quick Verification

```powershell
# Run these commands to verify state
git log --oneline -5
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"
```

## Session History

| Date | Phase | Tasks | Status |
|------|-------|-------|--------|
| YYYY-MM-DD | 2A | W2.5, W2.2 | ✅ Complete |
| YYYY-MM-DD | 2A | W2.15 | 🔄 In Progress |

## Files to Review

If you need context, read these files in order:
1. `.agents/AGENT-INSTRUCTIONS.md` (this process)
2. `.agents/modernize-TODO.md` (task details)
3. `.agents/sessions/YYYY-MM-DD-phase-XX.md` (last session details)
```

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
# Full build (single-threaded to avoid file locking)
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false

# Run tests with standard filters
dotnet test Qwiq.sln -c Release --no-build --filter "TestCategory!=localOnly&TestCategory!=Benchmark&TestCategory!=SOAP&TestCategory!=REST&TestCategory!=IntegrationTests"

# Format code
dotnet format Qwiq.sln
```

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

## Critical Reminders

### DO:
- ✅ Read ALL instructions before starting
- ✅ Work incrementally with small commits
- ✅ Update documentation as you go
- ✅ Check off tasks immediately when complete
- ✅ Run build/tests frequently
- ✅ Use `dotnet format` after code changes
- ✅ Update HANDOFF.md before session ends
- ✅ Force-add `.agents/` files if needed

### DON'T:
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

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2025-12-06 | Initial agent instructions |
