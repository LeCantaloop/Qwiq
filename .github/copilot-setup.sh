#!/usr/bin/env bash
#
# GitHub Copilot Workspace Setup Script
# Automatically configures git hooks and linting tools for Copilot Workspace
#
# This script can be run manually in Copilot Workspace environments:
#   bash .github/copilot-setup.sh
#
# Or triggered via the copilot-setup-steps.yml workflow
#

set -e

# Colors for output
GREEN='\033[0;32m'
CYAN='\033[0;36m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo_success() {
    echo -e "${GREEN}✓${NC} $1"
}

echo_info() {
    echo -e "${CYAN}ℹ${NC} $1"
}

echo_warning() {
    echo -e "${YELLOW}⚠${NC} $1"
}

# Get repository root
REPO_ROOT=$(git rev-parse --show-toplevel 2>/dev/null || pwd)
cd "$REPO_ROOT"

echo_info "Setting up Qwiq development environment for GitHub Copilot Workspace..."
echo ""

# 1. Check and install .NET SDK
echo_info "Checking .NET SDK..."
REQUIRED_SDK_VERSION=""
if [ -f "global.json" ]; then
    # Extract SDK version from global.json
    REQUIRED_SDK_VERSION=$(grep -o '"version"[[:space:]]*:[[:space:]]*"[^"]*"' global.json | sed 's/"version"[[:space:]]*:[[:space:]]*"\([^"]*\)"/\1/')
    echo "Required .NET SDK version: $REQUIRED_SDK_VERSION"
fi

if command -v dotnet &> /dev/null; then
    CURRENT_SDK=$(dotnet --version 2>/dev/null || echo "unknown")
    echo "Current .NET SDK version: $CURRENT_SDK"
    
    # Check if the required SDK version is installed
    if [ -n "$REQUIRED_SDK_VERSION" ]; then
        if dotnet --list-sdks 2>/dev/null | grep -q "^$REQUIRED_SDK_VERSION"; then
            echo_success ".NET SDK $REQUIRED_SDK_VERSION is already installed"
        else
            echo_warning ".NET SDK $REQUIRED_SDK_VERSION is not installed"
            echo ""
            echo "To install .NET SDK $REQUIRED_SDK_VERSION:"
            echo "  1. Visit: https://dotnet.microsoft.com/download/dotnet"
            echo "  2. Download and install .NET SDK $REQUIRED_SDK_VERSION"
            echo "  3. Or use the dotnet-install script:"
            echo ""
            echo "  # Linux/macOS:"
            echo "  curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version $REQUIRED_SDK_VERSION"
            echo ""
            echo "  # Windows (PowerShell):"
            echo "  Invoke-WebRequest https://dot.net/v1/dotnet-install.ps1 -OutFile dotnet-install.ps1"
            echo "  ./dotnet-install.ps1 -Version $REQUIRED_SDK_VERSION"
            echo ""
            echo "Attempting to install .NET SDK $REQUIRED_SDK_VERSION..."
            
            # Try to install using dotnet-install script
            if curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh 2>/dev/null; then
                chmod +x /tmp/dotnet-install.sh
                if /tmp/dotnet-install.sh --version "$REQUIRED_SDK_VERSION" --install-dir "$HOME/.dotnet" 2>/dev/null; then
                    echo_success ".NET SDK $REQUIRED_SDK_VERSION installed to $HOME/.dotnet"
                    export PATH="$HOME/.dotnet:$PATH"
                    export DOTNET_ROOT="$HOME/.dotnet"
                    echo "Added to PATH for current session"
                    echo ""
                    echo "To persist, add to your shell profile:"
                    echo "  echo 'export PATH=\"\$HOME/.dotnet:\$PATH\"' >> ~/.bashrc"
                    echo "  echo 'export DOTNET_ROOT=\"\$HOME/.dotnet\"' >> ~/.bashrc"
                else
                    echo_warning "Failed to install .NET SDK automatically"
                    echo "Please install manually using the instructions above"
                fi
                rm -f /tmp/dotnet-install.sh
            else
                echo_warning "Could not download dotnet-install script"
                echo "Please install .NET SDK manually using the instructions above"
            fi
        fi
    fi
