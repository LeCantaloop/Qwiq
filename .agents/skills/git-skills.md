# Git Skills

## Skill-Git-001

**Statement**: Use `git checkout -- [file]` to immediately restore accidentally overwritten files

**Atomicity**: 97%

**Category**: Git

**Context**: When installation scripts or operations overwrite existing content

**Evidence**: Session 40 - CLAUDE.md recovery

**Details**:

- Files can be accidentally overwritten by scripts or tools
- Immediate recovery is possible if not yet committed
- `git checkout` restores from last commit (no data loss)
- Fast recovery prevents manual reconstruction

**Recovery Procedure**:

```bash
# 1. Immediately after discovering file was overwritten
git status  # Confirm file shows as modified

# 2. Restore to last committed version
git checkout -- path/to/file.md

# 3. Verify restoration
git status  # Should show clean

# 4. If multiple files overwritten
git checkout -- .agents/
git checkout -- .

# 5. Check what was restored
git diff HEAD  # Should show nothing
```

**Real Example** (Session 40):

```bash
# Installation script overwrote CLAUDE.md
# Immediately restored:
git checkout -- CLAUDE.md

# Verified 200+ lines of project config restored
git show CLAUDE.md | head -50
```

**When to Use**:

1. **Installation scripts** override existing config
2. **Auto-formatting tools** mangle files
3. **Accidental edits** via IDE operations
4. **Merge tools** create bad output

**Warning**: Once changes are committed, recovery requires git reflog:

```bash
# If file was already committed, use reflog
git reflog  # Find previous state
git checkout <sha> -- file.md
```

**Prevention**:

1. Review scripts before running on configured repos
2. Backup critical files before running automated tools
3. Use version control for all important files
4. Test scripts on clean clone first

---

## Skill-Git-002

**Statement**: Use `git mv` for file moves to preserve history during reorganization

**Atomicity**: 91%

**Category**: Git

**Context**: When reorganizing directory structure

**Evidence**: Agent-docs-reorganization-plan.md validation checklist

**Details**:

- Moving files with `mv` command breaks git history
- `git mv` preserves blame, history, and annotations
- Important for documentation and code archaeology
- Reviewers can trace file origins

**Pattern - Wrong** ❌:

```bash
# Breaks history - git sees delete + new file
mv old/location/file.md new/location/file.md
git add new/location/file.md
git rm old/location/file.md
```

**Pattern - Correct** ✅:

```bash
# Preserves history - git tracks movement
git mv old/location/file.md new/location/file.md
git commit -m "refactor: move file to new structure"
```

**Multi-File Reorganization**:

```bash
# Move entire subdirectory
git mv .agents/docs/ .agents/planning/

# Move multiple files at once
git mv old/file1.md new/
git mv old/file2.md new/
git mv old/file3.md new/

# Verify moves before committing
git status  # Shows all moves as 'renamed'

# Commit with descriptive message
git commit -m "refactor: reorganize documentation structure"
```

**Verification**:

```bash
# Check history is preserved
git log --follow -- new/location/file.md  # Shows old path history
git blame new/location/file.md  # Shows original authors
```

**Benefits**:

1. **History Tracking**: `git log --follow` finds file through moves
2. **Blame Attribution**: Original authors credited in new location
3. **Bisect Functionality**: Git can track changes across moves
4. **Clean Commits**: Single move operation vs. delete + add

**When to Use**:

- Major directory restructuring
- Renaming files
- Consolidating files from multiple locations
- Moving code between packages/projects

**Note**: GUI tools (VS Code explorer) often handle this automatically with `git mv`
