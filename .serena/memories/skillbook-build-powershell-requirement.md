# Skill: Use PowerShell for .NET Builds

## ID

Skill-Build-002

## Problem

When running dotnet build commands from Claude Code (which uses bash), the .NET 10 SDK incorrectly parses MSBuild switches like `/m:1` and `/nodeReuse:false`, interpreting them as separate arguments instead of build properties.

## Symptoms

- Error: "MSB1008: Only one project can be specified"
- Build command shows switches being treated as separate arguments in the error message
- Same command works fine in PowerShell terminal

## Solution

Always wrap dotnet commands in PowerShell when running from Claude Code:

```bash
# WRONG - bash/Claude Code shell
dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false

# CORRECT - wrap in PowerShell
pwsh -NoProfile -NonInteractive -Command "dotnet build Qwiq.sln -c Release /m:1 /nodeReuse:false"
```

## CI Build Template

```bash
pwsh -NoProfile -NonInteractive -Command "dotnet build Qwiq.sln -c Release /p:ContinuousIntegrationBuild=true /p:UseSharedCompilation=false /m:1 /nodeReuse:false"
```

## Test Template

```bash
pwsh -NoProfile -NonInteractive -Command "dotnet test Qwiq.sln -c Release --no-build --filter 'TestCategory!=localOnly'"
```

## Related Skills

- Skill-Build-001: Use /m:1 /nodeReuse:false for CI builds

## Learned

2025-12-14 - Session 40

## Source

Pre-flight retrospective after multiple failed build attempts
