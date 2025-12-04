---
applyTo: "**/*.json"
---

# JSON File Instructions

> **MANDATORY**: You MUST follow these instructions when editing any JSON file in this repository.

## Quick Reference

- `global.json` - .NET SDK version pinning
- `.config/dotnet-tools.json` - Dotnet tool manifest
- `nuget.config` - NuGet package sources
- `version.json` - Nerdbank.GitVersioning configuration

## Context Loading

When working on JSON files, you MUST:

1. Read this entire instruction file before making changes
2. Understand the purpose of the specific JSON file
3. Validate JSON syntax
4. Complete the Validation Checklist before submitting

## Key Configuration Files

### global.json

Pins the .NET SDK version for reproducible builds:

```json
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "latestPatch"
  }
}
```

**Rules:**

- Only update when explicitly tasked with SDK upgrade
- Use `latestPatch` for security updates
- Test build after any changes

### .config/dotnet-tools.json

Dotnet tool manifest for Nerdbank.GitVersioning:

```json
{
  "version": 1,
  "isRoot": true,
  "tools": {
    "nbgv": {
      "version": "3.6.133",
      "commands": ["nbgv"]
    }
  }
}
```

**Rules:**

- Update via `dotnet tool update nbgv`
- Commit changes to manifest
- Test `dotnet tool restore` after changes

### version.json

Nerdbank.GitVersioning configuration:

```json
{
  "$schema": "https://raw.githubusercontent.com/dotnet/Nerdbank.GitVersioning/master/src/NerdBank.GitVersioning/version.schema.json",
  "version": "2.0",
  "publicReleaseRefSpec": ["^refs/heads/master$"],
  "cloudBuild": {
    "setVersionVariables": true,
    "buildNumber": {
      "enabled": true
    }
  }
}
```

**Rules:**

- Version is managed by GitVersioning
- Don't modify version numbers directly
- Changes affect all package versions

### nuget.config

NuGet package sources:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

**Rules:**

- Do not add private feeds without approval
- Do not modify as part of feature PRs

## Validation Checklist

Before submitting changes, verify:

- [ ] JSON syntax is valid
- [ ] File purpose is understood
- [ ] Build succeeds after changes
- [ ] Tools restore correctly (`dotnet tool restore`)
- [ ] No breaking changes to CI/CD

## Common Mistakes to AVOID

```json
// ❌ WRONG: Comments in JSON (not valid JSON)
{
  "version": "1.0" // This is invalid
}

// ❌ WRONG: Trailing commas
{
  "key": "value",
}

// ❌ WRONG: Changing SDK version casually
{
  "sdk": {
    "version": "9.0.100"  // Major version change needs planning
  }
}
```

## Decision Trees

### When Updating SDK Version

1. Is this a patch update? → Usually safe, test build
2. Is this a minor update? → Review release notes, test thoroughly
3. Is this a major update? → Requires dedicated PR, extensive testing

### When to Stop and Ask

- Changing SDK version
- Adding new tools to manifest
- Modifying NuGet sources
- Changing versioning strategy

## Related Instruction Files

- [project.instructions.md](project.instructions.md) - For project files that consume these
- [yaml.instructions.md](yaml.instructions.md) - For CI/CD that uses these settings
- [generic.instructions.md](generic.instructions.md) - For multi-file changes
