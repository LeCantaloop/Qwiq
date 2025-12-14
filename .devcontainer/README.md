# Qwiq Development Container

This directory contains the configuration for the Qwiq development container, which provides a consistent development environment for GitHub Codespaces, VS Code Remote Containers, and other devcontainer-compatible tools.

**For GitHub Copilot Workspace users**: See [../.github/copilot-setup.sh](../.github/copilot-setup.sh) for Copilot Workspace setup.

## What Gets Configured

The devcontainer automatically sets up:

1. **Git Hooks**: Pre-commit hooks are automatically enabled via `git config core.hooksPath .githooks`
2. **Auto-fix Mode**: `SKIP_AUTOFIX=0` environment variable ensures linting issues are automatically fixed
3. **Dotnet Tools**: Nerdbank.GitVersioning (nbgv) and other tools are restored
4. **Markdown Linting**: markdownlint-cli2 is installed globally for markdown validation

## Files

- `devcontainer.json` - Devcontainer configuration
- `setup.sh` - Post-create setup script (runs automatically)
- `README.md` - This file

## Usage

### GitHub Codespaces

1. Open the repository in GitHub
2. Click "Code" → "Codespaces" → "Create codespace on [branch]"
3. Wait for container to build and setup to complete
4. Git hooks are automatically enabled - commits will auto-fix linting issues

### VS Code Remote Containers

1. Install the "Dev Containers" extension in VS Code
2. Open the repository in VS Code
3. Press F1 → "Dev Containers: Reopen in Container"
4. Wait for container to build and setup to complete

## Environment Variables

| Variable        | Default | Description                                      |
| --------------- | ------- | ------------------------------------------------ |
| `SKIP_AUTOFIX`  | `0`     | Controls auto-fix mode (0=enabled, 1=check only) |

## Verification

After the container is created, you can verify the setup:

```bash
# Check git hooks path
git config --get core.hooksPath
# Should output: .githooks

# Check SKIP_AUTOFIX
echo $SKIP_AUTOFIX
# Should output: 0

# Verify dotnet tools
dotnet tool list
# Should include: nbgv

# Verify markdownlint
npx markdownlint-cli2 --help
# Should display help text
```

## Troubleshooting

### Git hooks not working

If git hooks are not running, manually enable them:

```bash
git config core.hooksPath .githooks
```

### Linting tools not found

Re-run the setup script:

```bash
bash .devcontainer/setup.sh
```

## Related Documentation

- [CONTRIBUTING.md](../CONTRIBUTING.md) - Git hooks documentation
- [.githooks/pre-commit](../.githooks/pre-commit) - Pre-commit hook implementation
- [.agents/retrospective/markdown-lint-incident.md](../.agents/retrospective/markdown-lint-incident.md) - Background on why automatic setup is important
