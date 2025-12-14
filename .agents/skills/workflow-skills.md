# Workflow & Installation Skills

## Skill-Workflow-001

**Statement**: Check for installation scripts in source repos before manual file operations

**Atomicity**: 95%

**Category**: Workflow

**Context**: When installing tools/configs from external repos

**Evidence**: Session 40 - User correction led to using install-claude-repo.ps1

**Details**:

- Many open-source repos provide installation scripts
- Manual file download/copy often duplicates what scripts do
- Scripts handle edge cases and maintain proper formatting
- Using scripts ensures correct, maintainable installation

**Pattern - Wrong** ❌:

```bash
# Manual approach - error prone
curl https://raw.githubusercontent.com/repo/file.ps1 > ~/file.ps1
# Copy individual files manually
# Risk: missing dependencies, wrong format, incomplete setup
```

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

**Real Example** (Session 40):

```bash
# Initial approach: manual download
# User: "That's inefficient, use the installation script"
# Correct approach:
./install-claude-repo.ps1 -RepoPath .
# Result: All 16 agents installed correctly
```

**Benefits**:

- Scripts handle edge cases
- Consistent across environments
- Maintainable (updates in one place)
- Preserves existing configurations
- Proper error handling

---

## Skill-Install-001

**Statement**: Installation scripts may replace config files; backup or verify before running

**Atomicity**: 91%

**Category**: Installation

**Context**: When running installation scripts on repos with existing configs

**Evidence**: Session 40 - Script replaced CLAUDE.md, required restoration

**Details**:

- Installation scripts sometimes have bugs (append vs replace)
- Can overwrite existing configuration files
- Should backup or verify script behavior first
- Recovery via `git checkout` if not committed

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

**Real Example** (Session 40):

```bash
# Script used Out-File without -Append flag
# Result: CLAUDE.md was replaced, not appended
# Recovery: git checkout -- CLAUDE.md

# Restored 200+ lines of project configuration
# Could have been prevented with backup step
```

**Bug Report Pattern**:

When you find script issues:

```bash
# 1. Document the problem
# 2. Recover with git
git checkout -- [affected-files]

# 3. Report issue with suggested fix
# Include example PowerShell code:
# >> for append instead of > for replace
# -Append parameter in Out-File

# 4. Provide before/after example
```

**Verification**:

```bash
# Before running script
git status  # Clean working tree
git stash  # Save any uncommitted work

# After script runs, verify changes
git status  # Check what changed
git diff CLAUDE.md  # Review specific files

# If wrong, restore immediately
git checkout -- CLAUDE.md
```

**Best Practices**:

1. **Always review changes**:

   ```bash
   git diff  # See exactly what changed
   ```

2. **Commit changes separately**:

   ```bash
   git add .
   git commit -m "chore: install new agents"
   ```

3. **Test on feature branch**:

   ```bash
   git checkout -b test/agent-installation
   ./install-script.ps1
   ```

4. **PR before merging** to main

5. **Run CI** to verify no breakage

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

**Statement**: AI agents generating markdown should run lint validation before commit

**Atomicity**: 90%

**Category**: Workflow

**Context**: When agents create or modify markdown files

**Evidence**: Session markdown-lint-incident 2025-12-14 - Multiple agent-generated files in .agents/ contained lint errors

**Details**:

- AI agents (Claude, retrospective, analyst, etc.) generate markdown documentation
- Generated markdown may not follow linting rules (MD040, MD033, MD036, etc.)
- Pre-commit hooks only validate staged files, not generation quality
- Solution: Agents should validate their own output before committing

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

| Issue | Rule | Fix |
|-------|------|-----|
| Missing language on code fence | MD040 | Add `text`, `bash`, `yaml`, etc. |
| HTML lists in tables | MD033 | Ensure ul/li in allowed_elements |
| Bold text as heading | MD036 | Use `##` or `###` headings |
| Missing blank lines | MD031 | Add blank line before/after fences |

**Agent Instructions Addition**:

Consider adding to agent prompts:

```text
When generating markdown files:
1. Always include language identifiers on code fences
2. Use proper heading syntax (##, ###) not bold text
3. Run markdownlint validation before committing
```

**Benefits**:

- Catches issues at generation time, not CI time
- Reduces fix-commit cycles
- Maintains documentation quality
- Prevents lint error accumulation
