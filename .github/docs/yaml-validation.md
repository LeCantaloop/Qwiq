# YAML Validation Guide

This repository uses **automatic YAML validation and formatting** via pre-commit hooks. **Agents should NEVER manually validate YAML files.**

## For GitHub Copilot Agents

**DO NOT run manual validation commands.** YAML files are automatically validated by the pre-commit hook.

### ✅ Correct Workflow (Zero Tokens)

After editing YAML files, just commit:

```bash
git add .github/workflows/my-workflow.yml
git commit -m "feat: add workflow"
# ✓ Pre-commit hook auto-formats with pprettier
# ✓ Pre-commit hook validates syntax
# ✓ Files are re-staged if fixed
# ✓ Commit proceeds if valid
```

**No additional validation needed.** The pre-commit hook handles everything automatically.

### ❌ DO NOT Do This (Wastes Tokens)

**NEVER run these commands after editing YAML:**

```bash
# ❌ NEVER DO THIS - wastes tokens
python3 -c "import yaml; yaml.safe_load(open('file.yml'))"

# ❌ NEVER DO THIS - wastes tokens
./.github/scripts/validate-yaml.ps1 file.yml

# ❌ NEVER DO THIS - wastes tokens
dotnet pprettier --check file.yml
```

**Why?** The pre-commit hook automatically runs these checks. Running them manually wastes tokens in an unnecessary OODA loop.

## How It Works

### 1. Pre-Commit Hook (.githooks/pre-commit)

**Automatically enabled** via `copilot-setup-steps.yml` workflow.

The hook automatically:

1. Detects staged YAML files
2. Runs `dotnet pprettier --write` to fix formatting
3. Validates syntax
4. Re-stages fixed files
5. Allows commit if valid, blocks if syntax errors remain

**Configuration:**

```bash
# Already configured - no action needed
git config core.hooksPath .githooks
```

### 2. For Human Developers Only

If you need to manually format (not for agents):

```bash
# Format single file
dotnet pprettier --write .github/workflows/main.yml

# Format all YAML files
dotnet pprettier --write "**/*.{yml,yaml}"
```

### 3. Environment Variable

```bash
# Auto-fix mode (default)
git commit

# Check-only mode (CI)
SKIP_AUTOFIX=1 git commit
```

## Common YAML Issues (Auto-Fixed)

| Issue                    | Auto-Fixed? | Example             |
| ------------------------ | ----------- | ------------------- |
| Inconsistent indentation | ✓ Yes       | 2 vs 4 spaces       |
| Trailing whitespace      | ✓ Yes       | `value:` → `value:` |
| Missing newline at EOF   | ✓ Yes       | Adds `\n`           |
| Line length              | ✓ Yes       | Wraps long lines    |
| Quote style              | ✓ Yes       | Normalizes quotes   |
| **Syntax errors**        | ✗ No        | Must fix manually   |
| **Duplicate keys**       | ✗ No        | Must fix manually   |

## Syntax Errors (Manual Fix Required)

### Invalid Indentation

```yaml
# ❌ WRONG
jobs:
  build:
  runs-on: ubuntu-latest # Not indented properly
```

```yaml
# ✓ CORRECT
jobs:
  build:
    runs-on: ubuntu-latest
```

### Duplicate Keys

```yaml
# ❌ WRONG
jobs:
  build:
    name: Build
    name: Build Again  # Duplicate!
```

### Tab Characters

```yaml
# ❌ WRONG (uses tabs - shown as →)
jobs:
→ build:

# ✓ CORRECT (uses spaces)
jobs:
  build:
```

**Error:** Python will show `found character '\t' that cannot start any token`

## GitHub Actions-Specific

### Required Job Name

For `copilot-setup-steps.yml`:

```yaml
jobs:
  # MUST be named exactly "copilot-setup-steps"
  copilot-setup-steps:
    runs-on: ubuntu-latest
```

Reference: [Customize Copilot Agent Environment](https://docs.github.com/en/copilot/how-tos/use-copilot-agents/coding-agent/customize-the-agent-environment)

## Troubleshooting

### Pre-commit hook not running

```bash
# Re-enable git hooks
git config core.hooksPath .githooks

# Or run setup script
bash .github/copilot-setup.sh
```

### pprettier not found

```bash
# Restore dotnet tools
dotnet tool restore
```

## For Agents: Token Efficiency Rules

| Action                    | Allowed? | Reason                                  |
| ------------------------- | -------- | --------------------------------------- |
| Edit YAML, then commit    | ✅ YES   | Pre-commit hook validates automatically |
| Run Python validation     | ❌ NO    | Wastes tokens - pre-commit does this    |
| Run validation script     | ❌ NO    | Wastes tokens - pre-commit does this    |
| Run dotnet pprettier      | ❌ NO    | Wastes tokens - pre-commit does this    |
| Check validation manually | ❌ NO    | Wastes tokens - pre-commit does this    |

**Rule:** After editing YAML, just commit. The pre-commit hook handles all validation.

## For CI/CD (Optional Validation Script)

A PowerShell validation script is available for CI/CD pipelines: `.github/scripts/Validate-Yaml.ps1`

**Features:**

- Uses `dotnet pprettier` for formatting and syntax validation (already in repo)
- Optionally uses `yamllint` for enhanced linting (if installed)
- Cross-platform (Windows, Linux, macOS)

**Requirements:**

- PowerShell 7+ (pwsh)
- dotnet pprettier (via `dotnet tool restore`)
- yamllint (optional): `pip install yamllint`

**Usage:**

```powershell
# Formatting + syntax validation only
pwsh .github/scripts/Validate-Yaml.ps1 .github/workflows/main.yml

# With yamllint checks (if installed)
pwsh .github/scripts/Validate-Yaml.ps1 .github/workflows/main.yml

# Skip yamllint, only pprettier
pwsh .github/scripts/Validate-Yaml.ps1 .github/workflows/main.yml -SkipLint
```

**Simple approach:** The script uses existing tools (pprettier) + optional linter (yamllint). No custom YAML parsing to maintain.

## References

- [Pre-commit hook](../.githooks/pre-commit) - Auto-fix implementation
- [pprettier](https://github.com/belav/csharpier/tree/main/Src/PackedPrettier) - Dotnet tool for JSON/YAML formatting
- [GitHub Actions Workflow Syntax](https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions)
