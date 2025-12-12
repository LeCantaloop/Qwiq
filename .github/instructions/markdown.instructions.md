---
applyTo: "**/*.md"
---

# Markdown File Instructions

> **MANDATORY**: You MUST follow these instructions when editing any Markdown file in this repository.

## Quick Reference

- Use proper heading hierarchy (H1 → H2 → H3)
- Add language identifiers to code blocks (MD040)
- Add blank lines around code blocks (MD031) and tables (MD058)
- Use reference-style links for repeated URLs
- No bare URLs - use `<url>` or `[text](url)` (MD034)
- Follow conventional commit format for PR descriptions

## Linting Configuration

This repository uses markdownlint and Prettier for markdown formatting:

- **Configuration**: `.markdownlint-cli2.yaml`, `.prettierrc`
- **Run linting**: `dotnet pprettier --check "**/*.md"`
- **Auto-fix**: `dotnet pprettier --write "**/*.md"`

## Context Loading

When working on Markdown files, you MUST:

1. Read this entire instruction file before making changes
2. Preview rendered output before submitting
3. Check for broken links
4. Run `dotnet pprettier --check` to verify formatting
5. Complete the Validation Checklist before submitting

## Markdown Standards

### Headings

```markdown
# Document Title (H1 - only one per file)

## Major Section (H2)

### Subsection (H3)
```

### Code Blocks

Always include language identifiers:

````markdown
```csharp
public void Method() { }
```

```powershell
dotnet build Qwiq.sln
```

```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
</PropertyGroup>
```
````

### Tables

Use consistent formatting:

```markdown
| Column 1 | Column 2 | Column 3 |
| -------- | -------- | -------- |
| Value 1  | Value 2  | Value 3  |
```

### Links

```markdown
<!-- Inline links for one-time use -->

See the [README](../README.md) for details.

<!-- Reference links for repeated URLs -->

Check the [documentation][docs] and [API reference][docs].

[docs]: https://github.com/rjmurillo/Qwiq

## Quality Guidance

- **Keep related info together**: Use clear headings and avoid scattering a topic across distant sections.
- **Link to the source file**: When you mention constants or settings, point to `TestData.cs`, `Directory.Build.props`, or the correct file instead of copying values.
- **Call out test impact**: If a doc update changes how tests run, highlight the new steps so readers do not miss them.
- **Avoid doubles**: If guidance already exists in another instruction file, link to it rather than repeating the same text.
```

## Special Files

### copilot-instructions.md

The main Copilot instructions file. Changes should:

- Document patterns discovered during development
- Keep examples current with codebase
- Update "Trust These Instructions" when major changes occur

### README.md

Repository overview. Changes should:

- Keep installation instructions current
- Update badges if build status changes
- Maintain accurate code examples

### PR Descriptions

Follow this structure:

```markdown
# Title

## Summary

Brief description of changes.

## Changes

- Specific change 1
- Specific change 2

## Testing

- [ ] Build succeeds
- [ ] Tests pass
- [ ] Documentation updated

## Related

- Fixes #123
- Related to #456
```

## Validation Checklist

Before submitting changes, verify:

- [ ] Headings follow proper hierarchy
- [ ] Code blocks have language identifiers
- [ ] Blank lines around code blocks and tables
- [ ] Links are not broken (no bare URLs)
- [ ] Tables render correctly
- [ ] No trailing whitespace
- [ ] Linting passes: `dotnet pprettier --check "**/*.md"`
- [ ] Guidance avoids duplicate identity/configuration details by linking to canonical sources
- [ ] File ends with newline

## Common Mistakes to AVOID

```markdown
<!-- ❌ WRONG: No language identifier -->
```

dotnet build

````

<!-- ✅ CORRECT: With language identifier -->
```powershell
dotnet build
````

<!-- ❌ WRONG: Bold instead of heading -->

**Section Title**

<!-- ✅ CORRECT: Proper heading -->

## Section Title

<!-- ❌ WRONG: Inconsistent list markers -->

- Item 1

* Item 2

<!-- ✅ CORRECT: Consistent list markers -->

- Item 1
- Item 2

```

## Decision Trees

### When Updating Documentation

1. Is this a code change? → Update related docs
2. Is this a new feature? → Add usage examples
3. Is this a breaking change? → Document migration path

### When to Stop and Ask

- Major restructuring of documentation
- Changing installation instructions
- Modifying contribution guidelines

## Related Instruction Files

- [generic.instructions.md](generic.instructions.md) - For multi-file changes
- [csharp.instructions.md](csharp.instructions.md) - For code examples in docs
```
