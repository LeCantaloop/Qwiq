---
applyTo: "**"
---

# Generic File Instructions

> **MANDATORY**: You MUST follow these instructions when editing any file type not covered by a specific instruction file.

## Quick Reference

- Check [README.md](README.md) for file-type specific instructions first
- When in doubt, ask before proceeding
- Document all changes in PR description
- Complete validation checklist before submitting

## Context Loading

When working on ANY file, you MUST:

1. Check if a file-type specific instruction file exists
2. Read [copilot-instructions.md](../copilot-instructions.md) for repository guidelines
3. Follow established patterns in similar files
4. Complete the Validation Checklist before submitting

## Multi-File Change Flowchart

For changes spanning multiple file types:

1. **Identify all affected file types**

   - List every file that will be modified
   - Find corresponding instruction files for each type

2. **Read all relevant instruction files**

   - Note validation requirements for each type
   - Identify dependencies between files

3. **Plan the change order**

   - Code changes before documentation
   - Build files before source files
   - Tests after implementation

4. **Execute changes systematically**

   - Make one logical change at a time
   - Validate after each significant change
   - Commit incrementally

5. **Validate comprehensively**

   - Build: `dotnet build Qwiq.sln /m:1 /nodeReuse:false -c Release`
   - Test: `dotnet test --filter "..."`
   - Lint: `dotnet format` (C# analyzer fixes) and `dotnet pprettier --write .` (formatting)

6. **Prepare PR with evidence**
   - Document changes for each file type
   - Include validation output
   - Link to CI runs

## Validation Checklist

Before submitting changes, verify:

- [ ] All affected files identified
- [ ] Relevant instruction files reviewed
- [ ] Changes follow established patterns
- [ ] Build succeeds
- [ ] Tests pass
- [ ] No new warnings introduced
- [ ] PR description complete

## Validation Evidence Requirements

Include in your PR description:

```markdown
## Validation Log

### Build
```

dotnet build output showing success

```

### Tests
```

Test results showing all pass

```

### Files Changed
- `path/to/file1.cs` - Description of change
- `path/to/file2.md` - Description of change

## CI Evidence
Link to CI run: [#123](link)
```

## Escalation Path

If you are blocked or uncertain:

1. **Stop work immediately**
2. Document what you were trying to accomplish
3. Explain what specific aspect is unclear
4. Request guidance with specific questions
5. Do not proceed until you have clear understanding

## Decision Trees

### When to Request Human Review

- Is this a new file type? → Check for instruction file, ask if none
- Is this a breaking change? → Document thoroughly, request review
- Are you uncertain about requirements? → Stop and ask

### When Making Cross-Cutting Changes

1. Does this affect build? → See [msbuild.instructions.md](msbuild.instructions.md)
2. Does this affect code style? → See [editorconfig.instructions.md](editorconfig.instructions.md)
3. Does this affect CI/CD? → See [yaml.instructions.md](yaml.instructions.md)
4. Does this affect documentation? → See [markdown.instructions.md](markdown.instructions.md)

## Common Mistakes to AVOID

- Skipping validation steps
- Not reading file-type specific instructions
- Making changes without understanding impact
- Submitting without validation evidence
- Ignoring existing patterns in the codebase

## Success Criteria

Your changes are successful when:

- All builds pass without warnings
- All tests pass
- No linting errors (run `dotnet format` and `dotnet pprettier --write .` to auto-fix)
- PR description is complete and accurate
- All checklist items completed
- Established patterns are followed

## Related Instruction Files

- [README.md](README.md) - Index of all instruction files
- [csharp.instructions.md](csharp.instructions.md) - For C# files
- [project.instructions.md](project.instructions.md) - For project files
- [msbuild.instructions.md](msbuild.instructions.md) - For MSBuild files
- [editorconfig.instructions.md](editorconfig.instructions.md) - For .editorconfig
- [markdown.instructions.md](markdown.instructions.md) - For documentation
- [yaml.instructions.md](yaml.instructions.md) - For CI/CD workflows
- [shell.instructions.md](shell.instructions.md) - For PowerShell scripts
- [json.instructions.md](json.instructions.md) - For JSON configuration
- [codacy.instructions.md](codacy.instructions.md) - For Codacy integration
