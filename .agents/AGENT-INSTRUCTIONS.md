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

` ` `some code` ` `

<!-- ✅ CORRECT -->

` ` `text
some code
` ` `
```

Common language identifiers:

| Content Type                      | Language ID            |
| --------------------------------- | ---------------------- |
| C# code                           | `csharp`               |
| PowerShell/shell commands         | `powershell` or `bash` |
| JSON/JSON-like                    | `json`                 |
| Markdown templates                | `markdown`             |
| Plain text, pseudo-code, diagrams | `text`                 |
| Tool calls (cloudmcp-manager)     | `text`                 |
| Workflow diagrams (→ arrows)      | `text`                 |

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

## Recommended Agent Workflows

> **Note**: The retrospective analysis (2025-12-13) identified several underutilized agents. Use these workflows to maximize agent effectiveness.

### Full Feature Development (Recommended)

Use this workflow for any non-trivial feature to ensure quality and documentation:

```text
analyst → architect → planner → critic → csharp-expert → qa → retrospective
```

| Step | Agent           | Purpose                                        | Output                   |
| ---- | --------------- | ---------------------------------------------- | ------------------------ |
| 1    | `analyst`       | Research existing code, gather requirements    | `.agents/analysis/`      |
| 2    | `architect`     | Design decision, create ADR if needed          | `.agents/architecture/`  |
| 3    | `planner`       | Break down into tasks with acceptance criteria | `.agents/planning/`      |
| 4    | `critic`        | **Validate plan before implementation**        | `.agents/critique/`      |
| 5    | `csharp-expert` | Implement code following the plan              | Source files             |
| 6    | `qa`            | Verify implementation, document test strategy  | `.agents/qa/`            |
| 7    | `retrospective` | Extract learnings, update skills               | `.agents/retrospective/` |

### Quick Fix Workflow

For small bug fixes or simple changes:

```text
csharp-expert → qa
```

### Strategic Decision Workflow

For major architectural or strategic decisions:

```text
analyst → independent-thinker → high-level-advisor → architect
```

### Post-Implementation Learning

After completing significant work:

```text
csharp-expert → retrospective → skillbook
```

### Plan Validation (Often Skipped - Don't Skip!)

**IMPORTANT**: The `critique/` directory is currently empty. Always invoke the critic agent before implementation:

```text
Task(subagent_type="critic", prompt="Validate plan at .agents/planning/[plan-file].md")
```

The critic will:

- Identify gaps in the plan
- Challenge assumptions
- Suggest improvements
- Flag risks

### QA Documentation (Often Skipped - Don't Skip!)

**IMPORTANT**: The `qa/` directory is currently empty. Always invoke the qa agent after implementation:

```text
Task(subagent_type="qa", prompt="Create test strategy for [feature], document in .agents/qa/")
```

The QA agent will:

- Define test coverage requirements
- Document edge cases
- Create acceptance criteria
- Verify implementation meets requirements

---

## Agent Invocation Reference

### Claude Code CLI

```python
# Research before implementation
Task(subagent_type="analyst", prompt="Investigate [topic]")

# Design review before coding
Task(subagent_type="csharp-pod", prompt="Review design for [feature]")

# Plan validation (REQUIRED before implementation)
Task(subagent_type="critic", prompt="Validate plan at .agents/planning/...")

# Implementation
Task(subagent_type="csharp-expert", prompt="Implement [feature] per plan")

# Test verification (REQUIRED after implementation)
Task(subagent_type="qa", prompt="Verify [feature] and document test strategy")

# Extract learnings (after significant work)
Task(subagent_type="retrospective", prompt="Analyze session and extract learnings")
```

### Memory Operations (When Available)

```python
# READ OPERATIONS - Search for relevant context before starting
mcp__cloudmcp-manager__memory-search_nodes(query="[topic]")
mcp__cloudmcp-manager__memory-read_graph()
mcp__cloudmcp-manager__memory-open_nodes(names=["entity-name"])

# WRITE OPERATIONS - Store learnings at session end
mcp__cloudmcp-manager__memory-create_entities(entities=[...])
mcp__cloudmcp-manager__memory-add_observations(observations=[...])
mcp__cloudmcp-manager__memory-create_relations(relations=[...])