else
    echo_warning "dotnet CLI not found"
    if [ -n "$REQUIRED_SDK_VERSION" ]; then
        echo ""
        echo "To install .NET SDK $REQUIRED_SDK_VERSION:"
        echo "  1. Visit: https://dotnet.microsoft.com/download/dotnet"
        echo "  2. Download and install .NET SDK $REQUIRED_SDK_VERSION"
        echo "  3. Or use the dotnet-install script:"
        echo ""
        echo "  # Linux/macOS:"
        echo "  curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version $REQUIRED_SDK_VERSION"
        echo ""
        echo "Attempting to install .NET SDK $REQUIRED_SDK_VERSION..."
        
        # Try to install using dotnet-install script
        if curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh 2>/dev/null; then
            chmod +x /tmp/dotnet-install.sh
            if /tmp/dotnet-install.sh --version "$REQUIRED_SDK_VERSION" --install-dir "$HOME/.dotnet" 2>/dev/null; then
                echo_success ".NET SDK $REQUIRED_SDK_VERSION installed to $HOME/.dotnet"
                export PATH="$HOME/.dotnet:$PATH"
                export DOTNET_ROOT="$HOME/.dotnet"
                echo "Added to PATH for current session"
                echo ""
                echo "To persist, add to your shell profile:"
                echo "  echo 'export PATH=\"\$HOME/.dotnet:\$PATH\"' >> ~/.bashrc"
                echo "  echo 'export DOTNET_ROOT=\"\$HOME/.dotnet\"' >> ~/.bashrc"
            else
                echo_warning "Failed to install .NET SDK automatically"
                echo "Please install manually"
            fi
            rm -f /tmp/dotnet-install.sh
        else
            echo_warning "Could not download dotnet-install script"
            echo "Please install .NET SDK manually"
        fi
    fi
fi

echo ""

# 2. Enable git hooks
echo_info "Enabling git hooks..."
git config core.hooksPath .githooks
echo_success "Git hooks enabled at .githooks/"

# 3. Set SKIP_AUTOFIX environment variable
echo_info "Setting environment variables..."
export SKIP_AUTOFIX=0
echo_success "SKIP_AUTOFIX=0 (auto-fix enabled)"

# 4. Restore dotnet tools
echo_info "Restoring dotnet tools..."
if command -v dotnet &> /dev/null; then
    # Try to restore tools, but don't fail if SDK version mismatch
    if dotnet tool restore 2>/dev/null; then
        echo_success "Dotnet tools restored"
    else
        echo_warning "dotnet tool restore failed"
        echo "This usually means the .NET SDK version doesn't match global.json"
        if [ -n "$REQUIRED_SDK_VERSION" ]; then
            echo "Required: .NET SDK $REQUIRED_SDK_VERSION"
        fi
    fi
else
    echo_warning "dotnet CLI not found, skipping tool restore"
fi

# 5. Install npm packages (for markdownlint-cli2)
echo_info "Installing npm packages..."
if command -v npm &> /dev/null; then
    npm install --global markdownlint-cli2
    echo_success "markdownlint-cli2 installed"
else
    echo_warning "npm not found"
    echo ""
    echo "To install Node.js and npm:"
    echo "  1. Visit: https://nodejs.org/"
    echo "  2. Download and install Node.js LTS"
    echo "  3. Or use nvm (Node Version Manager):"
    echo ""
    echo "  # Install nvm:"
    echo "  curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.0/install.sh | bash"
    echo "  source ~/.bashrc"
    echo "  nvm install --lts"
fi

# 6. Verify environment
echo ""
echo_info "Environment setup complete!"
echo ""
echo "Git hooks: $(git config --get core.hooksPath)"
echo "SKIP_AUTOFIX: ${SKIP_AUTOFIX:-0} (0=enabled, 1=disabled)"
echo ""

# Test that tools are available
if command -v npx &> /dev/null && npx markdownlint-cli2 --help &> /dev/null; then
    echo_success "markdownlint-cli2 is ready"
fi

if command -v dotnet &> /dev/null && dotnet tool list 2>/dev/null | grep -q "nbgv"; then
    echo_success "nbgv tool is ready"
fi

echo ""
echo_success "Ready to develop! Pre-commit hooks will auto-fix linting issues."
echo ""
echo "To add SKIP_AUTOFIX to your shell profile, run:"
echo "  echo 'export SKIP_AUTOFIX=0' >> ~/.bashrc"
