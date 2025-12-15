# PII/OII Prevention Instructions

> **MANDATORY**: You MUST follow these instructions to prevent committing Personally Identifiable Information (PII) and Organizationally Identifiable Information (OII).

## Quick Reference

- **NEVER** commit absolute file paths with usernames or organization names
- **NEVER** commit email addresses (except in examples like `user@example.com`)
- **NEVER** commit real names in documentation (use placeholders)
- **ALWAYS** use repository-relative paths
- **ALWAYS** use generic examples for demonstrations

## Context Loading

When creating or modifying any file in the repository, you MUST:

1. Read this entire instruction file before making changes
2. Review your changes for any PII/OII before committing
3. Use the validation checklist below
4. Complete a final scan before using **report_progress**

## What Constitutes PII/OII

### Personally Identifiable Information (PII)

Information that can identify a specific individual:

- Real names (except in LICENSE, AUTHORS, or copyright notices)
- Email addresses (except generic examples)
- Phone numbers
- Physical addresses
- User IDs or account names
- Social media handles

### Organizationally Identifiable Information (OII)

Information that can identify an organization or reveal internal structure:

- Company/organization names (in non-public contexts)
- Internal project names or codenames
- Department names
- Internal URLs or network paths
- Proprietary tool names
- Internal process names

### Absolute Path Information

File system paths that reveal:

- Username: `C:\Users\johndoe\...` or `/home/johndoe/...`
- Organization structure: `D:\src\CompanyName\...`
- Machine names: `\\MACHINE-NAME\share\...`
- Local development structure beyond repository root

## Acceptable vs. Unacceptable Examples

### File Paths

```markdown
<!-- ❌ WRONG: Absolute path with username/org -->

From `D:\src\GitHub\rjmurillo\Qwiq\Directory.Build.props`:

<!-- ❌ WRONG: Absolute path with username -->

Located at `/home/johndoe/projects/Qwiq/src/file.cs`:

<!-- ❌ WRONG: Network path with organization -->

From `\\CORP-SERVER\Projects\Qwiq\config.xml`:

<!-- ✅ CORRECT: Repository-relative path -->

From `Directory.Build.props`:

<!-- ✅ CORRECT: Repo-relative path with context -->

Located at `src/Qwiq.Core/WorkItem.cs`:

<!-- ✅ CORRECT: Generic placeholder for examples -->

Clone to your preferred location (e.g., `~/projects/Qwiq`):
```

### Email Addresses

```markdown
<!-- ❌ WRONG: Real email address -->

Contact: johndoe@company.com

<!-- ❌ WRONG: Real contributor email in docs -->

Implemented by alice.smith@example.org

<!-- ✅ CORRECT: Generic example -->

Use format: `user@example.com`

<!-- ✅ CORRECT: Generic placeholder -->

Set email in config: `your-email@domain.com`

<!-- ✅ CORRECT: Official project contact (if public) -->

Report security issues to: security@project.org
```

### Names

```markdown
<!-- ❌ WRONG: Real names in documentation -->

Analysis by: John Doe
Reviewed by: Alice Smith

<!-- ✅ CORRECT: Role-based attribution -->

Analysis by: Analyst Agent
Reviewed by: QA Agent

<!-- ✅ CORRECT: Generic placeholders -->

Author: Your Name
Committer: Developer Name
```

### Organization References

```markdown
<!-- ❌ WRONG: Internal organization name -->

Deployed to ACME Corp infrastructure

<!-- ❌ WRONG: Internal project codename -->

Part of Project Phoenix initiative

<!-- ✅ CORRECT: Generic reference -->

Deployed to production infrastructure

<!-- ✅ CORRECT: Public project name -->

Part of the Qwiq library ecosystem
```

## Special Cases: When PII/OII is Acceptable

### 1. LICENSE and COPYRIGHT Files

Acceptable to include:

- Copyright holder names (required by license)
- Author names (required by license)
- Official project maintainer contact

### 2. AUTHORS or CONTRIBUTORS Files

Acceptable to include:

- Real names of contributors who explicitly consent
- GitHub usernames (public information)
- Email addresses if contributors provided them publicly

### 3. Git Commit Metadata

Acceptable to include:

- Author names in commit metadata (Git feature)
- Email addresses in commit metadata (Git feature)
- Co-authored-by trailers (Git feature)

**Note**: These are Git features, not documentation. They don't violate this policy.

### 4. Code Comments with Attribution

```csharp
// ❌ WRONG: Real name in code comment
// Bug fix by John Doe <john.doe@company.com>

// ✅ CORRECT: Git blame provides attribution
// Bug fix for null reference exception

// ✅ CORRECT: Link to PR/issue for attribution
// Fix for GitHub issue #123
```

## Agent Instructions

### For Documentation Agents (explainer, analyst, qa, etc.)

When generating documentation:

1. **File References**: Use repository-relative paths only

   - ✅ `.agents/analysis/report.md`
   - ❌ `D:\src\GitHub\user\Qwiq\.agents\analysis\report.md`

2. **Attribution**: Use role-based attribution

   - ✅ "Analyst Agent", "QA Agent", "Security Agent"
   - ❌ "John Doe", `alice@company.com`

3. **Output Locations**: Omit absolute path footers

   - ❌ `## Output Location: D:\src\GitHub\...\file.md`
   - ✅ Just omit this section entirely

4. **Examples**: Use generic placeholders
   - ✅ `user@example.com`, `~/projects/Qwiq`
   - ❌ Real emails, real paths

### For Code Review Agents (critic, code_review, etc.)

When reviewing code or documentation:

1. **Flag as HIGH SEVERITY**: Any PII/OII in commits
2. **Specific items to flag**:

   - Absolute paths with usernames: `C:\Users\name\...`
   - Real email addresses: `name@company.com`
   - Organization names in non-public contexts
   - Real names in documentation (except LICENSE/AUTHORS)

3. **Recommended actions**:
   - Request immediate removal
   - Suggest repository-relative alternatives
   - Link to this instruction file

### For Implementation Agents (implementer, etc.)

When writing code:

1. **Test Data**: Use generic examples

   ```csharp
   // ✅ CORRECT
   var email = "user@example.com";
   var path = Path.Combine("projects", "Qwiq");

   // ❌ WRONG
   var email = "john.doe@company.com";
   var path = @"D:\src\CompanyName\Qwiq";
   ```

2. **Comments**: Avoid personal attribution

   ```csharp
   // ✅ CORRECT
   // Fix for null reference exception in WorkItem constructor

   // ❌ WRONG
   // Fixed by John Doe on 2025-01-15
   ```

3. **Configuration Examples**: Use placeholders

   **Correct example:**

   ```json
   {
     "author": "Your Name",
     "email": "your-email@example.com"
   }
   ```

   **Incorrect example:**

   ```json
   {
     "author": "John Doe",
     "email": "john.doe@company.com"
   }
   ```

## Validation Checklist

Before committing ANY file, verify:

- [ ] No absolute paths with usernames (e.g., `/home/user/...`, `C:\Users\name\...`)
- [ ] No absolute paths with organization names (e.g., `D:\src\CompanyName\...`)
- [ ] No real email addresses (except in LICENSE, AUTHORS, or public contacts)
- [ ] No real names in documentation (except in LICENSE, AUTHORS, or with consent)
- [ ] No organization-specific internal names or URLs
- [ ] All file paths are repository-relative
- [ ] All examples use generic placeholders

## Pre-Commit Scan Commands

Run these before using **report_progress**:

```bash
# Scan for Windows paths with potential usernames
git diff --cached | grep -i "C:\\\\Users\\\\" || echo "✅ No Windows user paths"
git diff --cached | grep -i "D:\\\\src\\\\" || echo "✅ No D:\src paths"

# Scan for Unix paths with home directories
git diff --cached | grep "/home/[^/]*/" | grep -v "/home/runner/" || echo "✅ No /home/user paths"

# Scan for email addresses (excluding known safe domains)
git diff --cached | grep -E "[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}" | grep -v "@example\." | grep -v "@anthropic\." | grep -v "@github\." | grep -v "@Live\.com" || echo "✅ No suspicious emails"

# Scan for GitHub usernames in paths
git diff --cached | grep -E "\\\\(GitHub|gitlab)\\\\[a-z]+" || echo "✅ No username in GitHub paths"
```

## Common Patterns to Watch For

### Documentation Generation

When agents generate analysis, critique, or QA reports:

```markdown
<!-- ❌ WRONG Pattern -->

**Analyst**: John Doe  
**Date**: 2025-12-14  
**Output**: D:\src\GitHub\rjmurillo\Qwiq\.agents\analysis\report.md

<!-- ✅ CORRECT Pattern -->

**Date**: 2025-12-14  
**Analyst**: Claude Code (Analyst Agent)  
**Status**: Complete
```

### File Reference Tables

```markdown
<!-- ❌ WRONG Pattern -->

| File   | Absolute Path                       |
| ------ | ----------------------------------- |
| Config | D:\src\GitHub\user\Qwiq\config.json |

<!-- ✅ CORRECT Pattern -->

| File   | Repository Path |
| ------ | --------------- |
| Config | `config.json`   |
```

### Command Examples

```bash
# ❌ WRONG Pattern
cd D:\src\GitHub\rjmurillo\Qwiq
dotnet build

# ✅ CORRECT Pattern
cd /path/to/Qwiq
dotnet build

# ✅ BETTER Pattern (relative)
# From repository root:
dotnet build
```

## Remediation Process

If PII/OII is discovered in committed files:

1. **Identify**: Scan repository for all instances
2. **Remove**: Edit files to use repository-relative paths or generic examples
3. **Verify**: Run validation commands above
4. **Commit**: Use conventional commit message

   ```text
   fix(docs): remove PII/OII from documentation files

   - Replace absolute paths with repository-relative paths
   - Replace real names with role-based attribution
   - Remove email addresses from examples
   ```

5. **Update Instructions**: If pattern was missed, update this file

## Related Instruction Files

- [markdown.instructions.md](markdown.instructions.md) - For documentation files
- [generic.instructions.md](generic.instructions.md) - General file editing rules
- [csharp.instructions.md](csharp.instructions.md) - For code examples in C# files

## Exceptions and Edge Cases

### Exception: CI/CD Runner Paths

Acceptable in CI logs or workflows:

- `/home/runner/...` (GitHub Actions)
- `/tmp/...` (temporary files)
- Build artifact paths

These are ephemeral and don't leak personal information.

### Exception: Public Repository URLs

Acceptable to reference:

- `https://github.com/rjmurillo/Qwiq` (public repository)
- Public issue/PR URLs
- Public documentation URLs

These are already public information.

### Exception: Test Data

Acceptable in test fixtures:

```csharp
// ✅ CORRECT: Clearly fictional test data
var testEmail = "testuser@example.com";
var testPath = @"C:\TestData\Sample.txt";
```

**Rule**: Test data must be obviously fictional and not match real systems.

## Final Reminder

**When in doubt, use generic placeholders and repository-relative paths.**

If you're unsure whether something constitutes PII/OII, err on the side of caution and:

1. Use a generic placeholder
2. Use a repository-relative path
3. Ask for clarification before committing
