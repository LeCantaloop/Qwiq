# Qwiq Skills Index

**Date Extracted**: 2025-12-13  
**Total Skills**: 22  
**Atomicity Average**: 94%  
**Source**: `.agents/skills/` directory

## Overview

This index maps all validated skills from the Qwiq project, organized by category. Skills are reusable strategies that have been validated through successful execution and provide citation patterns for future use.

## Categories

### SkillCategory-Build

Build and CI skills for dotnet builds

- **Skill-Build-001**: Single-threaded builds with `/m:1 /nodeReuse:false`

  - Purpose: Avoid Windows file locking issues
  - Context: CI environments, parallel builds failing
  - Validation: Build succeeds without file locking errors

- **Skill-Build-002**: CI environment detection and optimization
  - Purpose: Optimize builds for CI vs local development
  - Context: Different build requirements for CI/local
  - Validation: Builds succeed in both environments

### SkillCategory-Testing

Testing skills for WireMock, test patterns, and verification

- **Skill-Testing-001**: WireMock integration test setup

  - Purpose: Configure WireMock for REST API testing
  - Context: Testing REST clients without live servers
  - Validation: Tests can mock Azure DevOps REST API responses

- **Skill-Testing-002**: ContextSpecification test pattern

  - Purpose: Use Given-When-Then structure for unit tests
  - Context: Writing maintainable, readable tests
  - Validation: Tests follow consistent pattern

- **Skill-Testing-003**: Test categorization and filtering
  - Purpose: Exclude integration tests from CI runs
  - Context: Different test suites for different environments
  - Validation: Unit tests run fast, integration tests skipped

### SkillCategory-Quality

Code quality skills for CA analyzers

- **Skill-Quality-001**: CA analyzer configuration in Directory.Build.props

  - Purpose: Centralize code analysis rules
  - Context: Consistent code quality across projects
  - Validation: Analyzers run correctly, no duplicate warnings

- **Skill-Quality-002**: EditorConfig vs analyzer severity distinction
  - Purpose: Understand tool separation
  - Context: Formatting vs analysis configuration
  - Validation: Correct tool configured for each concern

### SkillCategory-Documentation

Documentation skills for file management

- **Skill-Documentation-001**: Remove duplicate documentation files

  - Purpose: Eliminate conflicting information
  - Context: Multiple sources causing confusion
  - Validation: Single source of truth established

- **Skill-Documentation-002**: Cross-reference validation

  - Purpose: Ensure documentation references are valid
  - Context: Broken links, missing files
  - Validation: All references resolve correctly

- **Skill-Documentation-003**: Agent output organization
  - Purpose: Store agent artifacts in `.agents/` subdirectories
  - Context: Agent system producing many files
  - Validation: Files organized by agent type and purpose

### SkillCategory-Git

Git workflow and recovery skills

- **Skill-Git-001**: Atomic commits with conventional commit messages

  - Purpose: Clear git history with traceable changes
  - Context: Multiple changes needing separate commits
  - Validation: Each commit is independently understandable

- **Skill-Git-002**: Restaging changes after pre-commit hooks

  - Purpose: Recover from pre-commit hook modifications
  - Context: Hooks modify files, causing staging issues
  - Validation: Changes successfully committed after hook runs

- **Skill-Git-003**: PR linking with issue numbers in commits
  - Purpose: Auto-link commits to GitHub issues
  - Context: Tracking work against issues
  - Validation: Commits appear in issue timeline

### SkillCategory-Markdown

Markdown linting and formatting skills

- **Skill-Markdown-001**: markdownlint-cli2 with auto-fix

  - Purpose: Fix markdown lint errors automatically
  - Context: Linting failures in CI
  - Validation: `npx markdownlint-cli2 --fix` resolves errors

- **Skill-Markdown-002**: prettier integration for markdown

  - Purpose: Format markdown consistently
  - Context: Inconsistent formatting across files
  - Validation: `dotnet pprettier --write .` succeeds

- **Skill-Markdown-003**: Multi-tool linting workflow
  - Purpose: Combine markdownlint and prettier
  - Context: Different tools fixing different issues
  - Validation: Both tools run cleanly

### SkillCategory-Workflow

Workflow and installation skills

- **Skill-Workflow-001**: Orchestrator coordination pattern

  - Purpose: Manage multi-agent task execution
  - Context: Complex tasks requiring multiple agents
  - Validation: Agents execute in correct sequence

- **Skill-Installation-001**: PowerShell installation scripts

  - Purpose: Automate repository setup
  - Context: Onboarding new developers/agents
  - Validation: Script completes without errors

- **Skill-Installation-002**: Agent prompt installation
  - Purpose: Install agent prompts from repository
  - Context: Setting up agent system
  - Validation: Agents available in VS Code

### SkillCategory-Strategy

Strategic decision-making skills

- **Skill-Strategy-001**: Technical direction planning
  - Purpose: Make high-level architectural decisions
  - Context: Major feature planning, technology choices
  - Validation: Decision documented in ADR

### SkillCategory-DevEx

Developer experience and hooks

- **Skill-DevEx-001**: Pre-commit hook setup
  - Purpose: Automate quality checks before commit
  - Context: Preventing bad commits from entering history
  - Validation: Hooks run and enforce quality standards

### SkillCategory-GitHub

GitHub API and issue management

- **Skill-GitHub-001**: GitHub CLI for issue creation

  - Purpose: Create issues programmatically
  - Context: Bulk issue creation, automation
  - Validation: `gh issue create` succeeds

- **Skill-GitHub-002**: Label management patterns
  - Purpose: Consistent issue labeling
  - Context: Organizing issues by type/priority
  - Validation: Labels applied correctly

## Skill Citation Pattern

When applying a skill, use this format:

```markdown
**Applying**: [Skill-ID]
**Strategy**: [Brief description]
**Expected Outcome**: [What should happen]

[Execute...]

**Result**: [Actual outcome]
**Skill Validated**: Yes | No | Partial
**Feedback**: [Note for retrospective]
```

## Atomicity Metrics

- Average atomicity across all skills: 94%
- Skills are designed to be independently applicable
- Each skill has clear context, validation criteria, and outcomes

## Relations

- **QwiqSkillsIndex** `categorizes` **SkillCategory-Build**
- **QwiqSkillsIndex** `categorizes` **SkillCategory-Testing**
- **QwiqSkillsIndex** `categorizes` **SkillCategory-Quality**
- **QwiqSkillsIndex** `categorizes` **SkillCategory-Documentation**
- **QwiqSkillsIndex** `categorizes` **SkillCategory-Git**
- **QwiqSkillsIndex** `categorizes` **SkillCategory-Markdown**
- **QwiqSkillsIndex** `categorizes` **SkillCategory-Workflow**
- **QwiqSkillsIndex** `categorizes` **SkillCategory-Strategy**
- **QwiqSkillsIndex** `categorizes` **SkillCategory-DevEx**
- **QwiqSkillsIndex** `categorizes` **SkillCategory-GitHub**

## Usage

This index serves as a quick reference for:

1. Finding validated strategies for common tasks
2. Understanding skill organization and relationships
3. Citing skills during execution
4. Identifying gaps in skill coverage

For detailed skill definitions, see individual skill files in `.agents/skills/`.
