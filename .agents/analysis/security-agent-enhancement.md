# Security Agent Enhancement: Comprehensive Code Audit & Risk Assessment

**Date**: 2025-12-13
**Status**: Upstream issue candidate for `rjmurillo/vs-code-agents`
**Priority**: HIGH - Directly prevents vulnerabilities like this session's shell injection

---

## Current vs. Proposed Security Agent Role

### Current (Too Narrow)

```text
security agent = Threat modeling + Vulnerability review + Code security review
```

**Limitations**:

- Reactive (only reviews if explicitly asked)
- Narrow scope (doesn't cover all security risks)
- Doesn't catch infrastructure security issues proactively
- Misses secret exposure, boundary violations, modularity issues

### Proposed (Comprehensive)

```text
security agent = Comprehensive code audit + risk assessment

Responsibilities:
  1. Threat modeling
  2. Vulnerability scanning (static + dynamic concepts)
  3. Secret detection (environment leaks, hardcoded secrets)
  4. Code quality audits (security perspective)
  5. Architecture review (security boundaries, modularity)
  6. File complexity analysis
  7. External coupling audit
  8. Best practices enforcement
```

---

## Proposed Security Agent Capabilities

### 1. Static Analysis & Vulnerability Scanning

**Capability**: Scan code for common vulnerability patterns

**What It Does**:

- CWE detection (CWE-78 shell injection, CWE-79 XSS, CWE-89 SQL injection, etc.)
- OWASP Top 10 scanning
- Vulnerable dependency detection
- Code anti-pattern detection
- Hardcoded credentials detection

**Example**:

```text
security agent receives: .githooks/pre-commit

Analysis:
  ✓ CWE-78: Shell injection via unquoted variable expansion
    → Identified pattern: `$MD_FILE_LIST` used in command
    → Risk: Filename with special characters can execute commands
    → Severity: CRITICAL
    → Mitigation: Use bash arrays with quoted expansion

  ✓ CWE-78: Command injection in C# formatting step
    → Identified pattern: `$CS_INCLUDE_ARGS` unquoted
    → Similar to markdown linting vulnerability

  ✓ Code style: No input validation on filenames
    → Risk: Assumption that filenames are safe
    → Recommendation: Validate/sanitize or quote all external inputs
```

**This Session**: Would have caught shell injection BEFORE implementation

### 2. Secret Detection & Environment Leak Scanning

**Capability**: Detect exposed secrets and environment coupling

**What It Does**:

- Scan for hardcoded API keys, tokens, passwords
- Detect environment variable leaks
- Find .env file exposure patterns
- Identify configuration in code vs. environment separation
- Credential pattern matching (AWS_ACCESS_KEY, JWT tokens, etc.)

**Example**:

```text
security agent receives: src/Services/ApiClient.cs

Analysis:
  ✗ CRITICAL: Hardcoded API key detected
    → Line 42: private const string API_KEY = "sk-1234567890"
    → Mitigation: Use environment variable or secure vault

  ⚠ HIGH: Direct environment.getenv("DB_PASSWORD")
    → Risk: Password visible in stack traces
    → Recommendation: Use secret management library

  ✓ GOOD: Configuration in appsettings.json with ${} syntax
    → Detected: Proper environment injection pattern
```

### 3. Architecture & Boundary Security Audit

**Capability**: Review architecture from security perspective

**What It Does**:

- Privilege boundary analysis (what runs with which privileges)
- Attack surface mapping
- Trust boundary identification
- Sensitive data flow analysis
- Dependency security review

**Example**:

```text
security agent receives: .github/workflows/main.yml + .githooks/pre-commit

Analysis:
  ⚠ HIGH: Pre-commit hook runs with full developer privileges
    → Can: Read all files, execute commands, access network
    → Risks: Malware injection, credential theft, data exfiltration
    → Boundary: Developer machine ↔ Untrusted filenames
    → Recommendation: Input validation, read-only filesystem hooks

  ✓ GOOD: CI workflow uses GitHub secrets (not environment variables)
    → Secret management: ✓ Isolated
    → Access control: ✓ Limited to repo maintainers
    → Audit logging: ✓ GitHub Actions logs all secret usage

  ⚠ MEDIUM: Pre-commit runs before commit → can modify files
    → Risk: Malicious modifications after user review
    → Recommendation: Log all modifications, require validation
```

### 4. Code Quality Audit (Security Perspective)

**Capability**: Identify code organization issues with security implications

**What It Does**:

- Flag files > 500 lines (testing burden, security review burden)
- Identify overly complex functions (> 20 lines with branches)
- Detect tight coupling (imports, dependencies, environment coupling)
- Module boundary violations
- Separation of concerns issues

**Example**:

```text
security agent receives: src/Services/AuthenticationService.cs (843 lines)

Analysis:
  ✗ FILE TOO LARGE: 843 lines
    → Risk: Hard to review for security, easy to miss issues
    → Contains: Auth logic, caching, DB access, external APIs
    → Recommendation: Split into:
       - AuthenticationService (interface)
       - CredentialValidator
       - TokenManager
       - SessionCache (handle separately)

  ⚠ COUPLING DETECTED: Tight coupling to environment variables
    → Config read: 15 locations in file
    → Risk: Config validation scattered, inconsistent
    → Recommendation: Create ConfigValidator class

  ✓ GOOD: Null validation on auth parameters
    → Pattern: Throws ArgumentNullException
    → Consistent with codebase style
```

### 5. Best Practices Enforcement

**Capability**: Enforce security best practices

**What It Does**:

- Input validation enforcement
- Error handling adequacy
- Logging of sensitive operations
- Cryptography usage correctness
- Testing coverage for security-critical code

**Example**:

```text
security agent receives: src/Controllers/UserController.cs

Analysis:
  ✗ MISSING INPUT VALIDATION: UpdateUser endpoint
    → Line 45: `var user = _service.Update(id, request);`
    → No validation of: request.Email, request.Phone, request.Permissions
    → Risk: Injection attacks, privilege escalation
    → Recommendation: Add ModelState.IsValid check, sanitize inputs

  ✗ MISSING LOGGING: Password change operation
    → Line 78: Password reset happens silently
    → Risk: No audit trail if account compromised
    → Recommendation: Log password change with timestamp, IP, user

  ✓ GOOD: Null check on authentication header
    → Pattern: if (string.IsNullOrEmpty(token)) throw
    → Prevents null reference exceptions

  ⚠ MEDIUM: Using MD5 for hashing
    → Line 120: `var hash = MD5.Create().ComputeHash(...)`
    → Risk: MD5 is cryptographically broken
    → Recommendation: Use bcrypt, PBKDF2, or Argon2
```

---

## Security Agent Invocation Rules

### MANDATORY Invocation (Blocking Entry Point)

**All of these REQUIRE security agent review before implementation**:

1. **Authentication/Authorization Changes**

   - Login flows, password reset, permission systems
   - Token management, session handling
   - Multi-factor authentication, OAuth integration

2. **Infrastructure Code**

   - Pre-commit hooks, build scripts
   - CI/CD workflows, deployment scripts
   - Docker files, cloud configuration

3. **Cryptography/Secrets**

   - Key generation, key storage
   - Secret management, encryption/decryption
   - Hashing, salting operations

4. **External API Integration**

   - Third-party service calls, API clients
   - Webhook handlers, callback endpoints
   - File upload/download operations

5. **Database/Data Operations**
   - Query construction (SQL injection risk)
   - Stored procedures, ORM configuration
   - Data access patterns, data validation

### RECOMMENDED Invocation (High-Value Entry Point)

**Beneficial for security agent to review**:

1. **New features** with data handling
2. **Refactoring** that affects security-critical code
3. **Code review** for security perspective
4. **Architecture changes** with security implications
5. **Third-party dependency** updates
6. **Configuration changes** affecting security posture

### AUTO-DETECT Invocation (Triggered by File Patterns)

**Security agent should be automatically suggested for**:

```text
File patterns that trigger security review:
  ├─ `.github/workflows/*` → infrastructure code
  ├─ `.githooks/*` → pre-commit/post-commit/etc
  ├─ `build/scripts/*` → build scripts
  ├─ `src/**/Controllers/*` → API endpoints (input validation)
  ├─ `src/**/Auth/*` → Authentication code
  ├─ `src/**/Security/*` → Security utilities
  ├─ `src/**/*Service*.cs` → External integrations
  ├─ `.editorconfig` → Analyzer configuration
  ├─ `Dockerfile`, `docker-compose.yml` → Container security
  ├─ `appsettings*.json` → Configuration (secrets leak)
  └─ Any file touching: credentials, tokens, keys, secrets
```

---

## Enhanced Capabilities Matrix for Security Agent

| Dimension               | Capability                                | Input                     | Output                                   |
| ----------------------- | ----------------------------------------- | ------------------------- | ---------------------------------------- |
| **Threat Modeling**     | Identify attack vectors, trust boundaries | Architecture, code flow   | Threat assessment, risk matrix           |
| **Static Analysis**     | Scan for CWE patterns, vulnerabilities    | Code, configuration       | Vulnerability list, severity, location   |
| **Secret Detection**    | Find hardcoded credentials, env leaks     | All source files, configs | Secret findings, exposure risk           |
| **Architecture Review** | Privilege analysis, boundary validation   | System design, deployment | Architecture assessment, recommendations |
| **Code Quality**        | File size, complexity, coupling analysis  | Source code metrics       | Quality report, refactoring suggestions  |
| **Best Practices**      | Input validation, error handling, logging | Code review               | Best practices violations, fixes         |
| **Dependency Audit**    | Vulnerable dependency detection           | Package manifests         | Dependency vulnerabilities, updates      |
| **Compliance Check**    | GDPR, SOC2, security standards            | Code, data handling       | Compliance gaps, remediation steps       |

---

## Example: This Session

### Without Enhanced Security Agent

```text
User: "Create workflow for markdown linting"
→ Skipped security agent
→ Implemented directly
→ Committed without review
→ Shell injection discovered in PR review
→ Reactive fix required
```

### With Enhanced Security Agent

```text
User: "Create workflow for markdown linting"
→ Infrastructure file detected (.github/workflows/lint.yml)
→ Security agent AUTO-INVOKED
→ Analysis:
   ✓ Pre-commit hook examined
   ✓ Unquoted variables detected
   ✓ CWE-78 shell injection identified
   ✓ Recommendations provided: use arrays + quoting
→ Pre-commit hook fixed BEFORE implementation
→ Approved for commit
→ Zero vulnerabilities in PR
```

---

## Upstream Enhancements Needed

### Issue 1: Enhance Security Agent Capabilities

**Title**: `enhancement: Expand security agent to perform comprehensive code audits`

**Description**:

```markdown
## Current Limitations

Security agent is limited to threat modeling and code review. This misses:

- Secret exposure (hardcoded credentials, env leaks)
- Code complexity from security perspective (files > 500 LOC)
- Architecture security (privilege boundaries, coupling)
- Best practices (input validation, error handling, logging)
- Vulnerable dependencies

## Proposed Enhancement

Expand security agent to perform:

1. **Static Analysis**: CWE scanning, OWASP Top 10, pattern detection
2. **Secret Detection**: Hardcoded credentials, environment variable leaks
3. **Code Quality Audit**: File size, complexity, tight coupling
4. **Architecture Review**: Privilege analysis, trust boundaries
5. **Best Practices**: Input validation, error handling, cryptography

## Benefits

- Catch security issues earlier (before implementation)
- Comprehensive security perspective (not just threats)
- Identify infrastructure security risks (hooks, workflows, scripts)
- Detect secret exposure automatically
- Recommend structural improvements

## Reference

See: https://github.com/rjmurillo/Qwiq/tree/develop/.agents/analysis/
```

### Issue 2: Auto-Detect Security-Critical Files

**Title**: `enhancement: Auto-trigger security agent for infrastructure & auth code`

**Description**:

```markdown
## Problem

Security agent not invoked unless explicitly requested. This session discovered
shell injection because security review was not automatic for infrastructure changes.

## Proposed Solution

Create file pattern matcher that auto-suggests security agent for:

- `.github/workflows/*` - CI/CD pipelines
- `.githooks/*` - Pre-commit/post-commit hooks
- `src/**/Auth/*` - Authentication code
- `src/**/Controllers/*` - API endpoints (input validation)
- `build/scripts/*` - Build scripts
- Any file modified that contains: credentials, secrets, tokens, keys

## Implementation

1. Pre-commit hook: Detect infrastructure files, warn "Security review recommended"
2. PR template: Auto-check "Security agent review completed" for security-critical files
3. CI gate: Flag infrastructure changes without security review comment (Phase 2)

## Benefit

Security-critical code automatically routed to security agent.
```

### Issue 3: Document Security Agent as Code Audit Tool

**Title**: `documentation: Reposition security agent as comprehensive code audit tool`

**Description**:

```markdown
## Current Positioning

Security agent presented as "threat modeling" specialist.

## Proposed Repositioning

Security agent is comprehensive code audit tool:

- CWE/OWASP scanning (static analysis)
- Secret exposure detection
- Code quality from security perspective
- Architecture security analysis
- Best practices enforcement

## Update Documentation

1. Expand AGENT-SYSTEM.md security agent description
2. Create SECURITY-AGENT-GUIDE.md with:
   - What to ask security agent
   - What to expect in response
   - Code patterns security agent looks for
   - When to use vs. other agents
3. Add examples of security agent usage:
   - "Review this hook for injection vulnerabilities"
   - "Audit this file for secrets exposure"
   - "Check architecture for privilege boundary issues"
```

---

## Implementation Checklist (Qwiq Repository)

### Phase 1: Detection & Reporting

- [ ] Create security audit checklist (`.agents/SECURITY-AUDIT-CHECKLIST.md`)
- [ ] Document what security agent checks for
- [ ] Add file pattern detection to pre-commit hook
- [ ] Warn when infrastructure files modified without security review

### Phase 2: Enforcement

- [ ] PR template includes security review checkbox for infrastructure
- [ ] CI gate flags missing security review for infrastructure changes
- [ ] Metrics tracking: infrastructure changes with/without security review

### Phase 3: Automation

- [ ] Auto-invoke security agent for infrastructure file changes
- [ ] Auto-scan code for common patterns (secrets, etc.)
- [ ] Generate security audit reports

---

## Success Metrics

| Metric                                          | Current | Target (3 months)               |
| ----------------------------------------------- | ------- | ------------------------------- |
| Infrastructure changes reviewed by security     | 0%      | 100%                            |
| Shell injection vulnerabilities caught pre-impl | 0       | 100%                            |
| Hardcoded secrets detected automatically        | 0%      | 100%                            |
| Files > 500 LOC flagged for review              | Unknown | 100%                            |
| Security agent invocation rate                  | ~5%     | 80%+ for security-critical code |

---

## Conclusion

The shell injection vulnerability in this session would have been prevented if the security agent had:

1. **Broader role** - Infrastructure code auditing, not just threats
2. **Auto-triggers** - Infrastructure files automatically routed to security
3. **Comprehensive scans** - Static analysis for CWE patterns like shell injection
4. **Clear invocation rules** - "Always use security agent for infrastructure code"

Expanding security agent from narrow (threat modeling) to comprehensive (code audit tool) shifts security from reactive (catch in PR) to proactive (catch before implementation).
