# GitHub Copilot Workspace Setup

This directory contains setup automation for GitHub Copilot Workspace environments.

## Quick Setup

Run the setup script once after opening the workspace:

```bash
bash .github/copilot-setup.sh
```

This will:

- ✓ Check and install .NET SDK (version from `global.json`)
- ✓ Enable git hooks at `.githooks/`
- ✓ Set `SKIP_AUTOFIX=0` (auto-fix mode enabled)
- ✓ Restore dotnet tools (nbgv)
- ✓ Install markdownlint-cli2
- ✓ Provide installation instructions for missing prerequisites

### What Gets Installed

The script automatically detects and installs missing prerequisites:

1. **.NET SDK** - If not installed or wrong version, the script will:

   - Download the .NET install script from Microsoft
   - Install the exact version specified in `global.json` to `~/.dotnet`
   - Add it to your PATH for the current session
   - Provide instructions to persist the PATH change

2. **Node.js/npm** - If not installed, provides installation instructions:

   - Direct download from <https://nodejs.org/>
   - Or via nvm (Node Version Manager)

3. **Git hooks** - Always configured automatically

4. **Linting tools** - markdownlint-cli2 and dotnet tools (nbgv)

## Files

- `copilot-setup.sh` - Setup script for Copilot Workspace (run manually)
- `workflows/copilot-setup-steps.yml` - GitHub Actions workflow for automated setup

## GitHub Actions Workflow

The `copilot-setup-steps.yml` workflow can be triggered manually via:

```bash
gh workflow run copilot-setup-steps.yml
```

Or from the GitHub Actions UI:

1. Go to Actions tab
2. Select "Copilot Workspace Setup"
3. Click "Run workflow"

## Comparison with DevContainer

| Feature           | DevContainer           | Copilot Workspace          |
| ----------------- | ---------------------- | -------------------------- |
| .NET SDK install  | Pre-installed in image | Auto-installed if missing  |
| Node.js install   | Pre-installed in image | Manual (with instructions) |
| Git hooks setup   | Automatic (postCreate) | Automatic (in script)      |
| SKIP_AUTOFIX      | Set via containerEnv   | Set via script             |
| Dotnet tools      | Automatic restore      | Automatic restore          |
| markdownlint-cli2 | Automatic install      | Automatic install          |
| Trigger           | Container creation     | Manual script execution    |

## Environment Variables

| Variable       | Default | Description                                      |
| -------------- | ------- | ------------------------------------------------ |
| `SKIP_AUTOFIX` | `0`     | Controls auto-fix mode (0=enabled, 1=check only) |

To persist `SKIP_AUTOFIX` across sessions, add to your shell profile:

```bash
echo 'export SKIP_AUTOFIX=0' >> ~/.bashrc
source ~/.bashrc
```

## Verification

After running the setup script, verify the configuration:

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

### .NET SDK version mismatch

If you see the warning about SDK version mismatch, the script will attempt to install the correct version automatically. If automatic installation fails:

**Manual installation options:**

1. **Using the dotnet-install script** (Linux/macOS):

   ```bash
   curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 10.0.101
   export PATH="$HOME/.dotnet:$PATH"
   export DOTNET_ROOT="$HOME/.dotnet"
   ```

2. **Using the dotnet-install script** (Windows PowerShell):

   ```powershell
   Invoke-WebRequest https://dot.net/v1/dotnet-install.ps1 -OutFile dotnet-install.ps1
   ./dotnet-install.ps1 -Version 10.0.101
   ```

3. **Direct download from Microsoft**:
   - Visit <https://dotnet.microsoft.com/download/dotnet/10.0>
   - Download the SDK installer for your platform
   - Run the installer

After installing, verify:

```bash
dotnet --version
# Should output: 10.0.101

dotnet --list-sdks
# Should include: 10.0.101 [path]
```

### Git hooks not running

Re-run the setup script:

```bash
bash .github/copilot-setup.sh
```

Or manually configure:

```bash
git config core.hooksPath .githooks
```

### SDK version mismatch

The script will warn but continue if the .NET SDK version doesn't match `global.json`. You can:

1. Install the required SDK version (check `global.json`)
2. Or update `global.json` to use an installed SDK version

### npm packages not found

The script will provide installation instructions if Node.js/npm is not found. To install:

#### Option 1: Direct download

Visit <https://nodejs.org/> and download the LTS version for your platform.

#### Option 2: Using nvm (recommended for development)

```bash
# Install nvm
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.0/install.sh | bash

# Reload shell configuration
source ~/.bashrc

# Install Node.js LTS
nvm install --lts

# Verify installation
node --version
npm --version
```

After installing Node.js, re-run the setup script to install markdownlint-cli2.

## Related Documentation

- [CONTRIBUTING.md](../CONTRIBUTING.md) - Git hooks documentation
- [.githooks/pre-commit](../.githooks/pre-commit) - Pre-commit hook implementation
- [.devcontainer/](../.devcontainer/) - DevContainer setup for VS Code/Codespaces
- [.agents/retrospective/2025-12-14-automated-git-hooks-setup.md](../.agents/retrospective/2025-12-14-automated-git-hooks-setup.md) - Background on automated setup
