# Product Requirements Documents: 9 Upstream Issues for rjmurillo/vs-code-agents

**Date**: 2025-12-13
**Total Issues**: 9
**Priority Breakdown**: 3 CRITICAL, 3 HIGH, 3 MEDIUM
**Evidence Base**: CWE-78 shell injection vulnerability not caught until PR review (should have been caught pre-implementation)

---

## PRD #1: Orchestrator Entry Criteria (CRITICAL)

**Issue URL**: <https://github.com/rjmurillo/vs-code-agents/issues/16>
**Title**: `enhancement: Define mandatory entry criteria for orchestrator agent`
**Priority**: CRITICAL
**Complexity**: Medium
**Effort**: 2-3 days

### Problem Statement

The orchestrator agent is powerful but underutilized. Developers don't know when (or if) they should invoke the orchestrator, resulting in complex multi-specialty tasks being attempted without proper coordination.

**Evidence**: This session's shell injection vulnerability occurred in a task that required multi-specialty coordination (devops + security + architecture) but was implemented directly without orchestrator assessment.

### Goals

1. Define clear, objective criteria for when orchestrator MUST be invoked
2. Reduce decision ambiguity ("should I use orchestrator?")
3. Enable automated detection (CI/hooks can trigger orchestrator when criteria met)
4. Prevent complex tasks from being attempted without orchestrator coordination

### Success Criteria

- ✅ Orchestrator invocation rate increases from ~5% to 80%+ for multi-specialty tasks
- ✅ Developer can answer "Should I invoke orchestrator?" in under 30 seconds
- ✅ Multi-domain tasks identified and routed to orchestrator automatically
- ✅ Zero critical-severity issues found during PR review on tasks that met orchestrator criteria

### Requirements

#### Functional Requirements

1. **FR-1: Document Entry Criteria** (Hard Rules)
   - Define objective criteria where orchestrator MUST be invoked (non-negotiable)
   - Criteria Examples:
     - Changes affecting multiple domains (CI + security + architecture)
     - Changes requiring design validation before implementation
     - Changes with security implications
     - Changes to infrastructure code (hooks, workflows, build scripts)
   - Output: `.agents/orchestrator/ENTRY-CRITERIA.md`

2. **FR-2: Create Decision Tree** (Visual Navigation)
   - Single-page flowchart for developers
   - Max 3 questions to determine if orchestrator needed
   - Decision outcomes: "Invoke Orchestrator", "Use Specific Agent", "Proceed Directly"
   - Output: Visual diagram + markdown

3. **FR-3: Integration Points** (Automation)
   - Specify how to integrate entry criteria into:
     - Pre-commit hooks (warn if infrastructure change without orchestrator)
     - CI workflows (flag multi-domain changes)
     - PR templates (checkbox: "Was orchestrator invoked?")

4. **FR-4: Examples** (Clarity)
   - Document 5+ example scenarios showing when orchestrator applies
   - Real examples from codebase/sessions

#### Non-Functional Requirements

1. **NFR-1: Clarity**: Entry criteria understandable by any developer, no jargon
2. **NFR-2: Brevity**: Decision tree fits on one page
3. **NFR-3: Actionability**: Criteria must be implementable in automated systems

### Acceptance Criteria

- [ ] Entry criteria document created and comprehensive
- [ ] Decision tree visual created (flowchart or similar)
- [ ] At least 5 example scenarios documented
- [ ] Criteria evaluated against this session's vulnerability (would have triggered orchestrator)
- [ ] Integration strategy defined (not implemented, just specified)
- [ ] PR review by vs-code-agents maintainers

### Dependencies

- None (standalone documentation)

### Metrics/KPIs

| Metric | Current | Target | Measurement |
|--------|---------|--------|-------------|
| Orchestrator invocation rate | 5% | 80%+ | Parse commit messages for orchestrator reference |
| Decision time (developer asks "should I?") | Unknown | <30 sec | Survey feedback |
| Multi-domain tasks without orchestrator | 100% of complex tasks | 0% | Audit multi-domain PRs |

