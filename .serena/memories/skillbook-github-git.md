# GitHub & Git Skills

## Skill-GitHub-001

**Entity Type**: Skill
**Statement**: GitHub @copilot cannot be assigned to issues; use comment mentions for bot integration
**Atomicity**: 98%
**Category**: GitHub
**Context**: When trying to involve Copilot in GitHub issues
**Evidence**: Session 40 - Assignment failed, comment mention succeeded
**Tag**: helpful
**Impact**: 7
**Validated**: 1

**Summary**: GitHub API `add-assignee` expects actual user accounts. "copilot" is not an assignable GitHub user. Cannot add GitHub Actions or app accounts as assignees. Solution: Use comment mentions to trigger actions/workflows.

**Pattern - Correct** ✅:

```bash
# Use comment mention instead
gh issue comment [issue] -b "@copilot [your request]"

# Or in GitHub UI:
# Comment: @copilot analyze this bug

# For GitHub Actions:
# Use workflow triggers (on: [issues])
# Not assignees
```

---

## Skill-Issue-001

**Entity Type**: Skill
**Statement**: Include suggested fix code in bug reports to accelerate resolution
**Atomicity**: 92%
**Category**: GitHub
**Context**: When reporting bugs in external repositories
**Evidence**: Session 40 - Issue #6 included PowerShell append code snippet
**Tag**: helpful
**Impact**: 8
**Validated**: 1

**Summary**: Bug reports without solutions are harder to fix. Suggesting code helps maintainers understand intent. Shows you've diagnosed root cause. Significantly increases chance of acceptance/PR.

**Pattern - Better** ✅:

```markdown
# Issue: Script overwrites files instead of appending

## Problem

The `install-claude-repo.ps1` script uses `Out-File` without `-Append` flag, replacing CLAUDE.md instead of merging new content.

## Root Cause

Line 45: `Out-File -FilePath $configPath` should append, not replace.

## Suggested Fix

Replace:
\`\`\`powershell
'<content>' | Out-File -FilePath $configPath
\`\`\`

With:
\`\`\`powershell
'<content>' | Out-File -FilePath $configPath -Append
\`\`\`

## Evidence

- Reproduced on clean clone
- CLAUDE.md lost 200+ lines of config
- Fixed with: `git checkout -- CLAUDE.md`

## Impact

High - Anyone installing on existing repo loses configuration
```

---

## Skill-Git-001

**Entity Type**: Skill
**Statement**: Use `git checkout -- [file]` to immediately restore accidentally overwritten files
**Atomicity**: 97%
**Category**: Git
**Context**: When installation scripts or operations overwrite existing content
**Evidence**: Session 40 - CLAUDE.md recovery
**Tag**: helpful
**Impact**: 9
**Validated**: 1

**Summary**: Files can be accidentally overwritten by scripts or tools. Immediate recovery is possible if not yet committed. `git checkout` restores from last commit (no data loss). Fast recovery prevents manual reconstruction.

**Recovery Procedure**:

```bash
# 1. Immediately after discovering file was overwritten
git status  # Confirm file shows as modified

# 2. Restore to last committed version
git checkout -- path/to/file.md

# 3. Verify restoration
git status  # Should show clean
```

---

## Skill-Git-002

**Entity Type**: Skill
**Statement**: Use `git mv` for file moves to preserve history during reorganization
**Atomicity**: 91%
**Category**: Git
**Context**: When reorganizing directory structure
**Evidence**: Agent-docs-reorganization-plan.md validation checklist
**Tag**: helpful
**Impact**: 8
**Validated**: 1

**Summary**: Moving files with `mv` command breaks git history. `git mv` preserves blame, history, and annotations. Important for documentation and code archaeology. Reviewers can trace file origins.

**Pattern - Correct** ✅:

```bash
# Preserves history - git tracks movement
git mv old/location/file.md new/location/file.md
git commit -m "refactor: move file to new structure"
```
