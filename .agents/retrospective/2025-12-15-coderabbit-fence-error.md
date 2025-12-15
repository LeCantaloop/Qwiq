# CodeRabbit Fence Language Identifier Error

**Date**: 2025-12-15
**Category**: Documentation Clarity / Bot Review Quality
**Severity**: Medium
**Status**: Documented

## Issue Summary

CodeRabbit incorrectly suggested removing code fence closing markers and changing PowerShell language identifiers to `text` for ADR files in the `.agents/` directory.

## Original Review Comment

**Source**: PR #118, discussion_r2617758655  
**File**: `.agents/architecture/ADR-011-portable-symbols-snupkg.md:206-224`

CodeRabbit suggested:

> **Fix fenced code block language identifier per coding guidelines.**
>
> This ADR is in `.agents/` directory. Per coding guidelines, pseudo-code and workflow blocks must use `text` or `markdown` language identifiers, not `powershell`.

**Suggested change**:

````diff
-```powershell
+```text
 # Push nupkg files
 foreach ($file in $nupkgFiles) {
     dotnet nuget push $file.FullName --api-key "$env:NUGET_API_KEY" `
         --source https://api.nuget.org/v3/index.json --skip-duplicate
 }
-```
+```
````

## Why This Is Wrong

1. **Closing fences MUST exist**: Markdown code blocks require both opening and closing fence markers (` ``` `). Removing the closing fence breaks markdown syntax.

2. **PowerShell code should use `powershell` identifier**: The code block contains actual PowerShell script code, not pseudo-code. It should use the `powershell` language identifier for proper syntax highlighting and semantic accuracy.

3. **No special `.agents/` rules exist**: There is NO documentation stating that files in `.agents/` directory should use different language identifier rules than other markdown files.

## Correct Documentation

Per `.github/instructions/markdown.instructions.md`:

````markdown
### Code Blocks

Always include language identifiers:

```powershell
dotnet build Qwiq.sln
```
````

Per `.agents/AGENT-INSTRUCTIONS.md`:

| Content Type                      | Language Identifier    |
| --------------------------------- | ---------------------- |
| PowerShell/shell commands         | `powershell` or `bash` |
| Plain text, pseudo-code, diagrams | `text`                 |

## Root Cause

CodeRabbit appears to have misinterpreted the guidance about using `text` for pseudo-code and incorrectly applied it to actual PowerShell code examples.

## Resolution

1. **Maintainer feedback**: @rjmurillo correctly identified this as an error and requested documentation of the issue.

2. **Current state**: ADR-011 file correctly uses:

   - ` ```powershell` (opening fence with language identifier)
   - Closing ` ``` ` fence
   - Actual PowerShell code content

3. **No code changes needed**: The markdown is already correct.

## Prevention

### For Human Reviewers

When evaluating bot suggestions about markdown fences:

1. ✅ **Always verify closing fences exist** - Never remove closing fence markers
2. ✅ **Match language identifier to content type** - Use `powershell` for PowerShell code, `text` for pseudo-code
3. ✅ **Check documentation references** - Verify that cited "coding guidelines" actually exist and say what's claimed

### For Documentation

This retrospective serves as clarification that:

- ✅ Code fences must always have opening AND closing markers
- ✅ PowerShell code blocks use `powershell` language identifier regardless of directory
- ✅ The `text` identifier is for pseudo-code, diagrams, and non-executable content
- ✅ No special markdown rules exist for `.agents/` directory files

## Related Documentation

- `.github/instructions/markdown.instructions.md` - Markdown standards (lines 49-67)
- `.agents/AGENT-INSTRUCTIONS.md` - Language identifier table
- `.agents/skills/markdown-skills.md` - Skill-Markdown-001

## Example: When to Use Each Identifier

### ✅ Use `powershell` for actual PowerShell code

```powershell
# Real PowerShell script
foreach ($file in Get-ChildItem *.nupkg) {
    dotnet nuget push $file.FullName
}
```

### ✅ Use `text` for pseudo-code or workflows

```text
FOR each package file:
    IF file is .nupkg:
        PUSH to NuGet
    ELSE IF file is .snupkg:
        PUSH symbols separately
```

### ✅ Use `text` for workflow diagrams

```text
Developer → Commit → Pre-commit hook → Lint → CI
                          ↓
                     Auto-fix issues
```

## Lesson Learned

**Statement**: Always verify that code fence closing markers exist and language identifiers match content type, regardless of bot suggestions.

**Atomicity**: 98%

**Context**: When reviewing markdown documentation changes suggested by automated tools.

**Evidence**: CodeRabbit PR #118 review incorrectly suggested removing closing fences and changing PowerShell identifier to text.

## Action Items

- [x] Document the error in this retrospective
- [x] Confirm current documentation is correct
- [x] Provide examples of correct usage
- [ ] Consider adding this to PR review checklist for human reviewers