# DELETE OPERATIONS - Remove or update incorrect learnings
mcp__cloudmcp-manager__memory-delete_entities(entityNames=[...])
mcp__cloudmcp-manager__memory-delete_observations(deletions=[...])
mcp__cloudmcp-manager__memory-delete_relations(relations=[...])
```

### Memory Service Fallback

If cloudmcp-manager memory service is unavailable:

**For READ operations** (`memory-search_nodes`, `memory-read_graph`, `memory-open_nodes`):

- Use `.agents/skills/README.md` to navigate available skills
- Open relevant skill files in `.agents/skills/[category]-skills.md`
- Use `grep` or search to find specific skills by ID or keyword

**For WRITE operations** (`memory-create_entities`, `memory-add_observations`, `memory-create_relations`):

- Add new skills to appropriate file in `.agents/skills/[category]-skills.md`
- Follow the skill format: statement, atomicity, category, context, evidence, details
- Create new category file if needed (e.g., `performance-skills.md`)
- Commit changes with descriptive message

**For DELETE operations** (`memory-delete_entities`, `memory-delete_observations`, `memory-delete_relations`):

- Edit relevant skill file in `.agents/skills/[category]-skills.md`
- Remove or update the skill section
- Delete entire file if all skills removed
- Commit deletion with message: `chore(skills): remove [skill-id] - [reason]`

**Example - Reading a skill:**

```bash
# Instead of:
mcp__cloudmcp-manager__memory-search_nodes(query="CI build")

# Use:
grep -r "CI build\|ContinuousIntegrationBuild" .agents/skills/
# Returns: .agents/skills/build-skills.md
cat .agents/skills/build-skills.md  # Open and review
```

**Example - Adding a skill:**

```bash
# Instead of:
mcp__cloudmcp-manager__memory-create_entities(entities=[...])

# Edit the appropriate file:
vim .agents/skills/[category]-skills.md

# Add new skill following this format:
## Skill-[Category]-NNN

**Statement**: [Your skill statement]
**Atomicity**: XX%
**Category**: [Category]
**Context**: [When to use]
**Evidence**: [Where discovered]
```

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
- ✅ **Invoke critic agent** before major implementations
- ✅ **Invoke qa agent** after implementations
- ✅ **Run retrospective agent** after significant sessions

### DON'T

- ❌ Skip the pre-flight checklist
- ❌ Make large commits with multiple unrelated changes
- ❌ Forget to update modernize-TODO.md checkboxes
- ❌ Leave session without updating HANDOFF.md
- ❌ Assume the next session has context you didn't document
- ❌ Skip verification steps
- ❌ **Skip critic validation** - empty critique/ directory is a warning sign
- ❌ **Skip qa documentation** - empty qa/ directory is a warning sign

---

## Emergency Recovery

If something goes wrong:

1. **Build fails**: Check last working commit with `git log --oneline`
2. **Tests fail**: Run specific test to isolate: `dotnet test --filter "FullyQualifiedName~TestName"`
3. **Lost context**: Read session logs in `.agents/sessions/`
4. **Unclear what to do**: Re-read `modernize-TODO.md` task description

---

## Extracted Skills Reference

> **Source**: Comprehensive retrospective analysis (2025-12-13)
> **Status**: Skills should be stored in cloudmcp-manager memory when service is available

### Build & CI Skills

| Skill ID        | Statement                                                                                                                                     | Atomicity |
| --------------- | --------------------------------------------------------------------------------------------------------------------------------------------- | --------- |
| Skill-Build-001 | Use `/p:ContinuousIntegrationBuild=true /p:UseSharedCompilation=false /m:1 /nodeReuse:false` for local builds to match CI analyzer strictness | 96%       |
| Skill-Build-002 | Set `BuildInParallel=false` and `ProduceReferenceAssembly=false` for Windows multi-framework builds                                           | 93%       |
| Skill-CI-001    | CI should verify lint rules without auto-fix to catch commits bypassing pre-commit hooks                                                      | 95%       |

### Testing Skills

| Skill ID       | Statement                                                                                                     | Atomicity |
| -------------- | ------------------------------------------------------------------------------------------------------------- | --------- |
| Skill-Test-001 | WireMock.Net OWIN hosting deadlocks on .NET Framework 4.7.2; use dedicated net8.0+ test project               | 95%       |
| Skill-Test-002 | Capture real HTTP traffic with Fiddler system proxy for WireMock stubs; SDK bypasses WireMock Cloud recording | 91%       |
| Skill-Test-003 | IdentityDescriptor must be string format in captured stubs, not object serialization                          | 94%       |

### Code Quality Skills

| Skill ID          | Statement                                                                                                  | Atomicity |
| ----------------- | ---------------------------------------------------------------------------------------------------------- | --------- |
| Skill-Quality-001 | Test classes ending in "Collection" trigger CA1711; use abbreviations like "ToWIC"                         | 92%       |
| Skill-Quality-002 | Add `#pragma warning disable CA1001` with explanation when test cleanup handles disposal via [TestCleanup] | 90%       |
| Skill-Quality-003 | Extract inline test arrays to `static readonly` fields to satisfy CA1861                                   | 88%       |

