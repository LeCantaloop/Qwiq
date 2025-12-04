---
applyTo: ".editorconfig"
---

# .editorconfig Instructions

> **MANDATORY**: You MUST follow these instructions when editing .editorconfig in this repository.

## Quick Reference

- `.editorconfig` controls code style AND analyzer severity configuration
- Changes affect IDE behavior and build warnings/errors
- Run `dotnet format` after changes to verify formatting

## Context Loading

When working on .editorconfig, you MUST:

1. Read this entire instruction file before making changes
2. Understand the impact on code formatting and analyzers
3. Run `dotnet format` and `dotnet pprettier --write .` to verify changes
4. Complete the Validation Checklist before submitting

## File Structure

The Qwiq `.editorconfig` contains:

### Code Style Settings

```editorconfig
[*.cs]
indent_size = 4
indent_style = space
end_of_line = crlf
```

### Analyzer Severity Configuration

Diagnostic severities using `dotnet_diagnostic.<rule>.severity`:

```editorconfig
# Example: Suppress missing XML comment warning
dotnet_diagnostic.CS1591.severity = none

# Example: Enable a rule as warning
dotnet_diagnostic.CA1234.severity = warning
```

## Analyzer Categories

| Category      | Rules                              | Purpose                                      |
| ------------- | ---------------------------------- | -------------------------------------------- |
| CS warnings   | CS0618, CS1574, CS1591, etc.       | Compiler warnings                            |
| CS86xx        | CS8600-CS8769                      | Nullable reference types (gradual migration) |
| CS3xxx        | CS3001-CS3027                      | CLS compliance                               |
| CA1xxx        | CA1000-CA1070                      | Design rules                                 |
| CA13xx        | CA1303-CA1310                      | Globalization rules                          |
| CA15xx        | CA1501-CA1513                      | Maintainability rules                        |
| CA17xx        | CA1700-CA1725                      | Naming rules                                 |
| CA18xx        | CA1801-CA1863                      | Performance rules                            |
| CA2xxx        | CA2000-CA2254                      | Reliability/Usage rules                      |
| CA3xxx-CA5xxx | CA3001-CA5403                      | Security rules                               |
| IDE0xxx       | IDE0001-IDE1006                    | Code style rules                             |
| SYSLIB        | SYSLIB0021, SYSLIB0050, SYSLIB0051 | Obsolete API warnings                        |

## Making Changes

### To Suppress a New Warning

1. Add to appropriate section in `.editorconfig`:

   ```editorconfig
   # <Brief explanation of why this is suppressed>
   dotnet_diagnostic.CA1234.severity = none
   ```

2. Group with similar rules for organization

3. Add a comment explaining the suppression

### To Enable a Warning

1. Change severity from `none` to `warning` or `error`:

   ```editorconfig
   dotnet_diagnostic.CA1234.severity = warning
   ```

2. Fix all violations in the codebase, OR

3. Use file-level or project-level suppressions for specific cases

## Validation Checklist

Before submitting changes, verify:

- [ ] Run `dotnet format` to apply analyzer code fixes to C# files
- [ ] Run `dotnet pprettier --write .` to auto-fix all formatting
- [ ] `dotnet build Qwiq.sln -c Release` succeeds
- [ ] No unexpected new warnings appear
- [ ] Tests still pass
- [ ] Changes are documented with comments

## Validation Evidence Requirements

Include in your PR description:

```markdown
## EditorConfig Validation Log

- [x] `dotnet format` executed
- [x] All files formatted correctly
- [x] Build succeeds with no new warnings

## CI Evidence

Link to CI run: [#123](link)
```

## Common Mistakes to AVOID

```editorconfig
# ❌ WRONG: Using NoWarn in Directory.Build.props
# (Analyzer severity should be in .editorconfig)

# ❌ WRONG: No explanation for suppression
dotnet_diagnostic.CA1234.severity = none

# ✅ CORRECT: With explanation
# CA1234: <Brief reason this is acceptable for this codebase>
dotnet_diagnostic.CA1234.severity = none
```

## Decision Trees

### When Suppressing a Warning

1. Is this a valid warning that should be fixed? → Fix it instead
2. Is this technical debt? → Add comment, suppress, track for future
3. Is this not applicable to this codebase? → Suppress with explanation
4. Uncertain? → Stop and ask

### When Enabling a Warning

1. Will this cause many violations? → Plan for cleanup first
2. Can violations be auto-fixed? → Use `dotnet format` to fix
3. Is this a breaking change for contributors? → Document in PR

## Related Instruction Files

- [csharp.instructions.md](csharp.instructions.md) - For C# code affected by rules
- [msbuild.instructions.md](msbuild.instructions.md) - For build configuration
- [generic.instructions.md](generic.instructions.md) - For multi-file changes
