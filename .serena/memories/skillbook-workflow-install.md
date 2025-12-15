# Workflow & Installation Skills

## Skill-Workflow-001

**Entity Type**: Skill
**Statement**: Check for installation scripts in source repos before manual file operations
**Atomicity**: 95%
**Category**: Workflow
**Context**: When installing tools/configs from external repos
**Evidence**: Session 40 - User correction led to using install-claude-repo.ps1
**Tag**: helpful
**Impact**: 8
**Validated**: 1

**Summary**: Many open-source repos provide installation scripts. Manual file download/copy often duplicates what scripts do. Scripts handle edge cases and maintain proper formatting. Using scripts ensures correct, maintainable installation.

**Pattern - Correct** ✅:

```bash
# Check for installation script first
# vs-code-agents repo includes install-claude-repo.ps1
./install-claude-repo.ps1 -RepoPath .

# Script handles:
# - Creating correct directory structure
# - Copying all necessary files
# - Preserving existing configurations
# - Setting up dependencies
```

**Pre-Installation Checklist**:

1. **Check repository** for:

   - `install.sh` / `install.ps1`
   - `setup.py` / `Makefile`
   - Installation instructions in README

2. **Review script** before running:

   - What files will it create/modify?
   - Will it preserve existing configs?
   - Does it require special permissions?

3. **Test on clean clone** first:

   ```bash
   git clone repo test-clone
   cd test-clone
   ./install-script.ps1  # Test first
   ```

4. **Only then run** on your actual repo:

   ```bash
   ./install-script.ps1 -RepoPath .
   ```

---

## Skill-Install-001

**Entity Type**: Skill
**Statement**: Installation scripts may replace config files; backup or verify before running
**Atomicity**: 91%
**Category**: Installation
**Context**: When running installation scripts on repos with existing configs
**Evidence**: Session 40 - Script replaced CLAUDE.md, required restoration
**Tag**: helpful
**Impact**: 9
**Validated**: 1

**Summary**: Installation scripts sometimes have bugs (append vs replace). Can overwrite existing configuration files. Should backup or verify script behavior first. Recovery via `git checkout` if not committed.

**Safety Protocol**:

```bash
# Step 1: Backup important files
cp CLAUDE.md CLAUDE.md.backup
cp .editorconfig .editorconfig.backup

# Step 2: Check script behavior
cat install-script.ps1 | grep -E "(Out-File|>|Add-Content)"
# Look for: >>  (append) vs >  (replace)

# Step 3: Run on test clone first
cd /tmp
git clone <repo> test-repo
cd test-repo
./install-script.ps1  # Run script
# Review changes safely

# Step 4: Only then run on real repo
cd ~/real-repo
./install-script.ps1
```

**Recovery Commands**:

```bash
# If script overwrote files
git checkout -- CLAUDE.md .editorconfig

# If committed by mistake
git revert [commit-sha]

# If need specific version
git show [older-commit]:CLAUDE.md > CLAUDE.md
```

---

## Skill-Workflow-002

**Entity Type**: Skill
**Statement**: AI agents generating markdown should run lint validation before commit
**Atomicity**: 90%
**Category**: Workflow
**Context**: When agents create or modify markdown files
**Evidence**: Session markdown-lint-incident 2025-12-14 - Multiple agent-generated files in .agents/ contained lint errors
**Tag**: helpful
**Impact**: 8
**Validated**: 1

**Summary**: AI agents (Claude, retrospective, analyst, etc.) generate markdown documentation. Generated markdown may not follow linting rules (MD040, MD033, MD036, etc.). Pre-commit hooks only validate staged files, not generation quality. Solution: Agents should validate their own output before committing.

**Agent Output Checklist**:

```markdown
## Before Committing Agent-Generated Markdown

- [ ] Run `npx markdownlint-cli2 --fix <file>` on generated files
- [ ] Verify code fences have language specifiers (`text` for generic content)
- [ ] Avoid HTML in tables unless ul/li is allowlisted
- [ ] Use proper headings instead of bold text for sections
- [ ] Ensure blank lines around code fences
```

**Implementation Pattern**:

```bash
# After agent generates markdown
npx markdownlint-cli2 --fix .agents/retrospective/new-file.md

# Verify no remaining issues
npx markdownlint-cli2 .agents/retrospective/new-file.md

# Then stage and commit
git add .agents/retrospective/new-file.md
git commit -m "docs: add retrospective analysis"
```

**Common Agent-Generated Issues**:

| Issue                          | Rule  | Fix                                |
| ------------------------------ | ----- | ---------------------------------- |
| Missing language on code fence | MD040 | Add `text`, `bash`, `yaml`, etc.   |
| HTML lists in tables           | MD033 | Ensure ul/li in allowed_elements   |
| Bold text as heading           | MD036 | Use `##` or `###` headings         |
| Missing blank lines            | MD031 | Add blank line before/after fences |

---

## Skill-Agent-WF-001

**Entity Type**: Skill
**Statement**: Multi-agent Epic workflow (analyst → roadmap → explainer → task-generator) produces zero-rework PRDs and atomic task lists
**Atomicity**: 90%
**Category**: Workflow
**Context**: When breaking down complex features requiring design validation before implementation
**Evidence**: Session 41 - Reproducible Builds Epic (W2.34-W2.37) produced 4 atomic tasks that mapped 1:1 to execution with zero rework cycles
**Tag**: helpful
**Impact**: 9
**Validated**: 1

**Summary**: Multi-agent perspective catches edge cases. Analyst identifies technical details and constraints. Roadmap agent frames strategic vision. Explainer creates comprehensive PRD. Task-generator atomizes into implementable tasks. Pipeline validates completeness before implementation begins.

**Workflow Pipeline**:

```text
Complex Feature Request
        ↓
    ┌───────────┐
    │  ANALYST  │ ← Research technical approach, constraints, risks
    └───────────┘
        ↓
    ┌───────────┐
    │  ROADMAP  │ ← Frame strategic vision and Epic scope
    └───────────┘
        ↓
    ┌───────────┐
    │ EXPLAINER │ ← Write comprehensive PRD with requirements
    └───────────┘
        ↓
    ┌───────────┐
    │TASK-GENER │ ← Create atomic, implementable task list
    └───────────┘
        ↓
Implementation Tasks (W2.34, W2.35, W2.36, W2.37)
```

**Real Example** (Session 41 - Reproducible Builds Epic):

| Agent     | Output                                                          |
| --------- | --------------------------------------------------------------- |
| analyst   | Research: DotNet.ReproducibleBuilds features, CI detection      |
| roadmap   | Epic vision: Deterministic builds for production deployment     |
| explainer | PRD: `.agents/planning/PRD-reproducible-builds.md`              |
| task-gen  | W2.34 (add pkg), W2.35 (integrate), W2.36 (verify), W2.37 (doc) |

**Implementation Results**:

- 4 tasks created by task-generator
- 4 tasks completed as specified (no changes needed)
- 0 rework cycles
- Build passed first try (0 warnings)
- All 724 tests passed

**When to Use Epic Workflow**:

| Scenario                        | Use Epic Workflow? |
| ------------------------------- | ------------------ |
| New feature integration         | ✅ Yes             |
| Bug fix                         | ❌ No              |
| Configuration change            | ❌ No              |
| Multi-file architectural change | ✅ Yes             |
| Research + implementation       | ✅ Yes             |
| Simple code edit                | ❌ No              |
