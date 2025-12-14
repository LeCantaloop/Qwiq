# GitHub Skills

## Skill-GitHub-001

**Statement**: GitHub @copilot cannot be assigned to issues; use comment mentions for bot integration

**Atomicity**: 98%

**Category**: GitHub

**Context**: When trying to involve Copilot in GitHub issues

**Evidence**: Session 40 - Assignment failed, comment mention succeeded

**Details**:

- GitHub API `add-assignee` expects actual user accounts
- "copilot" is not an assignable GitHub user
- Cannot add GitHub Actions or app accounts as assignees
- Solution: Use comment mentions to trigger actions/workflows

**Pattern - Wrong** ❌:

```bash
# This fails - copilot is not a valid assignee
gh issue edit [issue] --add-assignee copilot
# Error: User not found

# Won't work in GitHub UI either
# Assignee dropdown doesn't list bots
```

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

**Integration Patterns**:

### For AI Assistants (like Copilot)

```bash
# Mention in comment to request analysis
gh issue comment [issue] -b "@copilot Please review this architecture decision"

# Copilot (if integrated) will respond in comments
# Not as assignee
```

### For GitHub Actions/Workflows

```yaml
# main.yml - Triggered on issue events
name: Issue Handler
on:
  issues:
    types: [opened, labeled]

jobs:
  analyze:
    runs-on: ubuntu-latest
    steps:
      - name: Check issue
        run: echo "Issue opened, analyzing..."
      - name: Post analysis
        run: gh issue comment --body "Analysis: ..."
```

### For Bot Accounts

```bash
# Create actual bot user account, then:
gh issue edit [issue] --add-assignee bot-username

# OR use workflows that auto-comment/label
gh issue edit [issue] --add-label "needs-review"
```

**Real Example** (Session 40):

```bash
# Initial attempt (failed):
gh issue edit 6 --add-assignee copilot
# Error: User "copilot" not found

# Corrected approach (successful):
gh issue comment 6 -b "@copilot Can you review this installation bug?"
# Comment created, bot integration works via mentions
```

**GitHub API Comparison**:

| Operation | API | Status | Alternative |
|-----------|-----|--------|-------------|
| Assign user | `--add-assignee username` | ✅ Works | User must be valid |
| Assign bot | `--add-assignee copilot` | ❌ Fails | Use mentions in comments |
| Mention bot | Comment with @copilot | ✅ Works | Triggers via webhook |
| Run workflow | `workflow_run` event | ✅ Works | Automatic on events |

**Best Practices**:

1. **For AI requests**: Use comment mentions

   ```bash
   gh issue comment --body "@copilot [request]"
   ```

2. **For automation**: Use workflows, not assignees

   ```yaml
   on:
     issues:
       types: [opened]
   ```

3. **For human review**: Assign actual users

   ```bash
   gh issue edit --add-assignee developer-username
   ```

4. **For labels/metadata**: Use issue edit

   ```bash
   gh issue edit --add-label "needs-review"
   ```

---

## Skill-Issue-001

**Statement**: Include suggested fix code in bug reports to accelerate resolution

**Atomicity**: 92%

**Category**: GitHub

**Context**: When reporting bugs in external repositories

**Evidence**: Session 40 - Issue #6 included PowerShell append code snippet

**Details**:

- Bug reports without solutions are harder to fix
- Suggesting code helps maintainers understand intent
- Shows you've diagnosed root cause
- Significantly increases chance of acceptance/PR

**Pattern - Minimal** ❌:

```markdown
# Issue: Script overwrites files

The install-claude-repo.ps1 script replaces CLAUDE.md instead of appending.
This is bad because we lose project configuration.

Please fix.
```

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

**Issue Template with Code**:

```markdown
# [Type] [Brief Description]

## Expected Behavior
What should happen?

## Actual Behavior
What actually happens?

## Steps to Reproduce
1. Step 1
2. Step 2
3. Step 3

## Suggested Fix
\`\`\`[language]
[Your proposed code change]
\`\`\`

## Why This Fix Works
[Explanation]

## Alternative Approaches
- [Other possible fixes]
- [Trade-offs]

## Environment
- Tool version: X.Y.Z
- OS: Windows/Linux/macOS
```

**Real Example** (Session 40 - Issue #6):

```markdown
# Script overwrites CLAUDE.md when installing agents

## Problem
Running `install-claude-repo.ps1` on a repo with existing CLAUDE.md
replaces the file instead of appending new content.

## Root Cause
Out-File uses default behavior (replace) instead of append.

## Fix
Line 45 in install-claude-repo.ps1:

FROM:
\`\`\`powershell
$content | Out-File -FilePath $claude_md_path -Encoding UTF8
\`\`\`

TO:
\`\`\`powershell
$content | Out-File -FilePath $claude_md_path -Encoding UTF8 -Append
\`\`\`

## Result
Script was updated with proposed fix and issue resolved ✓
```

**Benefits of Detailed Issues**:

| Element | Impact | Example |
|---------|--------|---------|
| Root cause | ↑ Speed | "Line 45 uses Out-File without -Append" |
| Code fix | ↑ Clarity | Show exact change needed |
| Impact statement | ↑ Priority | "Affects all installations on existing repos" |
| Reproduction steps | ↑ Acceptance | Easy to verify fix works |
| Test case | ↑ Confidence | CI validation of fix |

**Submitting Code Suggestions**:

```bash
# Create PR with your fix
gh repo fork repo-name
git clone https://github.com/you/repo-name
cd repo-name
# Make fix
git commit -m "fix: append to CLAUDE.md instead of replacing"
gh pr create --title "fix: append to CLAUDE.md" --body "Fixes #6"

# Or just propose it in the issue
gh issue comment [issue] -b "Proposed fix: [code block]"
```

**Key Principles**:

1. **Show your work**: How did you find the bug?
2. **Propose solution**: What code change fixes it?
3. **Prove it works**: Include reproduction + test
4. **Consider alternatives**: Why this approach?
5. **Make it easy**: Clear, actionable fix
