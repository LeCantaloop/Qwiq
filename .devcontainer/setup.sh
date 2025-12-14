#!/usr/bin/env bash
#
# DevContainer Post-Create Setup Script
# Automatically configures the development environment for Qwiq
#
# This script runs after the devcontainer is created to:
# 1. Enable git hooks for automatic linting
# 2. Restore dotnet tools (nbgv for versioning)
# 3. Install npm packages for markdown linting
#

set -e

# Colors for output
GREEN='\033[0;32m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo_success() {
    echo -e "${GREEN}✓${NC} $1"
}

echo_info() {
    echo -e "${CYAN}ℹ${NC} $1"
}

# Get repository root
REPO_ROOT=$(git rev-parse --show-toplevel 2>/dev/null || pwd)
cd "$REPO_ROOT"

echo_info "Setting up Qwiq development environment..."
echo ""

# 1. Enable git hooks
echo_info "Enabling git hooks..."
git config core.hooksPath .githooks
echo_success "Git hooks enabled at .githooks/"

# 2. Restore dotnet tools
echo_info "Restoring dotnet tools..."
if command -v dotnet &> /dev/null; then
    # Try to restore tools, but don't fail if SDK version mismatch
    if dotnet tool restore 2>/dev/null; then
        echo_success "Dotnet tools restored"
    else
        echo "Warning: dotnet tool restore failed (possible SDK version mismatch)"
        echo "  You may need to install the SDK version specified in global.json"
    fi
else
    echo "Warning: dotnet CLI not found, skipping tool restore"
fi

# 3. Install npm packages (for markdownlint-cli2)
echo_info "Installing npm packages..."
if command -v npm &> /dev/null; then
    npm install --global markdownlint-cli2
    echo_success "markdownlint-cli2 installed"
else
    echo "Warning: npm not found, skipping npm package installation"
fi

# 4. Verify environment
echo ""
echo_info "Environment setup complete!"
echo ""
echo "Git hooks: $(git config --get core.hooksPath)"
echo "SKIP_AUTOFIX: ${SKIP_AUTOFIX:-0} (0=enabled, 1=disabled)"
echo ""
echo_success "Ready to develop! Pre-commit hooks will auto-fix linting issues."