### Timeline

- **Week 1**: Research, document criteria, create decision tree
- **Week 2**: Create examples, integration spec
- **Week 3**: PR review, refinement

---

## PRD #2: Security Agent Enhancement (CRITICAL)

**Issue URL**: <https://github.com/rjmurillo/vs-code-agents/issues/19>
**Title**: `enhancement: Expand security agent to perform comprehensive code audits`
**Priority**: CRITICAL
**Complexity**: High
**Effort**: 2-4 weeks

### Problem Statement

Security agent is currently narrow in scope (threat modeling + code review). This misses critical security risks:

- Shell injection vulnerabilities (CWE-78) in infrastructure code
- Hardcoded secrets and environment variable leaks
- Code complexity from security perspective (files > 500 LOC)
- Architectural security issues (privilege boundaries, coupling)
- Best practices enforcement

**Evidence**: This session's shell injection vulnerability was not caught because security agent wasn't invoked. If security agent had broader static analysis capabilities, vulnerability would have been detected.

### Goals

1. Expand security agent from narrow (threat modeling) to comprehensive (code audit tool)
2. Enable automatic vulnerability scanning (CWE patterns, OWASP Top 10)
3. Detect secret exposure automatically
4. Flag code organization issues with security implications
5. Enforce security best practices

### Success Criteria

- ✅ Security agent invoked in 90%+ of infrastructure code changes
- ✅ CWE vulnerabilities detected in static analysis (before implementation)
- ✅ Hardcoded secrets detected in all new code
- ✅ Files > 500 LOC flagged for review
- ✅ Shell injection vulnerabilities caught pre-implementation (not during PR)

### Requirements

#### Functional Requirements