### Strategic Skills

| Skill ID            | Statement                                                                                                 | Atomicity |
| ------------------- | --------------------------------------------------------------------------------------------------------- | --------- |
| Skill-Strategic-001 | Always verify deployment scale (100+ team members vs external adoption) before declaring maintenance mode | 97%       |
| Skill-Strategic-002 | SOAP clients cannot deploy to Kubernetes (net472 Windows-only); deprecate in favor of REST                | 94%       |

### Documentation Skills

| Skill ID      | Statement                                                                               | Atomicity |
| ------------- | --------------------------------------------------------------------------------------- | --------- |
| Skill-Doc-001 | Split documentation files when approaching 25,000 token AI agent limit                  | 93%       |
| Skill-Doc-002 | Update HANDOFF.md at session end with build/test status, completed work, and next steps | 95%       |

### Git Skills

| Skill ID      | Statement                                                                          | Atomicity |
| ------------- | ---------------------------------------------------------------------------------- | --------- |
| Skill-Git-001 | Use `git checkout -- [file]` to immediately restore accidentally overwritten files | 97%       |
| Skill-Git-002 | Use `git mv` for file moves to preserve history during reorganization              | 91%       |

### Markdown Skills

| Skill ID           | Statement                                                                             | Atomicity |
| ------------------ | ------------------------------------------------------------------------------------- | --------- |
| Skill-Markdown-001 | Always add language identifier to code fences; use 'text' for pseudo-code or diagrams | 98%       |
| Skill-Markdown-002 | Generic type syntax like `ArrayPool<T>` triggers MD033; use code blocks instead       | 96%       |

### Developer Experience Skills

| Skill ID           | Statement                                                                             | Atomicity |
| ------------------ | ------------------------------------------------------------------------------------- | --------- |
| Skill-DevEx-001    | Pre-commit hooks should auto-fix issues then verify, failing only on unfixable errors | 97%       |
| Skill-GitHooks-001 | Auto-fix hooks must re-stage modified files with `git add` after applying fixes       | 99%       |

### Workflow Skills

| Skill ID           | Statement                                                                      | Atomicity |
| ------------------ | ------------------------------------------------------------------------------ | --------- |
| Skill-Workflow-001 | Check for installation scripts in source repos before manual file operations   | 95%       |
| Skill-Install-001  | Installation scripts may replace config files; backup or verify before running | 91%       |

### GitHub Skills

| Skill ID         | Statement                                                                              | Atomicity |
| ---------------- | -------------------------------------------------------------------------------------- | --------- |
| Skill-GitHub-001 | GitHub @copilot cannot be assigned to issues; use comment mentions for bot integration | 98%       |
| Skill-Issue-001  | Include suggested fix code in bug reports to accelerate resolution                     | 92%       |

### Skill Citation Protocol

When applying a skill, cite it explicitly:

```markdown
**Applying**: Skill-Build-001
**Strategy**: Use CI build flags locally
**Expected**: Match CI analyzer behavior

[Execute command...]

**Result**: Build succeeded with same warnings as CI
**Skill Validated**: Yes
```

---

## Document Control

| Version | Date       | Changes                                                          |
| ------- | ---------- | ---------------------------------------------------------------- |
| 1.0     | 2025-12-06 | Initial agent instructions                                       |
| 1.1     | 2025-12-13 | Added Lessons Learned section; updated CI build commands         |
| 1.2     | 2025-12-13 | Added Recommended Agent Workflows and Extracted Skills Reference |
