# Claude Skills for QWIQ

This directory contains Claude Skills - reusable capability modules that extend AI agent functionality through on-demand instruction injection.

## Skill Index

| Skill                                               | Description                                                  | Trigger Cues                                    |
| --------------------------------------------------- | ------------------------------------------------------------ | ----------------------------------------------- |
| [qwiq-csharp](./qwiq-csharp/SKILL.md)               | C# coding patterns, nullable reference types, and code style | C# files, nullable warnings, code review        |
| [qwiq-build](./qwiq-build/SKILL.md)                 | Build system, MSBuild, project configuration                 | Build errors, project files, package management |
| [qwiq-testing](./qwiq-testing/SKILL.md)             | Testing patterns, TDD, test categories                       | Writing tests, test failures, coverage          |
| [qwiq-cicd](./qwiq-cicd/SKILL.md)                   | CI/CD workflows and GitHub Actions                           | Workflow files, CI failures, deployment         |
| [nullable-migration](./nullable-migration/SKILL.md) | Nullable warning analysis and migration tracking             | CS8xxx warnings, nullable migration             |
| [sandbox-validation](./sandbox-validation/SKILL.md) | Integration test environment validation                      | Integration tests, sandbox setup                |
| [wiremock-capture](./wiremock-capture/SKILL.md)     | HTTP traffic capture for offline testing                     | WireMock, HAR files, traffic recording          |

## Three-Level Loading Model

Skills follow a progressive disclosure pattern:

### Level 1: Metadata (Always Loaded)

- `name` and `description` in SKILL.md frontmatter
- ~100 tokens per skill
- Used for skill selection

### Level 2: Instructions (Loaded on Trigger)

- Main body of SKILL.md
- Step-by-step procedures and constraints
- Loaded when task matches skill description

### Level 3: Resources (Loaded as Needed)

- REFERENCE.md, TEMPLATES.md files
- Helper scripts in scripts/ subdirectory
- Zero tokens until explicitly accessed

## Usage

When working on QWIQ tasks, the agent will:

1. Scan skill metadata to identify relevant skills
2. Load full instructions for matching skills
3. Access resources (scripts, references) as needed during execution

## Adding New Skills

1. Create a new directory under `claude/skills/`
2. Add `SKILL.md` with frontmatter and structured sections:
   ```yaml
   ---
   name: skill-name
   description: Brief description of what the skill does and when to use it.
   ---
   ```
3. Optionally add `REFERENCE.md`, `TEMPLATES.md`, and helper scripts in `scripts/`
4. Update this README's skill index table
5. Add entry to `SKILLS-INDEX.json` with triggers