1. **FR-1: Static Analysis for CWE Patterns**
   - Scan code for common vulnerability patterns:
     - CWE-78: Shell injection (unquoted variables, command substitution)
     - CWE-79: XSS vulnerabilities
     - CWE-89: SQL injection
     - CWE-200: Information exposure
     - CWE-287: Authentication bypass patterns
   - Output examples:

     ```text
     ✗ CWE-78: Shell injection via unquoted variable
       Line 42: npx cmd $UNQUOTED_VAR
       Mitigation: Use arrays with quoted expansion: "${ARRAY[@]}"

     ```

     ```text

2. **FR-2: Secret Detection**
   - Scan for hardcoded credentials:
     - API keys (AWS_ACCESS_KEY, sk-*, Bearer tokens)
     - Database passwords, connection strings
     - Private keys, certificates
     - Environment variable leaks
   - Output examples:

     ```
     ```text
     ✗ CRITICAL: Hardcoded API key detected
       File: src/Services/ApiClient.cs, Line 42
       Pattern: `private const string API_KEY = "sk-123..."`
       Risk: Exposed in source code
       Mitigation: Use environment variable or secrets vault

     ```

     ```text

3. **FR-3: Code Quality Audit (Security Perspective)**
   - Flag code organization issues:
     - Files > 500 lines (difficult to review, easy to miss vulnerabilities)
     - Functions > 20 lines with branches (complex, untestable)
     - Tight coupling to environment variables (scattered config)
     - Missing null validation on sensitive parameters
   - Output examples:

     ```
     ```text
     ✗ FILE TOO LARGE: 843 lines
       Risk: Hard to review, easy to miss security issues
       Recommendation: Split into smaller modules

     ```

     ```text

4. **FR-4: Architecture Review (Security Perspective)**
   - Analyze privilege boundaries:
     - What runs with what privileges?
     - What code runs with user input?
     - What code handles sensitive data?
   - Output examples:

     ```
     ```text
     ⚠ HIGH: Pre-commit hook runs with full developer privileges
       Risk: Can read all files, execute arbitrary commands
       Boundary: Developer machine ↔ Untrusted filenames
       Mitigation: Use arrays for filename args, validate input

     ```

     ```text

5. **FR-5: Best Practices Enforcement**
   - Verify security-critical code patterns:
     - Input validation on all endpoints
     - Error handling adequate (doesn't leak secrets)
     - Cryptography correct (bcrypt not MD5, proper key sizes)
     - Logging of sensitive operations (audit trail)
     - Testing coverage for security-critical code
   - Output examples:

     ```
     ```text
     ✗ MISSING INPUT VALIDATION: UpdateUser endpoint
       Line 45: `_service.Update(id, request)`
       Missing validation of: request.Email, request.Phone
       Risk: Injection attacks, privilege escalation

     ```

     ```text

#### Non-Functional Requirements

1. **NFR-1: Performance**: Analysis completes within 5 minutes for typical file
2. **NFR-2: Accuracy**: False positive rate < 10% (better to flag something safe than miss vulnerability)
3. **NFR-3: Clarity**: Findings presented with clear context and mitigation recommendations
4. **NFR-4: Integration**: Works with existing security agent invocation model

### Acceptance Criteria

- [ ] Static analysis for CWE patterns implemented
- [ ] Secret detection implemented and tested
- [ ] Code quality audit (security perspective) implemented
- [ ] Architecture review capability implemented
- [ ] Best practices enforcement implemented
- [ ] All 5 capabilities have documented examples
- [ ] Evaluated against this session's shell injection (would have caught it)
- [ ] Performance acceptable (<5 min typical analysis)

### Dependencies

- Requires FR-3 from PRD #3 (Agent Capabilities Matrix)
- Should coordinate with PRD #4 (Auto-Detection)

### Metrics/KPIs

| Metric | Current | Target | Measurement |
|--------|---------|--------|-------------|
| Vulnerabilities caught pre-implementation | 0% | 90%+ | Track where issues found (PR vs. pre-impl) |
| Hardcoded secrets detected | 0% | 100% of cases | Manual audit of commits |
| Files > 500 LOC flagged | 0% | 100% | Security agent report |
| Security agent invocation rate | 5% | 90%+ for infrastructure | Commit message analysis |

### Timeline

- **Week 1-2**: Design and implement CWE scanning
- **Week 2-3**: Implement secret detection
- **Week 3**: Implement code quality audit
- **Week 4**: Implement architecture review + best practices
- **Week 5**: Testing, documentation, examples

---

## PRD #3: Agent Capabilities Matrix (CRITICAL)

**Issue URL**: <https://github.com/rjmurillo/vs-code-agents/issues/17>
**Title**: `documentation: Publish comprehensive agent capabilities matrix with explicit limitations`
**Priority**: CRITICAL
**Complexity**: Medium
**Effort**: 1-2 weeks

### Problem Statement

The orchestrator (and developers) don't have comprehensive knowledge of what each agent can and cannot do. Current documentation is implicit and scattered, leading to:

- Incorrect agent routing (using wrong agent for task)
- Missed opportunities (not knowing agent can help)
- Inefficient coordination
- Ambiguity about agent limitations

**Evidence**: This session's orchestrator wasn't invoked partly because developers didn't have clear guidance on which agents to use for the task.

### Goals

1. Create single source of truth for agent capabilities
2. Make limitations explicit (not just capabilities)
3. Enable intelligent orchestrator routing decisions
4. Reduce ambiguity for developers

### Success Criteria

- ✅ All 15+ agents documented with explicit capabilities and limitations
- ✅ Orchestrator can make intelligent routing decisions based on matrix
- ✅ Developers can answer "Which agent should I use?" by consulting matrix
- ✅ Matrix reflects reality (tested against actual agent usage)

### Requirements

#### Functional Requirements

1. **FR-1: Comprehensive Matrix**
   - Document each agent with:
     - **Specialty**: Core domain of expertise
     - **Capabilities**: 5-10 specific tasks it excels at
     - **Limitations**: What it CANNOT do (critical!)
     - **Input Format**: What to ask/provide the agent
     - **Output Format**: What to expect from agent
     - **Best Paired With**: Which agents coordinate well with this one
     - **Invocation Rule**: When to use this agent
   - Example Entry (security agent):

     ```
     ```text
     | Property | Value |
     |----------|-------|
     | **Specialty** | Security review, vulnerability assessment |
     | **Capabilities** | <ul><li>Threat modeling</li><li>CWE/OWASP scanning</li><li>Secret detection</li><li>Code quality audit (security perspective)</li></ul> |
     | **Limitations** | <ul><li>Cannot implement fixes</li><li>Cannot test implementations</li><li>Cannot make architectural decisions alone</li><li>Requires expert security knowledge</li></ul> |
     | **Best Paired With** | qa (for security testing), csharp-expert (for fixes) |
     | **Invocation Rule** | MANDATORY for infrastructure changes, authentication, cryptography |

     ```

     ```text

2. **FR-2: Tier the Agents**
   - TIER 1 (Core): Agents used in 80%+ of workflows
   - TIER 2 (Common): Agents used in 20-80% of workflows
   - TIER 3 (Specialist): Agents used in <20% of workflows
   - Help developers quickly identify which agents are essential

3. **FR-3: Limitations Section**
   - Explicitly document what agents CANNOT do
   - This is as important as capabilities
   - Help developers avoid misuse

4. **FR-4: Real Examples**
   - For each agent: 2-3 real examples of when to use it
   - Include examples of when NOT to use it

#### Non-Functional Requirements

1. **NFR-1: Comprehensive**: All 15+ agents documented
2. **NFR-2: Current**: Matrix reflects actual agent capabilities in latest version
3. **NFR-3**: Accessible: Matrix in single document, not scattered across multiple files

### Acceptance Criteria

- [ ] Matrix created documenting all agents
- [ ] Each agent has: specialty, capabilities, limitations, input/output, pairings, invocation rules
- [ ] Agents tiered (TIER 1/2/3)
- [ ] Real examples for each agent
- [ ] Matrix reviewed against actual agent implementations (accurate)
- [ ] Evaluated against this session's task (would have routed to security agent)

### Dependencies

- Requires input from all agent implementations
- Depends on PRD #2 (Security Agent Enhancement) for security agent capabilities

### Metrics/KPIs

| Metric | Current | Target |
|--------|---------|--------|
| Agent documentation completeness | Unknown | 100% (all agents documented) |
| Developer satisfaction with "which agent?" clarity | Unknown | 4/5+ |

### Timeline

- **Week 1**: Interview agents, document capabilities/limitations
- **Week 2**: Create matrix, add examples
- **Week 3**: Review for accuracy, refine

---

## PRD #4: Security Agent Auto-Detection (HIGH)

**Issue URL**: <https://github.com/rjmurillo/vs-code-agents/issues/20>
**Title**: `enhancement: Auto-trigger security agent for infrastructure & auth code`
**Priority**: HIGH
**Complexity**: Medium
**Effort**: 1-2 weeks

### Problem Statement

Security agent is not invoked unless explicitly requested. Infrastructure code (hooks, workflows, scripts) and authentication code run with elevated privileges but security reviews are optional.

**Evidence**: This session's pre-commit hook vulnerability occurred because security agent wasn't automatically triggered for infrastructure code changes.

### Goals

1. Automatically invoke security agent for security-critical file patterns
2. Reduce reliance on developer memory ("did I invoke security agent?")
3. Catch infrastructure vulnerabilities before implementation

### Success Criteria

- ✅ Security agent triggered automatically for all TIER 1 files (infrastructure, auth)
- ✅ Zero infrastructure vulnerabilities discovered during PR review (caught pre-impl)
- ✅ Developer workflow unchanged (no added friction)

### Requirements

#### Functional Requirements

1. **FR-1: File Pattern Detection**
   - Define file patterns that auto-trigger security agent:
     - `.github/workflows/*` - CI/CD pipelines
     - `.githooks/*` - Pre-commit/post-commit hooks
     - `src/**/Auth/**` or `src/**/Authentication/**` - Authentication code
     - `src/**/Security/**` - Security utilities
     - `build/scripts/*` - Build scripts
     - Any file containing: credentials, secrets, keys, tokens
   - Output: List of triggering patterns

2. **FR-2: Integration Points** (Specify but don't implement)
   - Pre-commit hook: Detect infrastructure files, notify developer
   - PR template: Auto-check "Security agent invoked" for infrastructure files
   - CI gate (Phase 2): Block merge if infrastructure files lack security review
   - Specification document: `.agents/AUTO-DETECTION-INTEGRATION.md`

#### Non-Functional Requirements

1. **NFR-1: Accuracy**: No false positives (avoid triggering on non-critical files)
2. **NFR-2**: No friction: Auto-detection should not slow development

### Acceptance Criteria

- [ ] File patterns documented and justified
- [ ] Integration strategy documented
- [ ] Evaluated against this session's infrastructure changes (would have triggered)
- [ ] Validated by security team

### Dependencies

- Depends on PRD #3 (Agent Capabilities Matrix) - needs to know what security agent can do
- Related to PRD #2 (Security Agent Enhancement) - agent needs broader capabilities to justify auto-trigger

### Timeline

- **Week 1**: Define file patterns, document justification
- **Week 2**: Design integration strategy, create spec

---

## PRD #5: Threat Model Documentation (HIGH)

**Issue URL**: <https://github.com/rjmurillo/vs-code-agents/issues/18>
**Title**: `documentation: Create explicit threat models for infrastructure code categories`
**Priority**: HIGH
**Complexity**: Medium
**Effort**: 1-2 weeks

### Problem Statement

Infrastructure code (pre-commit hooks, CI workflows, build scripts) runs with elevated privileges but threat models are implicit, not explicit. Developers don't understand "why" security review matters for infrastructure code.

**Evidence**: This session's shell injection vulnerability wasn't recognized as security-critical because the threat model (hooks run with developer privilege) was implicit.

### Goals

1. Create explicit threat models for infrastructure code categories
2. Document privilege boundaries and attack surfaces
3. Help developers understand why security review is critical
4. Provide context for security agent recommendations

### Success Criteria

- ✅ Threat models created for all infrastructure code categories
- ✅ Developers understand privilege level and attack surface for each category
- ✅ Security recommendations justified by threat model
- ✅ Cross-referenced from code (e.g., hook header references threat model)

### Requirements

#### Functional Requirements

1. **FR-1: Pre-commit Hooks Threat Model**
   - Document:
     - Privilege level: Developer machine, full developer privileges
     - Access: Full filesystem, environment variables, network access
     - Can execute: Arbitrary commands (npx, dotnet, bash, etc.)
     - Attack vectors: CWE-78 (shell injection), environment variable injection
     - Example vulnerabilities: Unquoted filename expansion, command substitution
     - Mitigations: Use arrays, validate inputs, quote carefully
   - Reference: Include this session's shell injection as example

2. **FR-2: CI/CD Workflows Threat Model**
   - Document:
     - Privilege level: CI runner, limited permissions
     - Access: Source code, build artifacts, secrets (if configured)
     - Risks: Secret exposure in logs, artifact tampering, unauthorized deployments
     - Mitigations: Secrets management, audit logging, access control

3. **FR-3: Build Scripts Threat Model**
   - Document privilege, access, risks, mitigations

4. **FR-4: Configuration Files Threat Model (.editorconfig, appsettings, etc.)**
   - Document analyzer configuration, secrets exposure risks

#### Non-Functional Requirements

1. **NFR-1: Clarity**: Threat models understandable by any developer
2. **NFR-2: Actionability**: Recommendations based on threat model should be implementable

### Acceptance Criteria

- [ ] Threat models created for all infrastructure code categories
- [ ] Each model documents: privilege level, access, attack vectors, mitigations
- [ ] Real vulnerability from this session used as example
- [ ] Cross-referenced from code (e.g., hook includes link to threat model)
- [ ] Reviewed by security team

### Dependencies

- Depends on completion of this session's analysis
- Complements PRD #2 (Security Agent Enhancement)

### Timeline

- **Week 1**: Draft threat models for each category
- **Week 2**: Refine, add examples, create cross-references

---

## PRD #6: Agent System Governance (HIGH)

**Issue URL**: <https://github.com/rjmurillo/vs-code-agents/issues/21>
**Title**: `enhancement: Create governance framework for agent system (ADR template, steering, consolidation)`
**Priority**: HIGH
**Complexity**: High
**Effort**: 2-3 weeks

### Problem Statement

Agent system has grown to 15+ agents without clear governance:

- No steering committee reviewing new agents
- No process for consolidating overlapping agents
- No maximum agent count constraint
- Major architectural decisions (mandatory review gates) lack ADRs

**Evidence**: This session identified overlapping agents (architect vs. csharp-pod), complexity exceeding practical usage, and governance gaps.

### Goals

1. Create governance structure for agent system evolution
2. Define process for adding/removing/consolidating agents
3. Require ADRs for major agent-related architectural decisions
4. Establish architectural principles for agent design

### Success Criteria

- ✅ ADR process established and followed for major decisions
- ✅ Agent consolidation process defined
- ✅ Steering committee charter defined
- ✅ Architectural principles for agent design documented

### Requirements

#### Functional Requirements

1. **FR-1: ADR Template for Agent Decisions**
   - Create template for ADRs related to agent system
   - Sections:
     - Decision: What's being decided?
     - Context: Why is this decision needed?
     - Options: What alternatives were considered?
     - Decision: Which option was chosen and why?
     - Consequences: What are the trade-offs?
     - Acceptance criteria: How do we know this decision was good?
   - Example: ADR-011-Infrastructure-Review-Gates (required for this session's fix)

2. **FR-2: Steering Committee Charter**
   - Define:
     - Who reviews new agent proposals?
     - What criteria must new agents meet?
     - What architectural principles guide agent design?
     - How are overlapping agents consolidated?
     - What's the maximum agent count?

3. **FR-3: Agent Design Principles**
   - Non-overlapping responsibilities
   - Clear entry criteria (when to invoke)
   - Explicit limitations (what it cannot do)
   - Composable (works well with other agents)
   - Testable (can verify agent works correctly)

4. **FR-4: Consolidation Process**
   - Define how to identify overlapping agents
   - Merge criteria
   - Migration path for users of merged agents

#### Non-Functional Requirements

1. **NFR-1**: Lightweight (doesn't add bureaucratic overhead)
2. **NFR-2**: Effective (prevents uncontrolled growth)

### Acceptance Criteria

- [ ] ADR template created
- [ ] Steering committee charter defined
- [ ] Agent design principles documented
- [ ] Consolidation process defined
- [ ] Reviewed by maintainers

### Dependencies

- Depends on PRD #3 (Agent Capabilities Matrix) - need clear agent definitions

### Timeline

- **Week 1**: Define ADR template, steering committee charter
- **Week 2**: Define design principles, consolidation process
- **Week 3**: Create examples, refine

---

## PRD #7: Agent Invocation Metrics (MEDIUM)

**Issue URL**: <https://github.com/rjmurillo/vs-code-agents/issues/22>
**Title**: `enhancement: Add observability and metrics to agent system`
**Priority**: MEDIUM
**Complexity**: Medium
**Effort**: 1-2 weeks

### Problem Statement

Agent invocations are invisible. No way to:

- Detect when agents are skipped for security-critical tasks
- Measure shift-left effectiveness
- Track agent system adoption
- Identify improvement opportunities

**Evidence**: This session's vulnerability was only discovered during PR review (reactive) because there was no monitoring of agent invocations.

### Goals

1. Add observability to agent system
2. Define and track shift-left metrics
3. Enable data-driven improvements

### Success Criteria

- ✅ Agent invocation rate tracked and monitored
- ✅ Shift-left effectiveness measured (issues caught pre-impl vs. PR)
- ✅ Monthly metrics report generated
- ✅ Baseline and targets established

### Requirements

#### Functional Requirements

1. **FR-1: Define Metrics**
   - Invocation rate: % of complex tasks using orchestrator
   - Coverage: % of infrastructure changes with security review
   - Shift-left effectiveness: % of vulnerabilities caught pre-implementation
   - Agent usage distribution: Which agents are used most?
   - Turnaround time: How long agents take (P50, P95)

2. **FR-2: Measurement Infrastructure**
   - Agent review metadata in commits (e.g., `Reviewed-By: security-agent`)
   - CI job parsing agent review comments from merged PRs
   - Manual audit of issue discovery points (pre-impl, PR, post-merge)

3. **FR-3: Reporting**
   - Monthly dashboard generated
   - Trend analysis: metrics improving or declining?
   - Alerts: if shift-left effectiveness drops below target

#### Non-Functional Requirements

1. **NFR-1**: Minimal overhead (measurement shouldn't burden developers)
2. **NFR-2**: Accuracy (metrics must reflect reality)

### Acceptance Criteria

- [ ] Metrics defined and baseline established
- [ ] Measurement strategy documented
- [ ] CI job implemented to track metrics
- [ ] Dashboard template created
- [ ] First monthly report generated

### Dependencies

- Depends on PRD #3 (Capabilities Matrix) - need clear agent definitions to track

### Timeline

- **Week 1**: Define metrics, establish baseline
- **Week 2**: Implement measurement infrastructure, create dashboard

---

## PRD #8: Capabilities Discovery Protocol (MEDIUM)

**Issue URL**: <https://github.com/rjmurillo/vs-code-agents/issues/23>
**Title**: `enhancement: Implement "agent interview" protocol for capabilities discovery`
**Priority**: MEDIUM
**Complexity**: Low
**Effort**: 3-5 days

### Problem Statement

As agents are added or updated, their capabilities need to be formally documented. Current process is informal, leading to gaps in the capabilities matrix (PRD #3).

### Goals

1. Create repeatable process for agent capability discovery
2. Keep capabilities matrix current as agents evolve
3. Ensure new agents are properly documented before deployment

### Success Criteria

- ✅ Agent interview protocol documented and published
- ✅ All 15+ agents interviewed and capabilities verified
- ✅ Protocol followed for any new agents added

### Requirements

#### Functional Requirements

1. **FR-1: Agent Interview Protocol**
   - Questions to ask each agent:
     1. What is your core specialty?
     2. What specific tasks can you handle? (5-10 examples)
     3. What are your limitations? (What you CANNOT do)
     4. Which agents work well with you?
     5. What input format do you expect?
     6. What should I expect in your output?
     7. When should I use you? (Invocation rules)
     8. When should I NOT use you?
   - Output: Standardized response that feeds into capabilities matrix

#### Non-Functional Requirements

1. **NFR-1**: Simple and repeatable

### Acceptance Criteria

- [ ] Interview protocol documented
- [ ] All agents interviewed
- [ ] Responses incorporated into capabilities matrix
- [ ] Protocol to be followed for new agents

### Timeline

- **Week 1**: Create protocol, conduct interviews
- **Week 2**: Compile results into matrix

---

## PRD #9: Orchestrator Routing Logic (MEDIUM)

**Issue URL**: <https://github.com/rjmurillo/vs-code-agents/issues/24>
**Title**: `enhancement: Implement orchestrator decision logic for task routing`
**Priority**: MEDIUM
**Complexity**: Medium
**Effort**: 1-2 weeks

### Problem Statement

Orchestrator needs systematic decision logic for routing tasks to appropriate agents. Currently relies on implicit heuristics, making it difficult to implement or automate.

### Goals

1. Create explicit orchestrator routing algorithm
2. Enable automation of agent selection
3. Document agent sequencing (serial vs. parallel)

### Success Criteria

- ✅ Routing algorithm documented and implementable
- ✅ Algorithm handles complex multi-domain tasks
- ✅ Can be automated or used as reference guide

### Requirements

#### Functional Requirements

1. **FR-1: Routing Algorithm**
   - Pseudocode/flowchart for:
     - Task type classification (feature, bug, infrastructure, security, strategic)
     - Complexity assessment (simple, multi-step, multi-domain)
     - Risk level assessment (low, medium, high, critical)
     - Agent selection based on above
     - Execution strategy (serial vs. parallel agents)
     - Result synthesis

2. **FR-2: Execution Strategy**
   - Define when agents run sequentially vs. in parallel
   - Example:
     - SERIAL: Design blocks implementation (architect → critic → implement)
     - PARALLEL: Design and security can happen together (architect + security → implement)

3. **FR-3: Result Synthesis**
   - How to combine outputs from multiple agents
   - Conflict resolution (if agents disagree)
   - Unified plan creation

#### Non-Functional Requirements

1. **NFR-1**: Clear and implementable
2. **NFR-2**: Documented well enough for automation

### Acceptance Criteria

- [ ] Routing algorithm documented with pseudocode/flowchart
- [ ] Agent sequencing documented (serial vs. parallel)
- [ ] Result synthesis strategy documented
- [ ] Tested against this session's complex task (would have been routed correctly)

### Dependencies

- Depends on PRD #1 (Entry Criteria) - must know when orchestrator invoked
- Depends on PRD #3 (Capabilities Matrix) - must know agent capabilities for routing

### Timeline

- **Week 1**: Design algorithm, create flowchart
- **Week 2**: Document sequencing, result synthesis

---

## Summary Table

| # | Title | Priority | Complexity | Effort | Depends On |
|---|-------|----------|-----------|--------|-----------|
| 1 | Orchestrator Entry Criteria | CRITICAL | Medium | 2-3d | None |
| 2 | Security Agent Enhancement | CRITICAL | High | 2-4w | #3 |
| 3 | Agent Capabilities Matrix | CRITICAL | Medium | 1-2w | None |
| 4 | Security Agent Auto-Detection | HIGH | Medium | 1-2w | #3 |
| 5 | Threat Model Documentation | HIGH | Medium | 1-2w | Session analysis |
| 6 | Agent System Governance | HIGH | High | 2-3w | #3 |
| 7 | Agent Invocation Metrics | MEDIUM | Medium | 1-2w | #3 |
| 8 | Capabilities Discovery Protocol | MEDIUM | Low | 3-5d | None |
| 9 | Orchestrator Routing Logic | MEDIUM | Medium | 1-2w | #1, #3 |

### Filing Recommendation

**Phase 1 (File Immediately)**:

- #1, #3, #5 (Independent, high impact)

**Phase 2 (File After Phase 1)**:

- #2, #4, #6, #7, #8, #9 (Depend on Phase 1 or each other)

**Rationale**: Phase 1 establishes foundations that Phase 2 builds on. Filing in phases allows vs-code-agents team to prioritize and sequence work effectively.

---

## Evidence Base

All PRDs reference this session's incident:

- **CWE-78 Shell Injection** in `.githooks/pre-commit`
- **Root Cause**: Security agent not invoked for infrastructure changes
- **Symptom**: Vulnerability discovered during PR review by external bot (reactive, not proactive)
- **Fix**: Commit `2dd96a6d` - Use bash arrays with quoted expansion

**Related Documents**:

- `.agents/retrospective/security-shift-left-gap.md` - Original retrospective
- `.agents/retrospective/RETROSPECTIVE-CONSENSUS.md` - Consensus version with trade-off analysis
- `.agents/analysis/rca-agent-system-gaps.md` - Root cause analysis
- `.agents/analysis/orchestrator-capabilities-matrix.md` - Orchestrator assessment
- `.agents/analysis/security-agent-enhancement.md` - Security agent expansion
- `.agents/security/shell-injection-fix.md` - Technical vulnerability details

     ```
