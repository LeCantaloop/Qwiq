# Root Cause Analysis: Agent System Gaps & Remediation Plan

**Date**: 2025-12-13
**Analysis Focus**: Why agent system exists but is not operationalized
**Scope**: Issues identified in `security-shift-left-gap.md` and architect/critic reviews
**Status**: Actionable RCA with remediation tracking

---

## Executive Summary

The agent system was built (15 agents documented), deployed (`.agents/` directory structure created), and documented (AGENT-INSTRUCTIONS.md updated), but it is **not operationalized** (rarely invoked in practice).

This is a classic **deployment gap**: the system exists architecturally but not operationally. Root causes fall into three categories:

1. **Architectural Complexity** - 15 agents with unclear invocation rules
2. **Process Gaps** - No decision gates, no enforcement, no routing
3. **Organizational Friction** - Documentation-first approach, developer skepticism, no metrics

This analysis performs a 5-Why investigation into each root cause and identifies specific, measurable remediation items.

---

## Root Cause Analysis by Issue

### Issue 1: Agent System Exists But Isn't Routinely Referenced

**Symptom**: Security/devops agents not invoked for CI/infrastructure changes despite being documented in AGENT-INSTRUCTIONS.md

**5-Why Analysis**:

```text
1. Why aren't agents invoked?
   → No systematic decision point or routine to invoke them

2. Why is there no decision point?
   → Developers don't know when to invoke which agent
   → Agent selection matrix never created despite being recommended in docs

3. Why don't developers know which agent to use?
   → 15 agent types with minimal decision support
   → No heuristics for "given change type X, invoke agents Y"
   → Decision trees not created despite mention in workflows

4. Why wasn't the decision support created?
   → It exists in documentation as aspirational ("should include")
   → It was never built as operational artifacts (flowcharts, CLI tools, integrations)
   → Responsible party/owner was never assigned

5. Why were artifacts not built?
   → Assumption that documented workflows are self-executing
   → No feedback mechanism when documentation is ignored
   → No metrics tracking agent invocation rate
```

**Root Cause**: The agent system was designed as a **reference architecture** (documentation describing ideals) rather than an **operational system** (tooling enforcing behavior).

---

### Issue 2: Agent System Complexity Exceeds Practical Usage

**Symptom**: 15 agent types available, but only 2-3 used routinely (general-purpose, implementation agents)

**5-Why Analysis**:

```text
1. Why are only 2-3 agents used out of 15?
   → Developer cognitive load exceeds practical invocation threshold

2. Why is cognitive load too high?
   → 15 distinct agent types with overlapping responsibilities
   → No clear priority or tiering (core vs. optional agents)
   → No visual workflow or decision tree to navigate options

3. Why wasn't the complexity reduced?
   → Each agent was added independently based on historical need
   → No periodic review of agent catalog for consolidation
   → No "maximum agent count" architectural constraint

4. Why are agents added without consolidation review?
   → Agent system treated as "add-on" rather than core process
   → No governance/steering committee for agent system evolution
   → No architectural review board approving new agents

5. Why wasn't agent system elevated to core process status?
   → It was introduced as a "nice-to-have" optimization, not essential
   → Business justification never formalized
   → Success metrics never defined to demonstrate value
```

**Root Cause**: **Unmanaged growth** - The agent system was never treated as a core architectural component requiring governance, leading to complexity exceeding practical usage.

---

### Issue 3: No Feedback Loop When Agents Are Skipped

**Symptom**: Security agent not invoked for infrastructure changes, but only discovered during external PR review by GitHub Copilot bot

**5-Why Analysis**:

```text
1. Why wasn't the skip detected internally?
   → No monitoring/detection of missed agent invocations

2. Why is there no monitoring?
   → Agent invocations are voluntary (pull-based)
   → No registry or log of which agents were used
   → Git commits don't capture agent metadata

3. Why is invocation voluntary instead of enforced?
   → Would require pre-commit hooks or CI gates (implementation effort)
   → Seen as adding friction to developer workflow
   → No agreement that friction is justified by value

4. Why wasn't the value quantified to justify enforcement effort?
   → Agent system success metrics never defined
   → No baseline: "X% of infrastructure changes reviewed before security fix"
   → No target: "Y% post-implementation"
   → No tracking of prevented issues

5. Why aren't metrics defined for agent system?
   → Agent system not managed as a measured product/process
   → Treated as developer-optional tool rather than mandatory gate
   → Success defined vaguely ("better code") rather than specifically
```

**Root Cause**: **Lack of observability and enforcement** - Without metrics and enforcement, agent invocations are invisible, skips are undetected, and feedback is reactive (external PR review).

---

### Issue 4: No Agent Invocation Checklist for Infrastructure Changes

**Symptom**: When CI/workflow changes made, no systematic checklist or gate requires agent review before implementation

**5-Why Analysis**:

```text
1. Why is there no checklist?
   → No detection of "this is an infrastructure change"

2. Why aren't infrastructure changes detected/flagged?
   → No pattern matching on file types (.github/workflows/*, .githooks/*)
   → No pre-commit hook to intercept infrastructure changes
   → Manual compliance assumption ("developers will know it's infrastructure")

3. Why wasn't pattern matching implemented?
   → Would require development effort (bash/Python scripting)
   → Pre-commit hooks sometimes seen as annoying/friction-generating
   → Believed that documentation sufficient to indicate requirements

4. Why trust documentation when it was ignored before?
   → First time discovering this gap (agent system is new, ~2 months old)
   → Assumption that documentation in `.agents/` is consulted
   → No feedback loop showing documentation was ignored

5. Why assume documentation is sufficient?
   → Historical precedent: existing documentation (CONTRIBUTING.md, README) generally followed
   → Agent system framed as "best practices guide" rather than "mandatory requirements"
   → No comparison to equivalent processes in other repos with strong compliance
```

**Root Cause**: **Over-trust in documentation** - The project assumed developers would consult `.agents/` documentation when making infrastructure changes, but assumptions about developer behavior were never validated.

---

### Issue 5: Agent Selection Heuristics Not Documented

**Symptom**: AGENT-INSTRUCTIONS.md exists but doesn't provide "given change type X, invoke agents Y" guidance

**5-Why Analysis**:

```text
1. Why aren't heuristics documented?
   → They were never explicitly written down

2. Why were they never written down?
   → Heuristics existed implicitly in retrospective analysis (agent that did the work)
   → Never distilled into explicit decision rules for future use
   → Assumed developers could infer rules from agent descriptions

3. Why weren't they inferred from agent descriptions?
   → Agent descriptions written for agents themselves, not for selection
   → 15 descriptions create choice paralysis
   → Overlap between agents (e.g., architect vs. csharp-pod for design)

4. Why is there description overlap?
   → Agents built incrementally based on specific sessions
   → No consolidation/deduplication pass
   → No master catalog showing agent selection criteria

5. Why was there no consolidation pass?
   → Agent system not managed as a product requiring maintenance
   → Assumed "initial creation = complete"
   → No periodic review cadence established
```

**Root Cause**: **Lack of proactive documentation maintenance** - Heuristics were never extracted from practice and documented for future reference.

---

### Issue 6: Security Mindset Not Applied to Infrastructure Code

**Symptom**: Pre-commit hook with unquoted variable expansions (shell injection vulnerability) not caught by security review before implementation

**5-Why Analysis**:

```text
1. Why wasn't security mindset applied?
   → Security agent not invoked for infrastructure changes

2. Why isn't security agent invoked?
   → (See Issue 1-5 above, but specific angle:)
   → Developer did not perceive pre-commit hook as "security-relevant"

3. Why didn't developer perceive hook as security-relevant?
   → Hook is "linting" (seen as maintenance, not security)
   → Hook runs locally (seen as low-risk compared to CI)
   → No explicit guidance: "pre-commit hooks are security-critical"

4. Why is this guidance absent?
   → Security domain expertise not consulted during hook creation
   → Security agent exists but wasn't in workflow
   → Hook documented as "developer experience" improvement, not security boundary

5. Why wasn't hook framed as security boundary?
   → Hooks run with developer privilege (implicit security implication)
   → Hooks can execute arbitrary commands (security implication)
   → These implications not documented in hook itself or in CONTRIBUTING.md
```

**Root Cause**: **Implicit vs. explicit threat model** - The security implications of hooks (running with developer privilege) were obvious to security experts but not documented where developers make decisions.

---

## Summary: Root Causes by Category

### Architectural Root Causes

| Root Cause | Impact | Evidence |
|-----------|--------|----------|
| **Reference architecture, not operational system** | Agent invocations remain voluntary | Docs exist, system not used |
| **Unmanaged growth (15 agents)** | Cognitive load exceeds usage | Only 2-3 agents used routinely |
| **No consolidation/deduplication** | Unclear which agent for which task | Overlap between architect/csharp-pod |
| **Implicit threat models** | Security boundaries not recognized | Hook created without security review |

### Process Root Causes

| Root Cause | Impact | Evidence |
|-----------|--------|----------|
| **No decision gates/checkpoints** | Infrastructure changes bypass review | Workflow change made without agent invocation |
| **Documentation-first approach** | Assumes reading, doesn't enforce | Agent docs ignored in favor of direct implementation |
| **No mandatory checklist** | Reviewable only if developer remembers | Hook created without mentioning `.agents/AGENT-INSTRUCTIONS.md` |
| **No pattern matching/detection** | Invisible when violations occur | File changes to `.githooks/` and `.github/workflows/` not flagged |

### Organizational Root Causes

| Root Cause | Impact | Evidence |
|-----------|--------|----------|
| **No observability/metrics** | Invisible process gaps | Vulnerability discovered by external bot, not internal system |
| **No governance structure** | Agents accumulate without review | 15 agents without steering committee or max count |
| **Assumption-driven (not validation-driven)** | False assumptions about behavior | "Developers will know it's infrastructure" → violated |
| **No "owner" assigned** | Nobody responsible for system health | Documentation created but never maintained/enforced |

---

## Remediation Plan by Severity & Scope

### CRITICAL - Must Fix Before Production Use

#### CRIT-001: Establish Mandatory Security Review Gate for Infrastructure Changes

**Problem**: Shell injection vulnerability in pre-commit hook wasn't caught because security agent wasn't invoked

**Root Cause**: No decision gate, no detection, no mandatory checklist

**Remediation**:

```markdown
## Deliverables

1. Create `.github/PULL_REQUEST_TEMPLATE.md` with mandatory section:
   ```markdown
   ## Security Review (if infrastructure change)
   - [ ] This PR modifies `.github/workflows/*`, `.githooks/*`, or build scripts
   - [ ] If YES: Confirm security agent reviewed and approved
   - [ ] Security agent review: [link to agent review in PR comments]
   ```

1. Add pre-commit hook detection:
   - Detect: files matching `.github/workflows/*`, `.githooks/*`, `build/scripts/*`
   - Action: Warn (non-blocking) that infrastructure change detected
   - Message: "Infrastructure change detected. Ensure security agent reviews this PR."

2. Create CI gate in `.github/workflows/main.yml`:
   - Check: PR comments contain evidence of security agent review for infrastructure changes
   - Block merge if: Infrastructure files changed AND no security review comment found
   - Allow bypass with explicit `[no-review]` comment + justification

3. Create `.agents/INFRASTRUCTURE-CHANGE-CHECKLIST.md`:
   - Mandatory when `.github/workflows/*`, `.githooks/*`, or `build/scripts/*` changed
   - Embedded in PR template
   - Requires checkbox: "Security review completed"

4. File upstream issue: `rjmurillo/vs-code-agents#NNN`
   - Title: "Add mandatory security review gate for infrastructure changes"
   - Description: Explain pre-commit hook shell injection discovery, recommend template + CI check

**Effort**: Medium (2-3 days for CI integration)
**Risk**: Low (non-blocking warnings first, enforcement later)
**Upstream Needed**: YES - General pattern, not Qwiq-specific

---

#### CRIT-002: Reduce Agent Count & Create Clear Selection Heuristics

**Problem**: 15 agents with unclear selection rules → developers skip system due to cognitive load

**Root Cause**: Unmanaged growth, no consolidation, implicit rather than explicit heuristics

**Remediation**:

```markdown
## Deliverables

1. Audit all 15 agents and categorize by frequency/importance:
   - TIER 1 (Core): Used in 80%+ of workflows
   - TIER 2 (Common): Used in 20-80% of workflows
   - TIER 3 (Specialist): Used in <20% of workflows

   From retrospective analysis:
   - TIER 1: general-purpose, csharp-expert, qa
   - TIER 2: architect, analyst, planner, critic, retrospective
   - TIER 3: Others (skillbook, memory, feature-request-review, etc.)

2. Create explicit selection heuristics:
   - Single decision tree (visual flowchart, not text list)
   - Max 3 questions to determine which agents to invoke
   - Embedded in `.agents/QUICK-START.md` (new file)
   - Example:
     ```text
     Is this a code change?
     ├─ YES → Is it a feature? → Call: analyst, architect, planner, critic
     ├─ YES → Is it a bug? → Call: csharp-expert, qa
     ├─ NO → Is it infrastructure? → Call: architect, devops, security (if scripts)
     └─ NO → Is it documentation? → Call: (skip agent system, direct write)
     ```

3. Consolidate agent descriptions in `.agents/AGENT-SYSTEM.md`:
   - Remove redundant agents (e.g., merge csharp-pod into csharp-expert if overlap)
   - Add selection criteria to each agent description
   - Add "when to use" heuristics

4. Create agent "quick start" at `.agents/QUICK-START.md`:
   - Two-question flowchart for 80% of use cases
   - Fallback to full `.agents/AGENT-SYSTEM.md` for edge cases
   - Examples: "I'm implementing a feature", "I'm fixing a bug", "I'm changing CI"

5. File upstream issue: `rjmurillo/vs-code-agents#NNN`
   - Title: "Consolidate agent catalog and create selection heuristics"
   - Description: Current 15 agents have cognitive overhead, propose TIER 1-3 categorization + decision tree
   - Reference: This session's retrospective showing agents skipped due to complexity
```

**Effort**: Medium (1-2 days for consolidation, 1 day for decision tree design)
**Risk**: Low (documentation only, no behavior change)
**Upstream Needed**: YES - Pattern applies to all agent-based systems

---

#### CRIT-003: Implement Observability & Metrics for Agent System

**Problem**: Agent invocations invisible, skips undetected, feedback only from external PR review

**Root Cause**: No metrics, no logging, no feedback loops

**Remediation**:

```markdown
## Deliverables

1. Define success metrics for agent system:
   - Baseline: % of infrastructure changes with security review (currently 0%)
   - Target: 100% of infrastructure changes with security agent comment in PR
   - Measurement: Automated PR analysis (grep for `security-agent:` comments)

2. Create `.agents/METRICS.md` documenting:
   - How to measure agent invocation rate
   - How to measure agent effectiveness (prevented issues, cycle time impact)
   - Reporting frequency (monthly review of metrics)
   - Dashboard (if possible) tracking invocation rates

3. Add agent review metadata to commits:
   - Recommended: Include `Agent-Review: [agent-name]` in commit messages
   - Tracked in: `.agents/usage-log.md` updated quarterly
   - Example: "Reviewed-By: security-agent, architect-agent"

4. Create CI job to track metrics:
   - Parse `.agents/critique/`, `.agents/qa/`, `.agents/security/` files
   - Count agent reviews by type
   - Monthly report generated and stored in `.agents/metrics/`
   - Trend analysis: "Agent usage increasing? Decreasing? Why?"

5. File upstream issue: `rjmurillo/vs-code-agents#NNN`
   - Title: "Add observability and metrics to agent system"
   - Description: Recommend tracking agent invocation rates, effectiveness metrics
   - Propose standard metadata format for agent reviews in commits/PRs
```

**Effort**: Low (2-3 days for initial metrics setup, then automated)
**Risk**: Low (measurement only, no enforcement)
**Upstream Needed**: YES - Generic metrics framework, not Qwiq-specific

---

### HIGH - Should Fix in Next Sprint

#### HIGH-001: Create Explicit Threat Model for Infrastructure Changes

**Problem**: Pre-commit hook shell injection not recognized as security-critical

**Root Cause**: Implicit threat model (developers don't know hooks run with privilege)

**Remediation**:

```markdown
## Deliverables

1. Document threat model for infrastructure changes:
   - File: `.agents/analysis/infrastructure-threat-model.md`
   - Cover: Pre-commit hooks, CI workflows, build scripts, .editorconfig (analyzer config)
   - For each: document privilege level, what it can access, potential attack vectors

2. Example for pre-commit hooks:
   ```

## Pre-commit Hooks: Threat Model

### Privilege Level: Developer Machine, Full Developer Privileges

- Runs with: Developer's user ID
- Access: Full filesystem (user's home, project directory, etc.)
- Can execute: Arbitrary commands (npx, dotnet, bash, etc.)
- Risk: Shell injection via filenames (CWE-78)
- Risk: Command injection via environment variables
- Risk: Privilege escalation if hook runs with elevated privileges
- Mitigation: Use arrays for filenames, validate inputs, document assumptions

2. Add threat model reference to hook header:

   ```bash
   # Threat Model: https://github.com/rjmurillo/Qwiq/blob/develop/.agents/analysis/infrastructure-threat-model.md
   # Risk Level: HIGH (runs with developer privileges)
   ```

1. Create `.agents/INFRASTRUCTURE-SECURITY.md`:
   - When to involve security agent
   - Threat categories (CWE-78, environment variable injection, etc.)
   - Review checklist (what security agent looks for)

2. File upstream issue: `rjmurillo/vs-code-agents#NNN`
   - Title: "Document threat models for infrastructure code categories"
   - Description: Propose explicit threat models for pre-commit hooks, CI workflows, build scripts
   - Reference: Shell injection discovery in this session

**Effort**: Low (1 day to write threat model doc)
**Risk**: Low (documentation only)
**Upstream Needed**: YES - Infrastructure threat modeling applies broadly

---

#### HIGH-002: Create ADR-011: Agent System Governance

**Problem**: No architectural decision document for mandatory agent gates

**Root Cause**: New system not formalized through ADR process

**Remediation**:

```markdown
## Deliverable

Create `.agents/architecture/ADR-011-agent-system-governance.md`:

Key sections:
1. Decision: Mandatory security agent review for infrastructure changes
2. Context: Shell injection vulnerability discovered in pre-commit hook
3. Options considered:
   - Option A: Mandatory review + CI enforcement (recommended)
   - Option B: Mandatory checklist only (lower friction)
   - Option C: Post-commit review (lower friction, reactive)
4. Decision: Option A (mandatory + enforcement)
5. Consequences:
   - Pro: Early detection of infrastructure vulnerabilities
   - Pro: Consistent security posture
   - Con: 30-45 minute delay for infrastructure changes
   - Con: Requires CI gate implementation
6. Acceptance criteria:
   - [x] ADR-011 approved by architecture team
   - [ ] CI gate implemented (Phase 2)
   - [ ] Metrics dashboard created (Phase 3)
   - [ ] 6-month review: measure effectiveness
```

**Effort**: Low (1 day to write ADR)
**Risk**: Low (decision documentation only)
**Upstream Needed**: NO - Qwiq-specific governance decision

---

### MEDIUM - Next Iteration

#### MED-001: Extend Pre-commit Hook with Change Type Detection

**Problem**: Infrastructure changes not automatically flagged

**Root Cause**: Manual compliance model, no detection

**Remediation**:

```markdown
## Deliverables

1. Extend `.githooks/pre-commit` with change detection:
   ```bash
   # Detect infrastructure changes
   INFRASTRUCTURE_FILES=$(git diff --cached --name-only | grep -E '\.github/workflows/|\.githooks/|build/scripts/')

   if [ -n "$INFRASTRUCTURE_FILES" ]; then
       echo_warning "Infrastructure change detected:"
       echo_warning "Please ensure the following agents have reviewed this change:"
       echo_warning "  - security agent (for vulnerabilities)"
       echo_warning "  - architect agent (for design)"
       echo_warning "Please document review in PR description."
   fi
   ```

1. No blocking initially (warning only) to avoid friction
2. Version 2 (next sprint): Make blocking with bypass option
3. Enforcement: PR template checkbox confirms review

**Effort**: Low (1 day implementation)
**Risk**: Low (warning only)
**Upstream Needed**: NO - Hook implementation specific to Qwiq

---

#### MED-002: Create Agent Invocation Log/Registry

**Problem**: No record of which agents reviewed infrastructure changes

**Root Cause**: No registry, reviews not tracked

**Remediation**:

```markdown
## Deliverables

1. Create `.agents/usage-log.md`:
   - Maintained quarterly
   - Format: Date | Change Type | Agents Involved | Issue # | Notes
   - Example:
     ```
     | 2025-12-13 | Infrastructure | security | #113 | Shell injection fix in pre-commit |
     | 2025-12-13 | Feature | analyst,architect,planner,critic | --- | Auth refactoring |
     ```

2. CI job to generate usage report:
   - Parse agent review comments in merged PRs
   - Count by agent type, change type
   - Monthly trend report

3. Dashboard (if GitHub Pages available):
   - Visual: Agent invocation rate by month
   - Visual: Agent effectiveness (issues prevented vs. false positives)
   - Trend: Are we shifting left? (earlier detection?)

**Effort**: Low (1 day for log structure, 2 days for CI job)
**Risk**: Low (measurement only)
**Upstream Needed**: NO - Qwiq usage tracking

---

### LOW - Nice-to-Have Improvements

#### LOW-001: Create Agent System UI/CLI

**Problem**: Agent invocation still manual (requires reading docs)

**Root Cause**: Aspirational design, no tooling

**Remediation**:

```markdown
## Deliverables (Future)

1. CLI tool: `claude-agents.sh` or `poetry run agents`
   ```bash
   $ claude-agents suggest
   > Analyzing change type...
   > Files changed: .github/workflows/lint.yml
   > Detected: Infrastructure change
   > Recommended agents: architect, devops, security
   > Invoke? (Y/n)
   ```

1. VS Code extension (future):
   - Detects file being edited
   - Shows recommended agents in sidebar
   - One-click invoke

**Effort**: High (2+ weeks)
**Risk**: Medium (new tool requires maintenance)
**Upstream Needed**: MAYBE - If building as part of vs-code-agents suite

---

## Upstream Issues to File

### Issues for `rjmurillo/vs-code-agents` Repository

Based on analysis, recommend filing these issues upstream:

#### Issue 1: Mandatory Security Review Gates for Infrastructure

**Title**: `enhancement: Add mandatory security review gates for infrastructure changes`
**Description**:

```markdown
## Problem
This session discovered shell injection vulnerabilities in pre-commit hook that
were not caught until external GitHub Copilot bot review. Root cause: security
agent was not invoked because no mandatory gate existed for infrastructure changes.

## Root Cause
Agent system is "reference architecture" (documentation) rather than operational
system (enforcement). Security review gate is aspirational but not implemented.

## Proposed Solution
1. Add PR template requiring security review for infrastructure changes
2. Add CI gate that blocks merge if security review missing
3. Add pre-commit hook detection for infrastructure file patterns
4. Document threat models for infrastructure code categories

## Implementation
See: https://github.com/rjmurillo/Qwiq/blob/develop/.agents/retrospective/security-shift-left-gap.md
```

#### Issue 2: Agent System Complexity - Consolidate & Simplify

**Title**: `enhancement: Reduce agent complexity with clear selection heuristics`
**Description**:

```markdown
## Problem
15 agents with unclear selection rules create cognitive overload. Developers
skip agent system in favor of direct implementation due to decision paralysis.

## Evidence
- Only 2-3 agents used out of 15 documented
- Multiple agents with overlapping responsibilities (e.g., architect vs. csharp-pod)
- No clear decision tree for "which agent should I use?"

## Proposed Solution
1. Categorize agents into TIER 1 (core), TIER 2 (common), TIER 3 (specialist)
2. Create single-page decision tree/flowchart
3. Consolidate overlapping agents
4. Document selection heuristics explicitly

## Impact
Reduced cognitive load → higher agent invocation rate → better code quality
```

#### Issue 3: Add Observability & Metrics to Agent System

**Title**: `enhancement: Add metrics and observability to agent system`
**Description**:

```markdown
## Problem
Agent invocations are invisible. Skip detection only happens via external
PR review (reactive, not proactive). No way to measure if shift-left is working.

## Proposed Solution
1. Define metrics: % of infrastructure changes with security review
2. Add agent review metadata to commits/PRs
3. Create dashboard tracking invocation rates by agent type
4. Monthly trend reporting

## Benefits
- Visibility into agent system health
- Early detection when agents are skipped
- Data-driven improvements
```

#### Issue 4: Document Threat Models for Infrastructure Code

**Title**: `documentation: Create explicit threat models for infrastructure code`
**Description**:

```markdown
## Problem
Pre-commit hooks run with developer privileges (security-critical) but this
wasn't recognized as security-relevant. Shell injection vulnerability not
caught because threat model was implicit, not explicit.

## Proposed Solution
1. Document threat model for pre-commit hooks (CWE-78, etc.)
2. Document threat model for CI workflows
3. Document threat model for build scripts
4. Reference threat models in code where infrastructure runs

## Benefits
- Developers understand "why" security review matters for infrastructure
- Explicit boundaries (code vs. infrastructure vs. CI)
```

#### Issue 5: Create Agent System Governance (ADR Template)

**Title**: `enhancement: Add ADR template for agent system governance decisions`
**Description**:

```markdown
## Problem
Agent system lacks governance structure. Each repository adds agents
independently without consolidation or review.

## Proposed Solution
1. Create ADR template for agent-related decisions
2. Recommend ADR for any decision to add/remove/modify agents
3. Establish steering committee review for new agents
4. Define maximum agent count constraint

## Upstream Benefit
Provides structure for managing agent systems across multiple repositories
```

---

## Implementation Roadmap

### Phase 1: Immediate (This Week) - Foundation

- [ ] Create `.agents/QUICK-START.md` with decision tree
- [ ] Add PR template with infrastructure change checkbox
- [ ] File 5 upstream issues to `rjmurillo/vs-code-agents`
- [ ] Create threat model document

**Effort**: 2-3 days
**Value**: Sets foundation, unblocks Phase 2

### Phase 2: Near-term (Next Sprint) - Detection & Logging

- [ ] Extend pre-commit hook with change detection
- [ ] Create agent invocation log/registry
- [ ] Implement CI metrics job
- [ ] Create ADR-011 (governance)

**Effort**: 1 week
**Value**: Observability, early warning signals

### Phase 3: Medium-term (Next Month) - Enforcement

- [ ] Implement CI gate blocking infrastructure changes without review
- [ ] Create dashboard with metrics/trends
- [ ] Test enforcement with team feedback
- [ ] Adjust friction/bypass model based on data

**Effort**: 2 weeks
**Value**: Operational enforcement, compliance assurance

### Phase 4: Long-term (Next Quarter) - Optimization

- [ ] Build agent system CLI/UI for easier invocation
- [ ] Consolidate agents (reduce 15 → 8-10)
- [ ] Review effectiveness metrics (6-month assessment)
- [ ] Consider VS Code extension integration

**Effort**: 4+ weeks
**Value**: Improved developer experience, higher compliance

---

## Success Criteria

| Metric | Current | Target (3 months) |
|--------|---------|------------------|
| Infrastructure changes with security review | 0% | 100% |
| Agent system invocation rate | ~15% | 80%+ |
| Time to detect infrastructure issues | Post-merge | Pre-merge |
| Developer satisfaction with agent system | Unknown | 4/5+ (survey) |
| Shift-left effectiveness | Unknown | TBD after metrics |

---

## Escalation & Responsibilities

### Internal (Qwiq Repository)

- **Retrospective revision**: Incorporate architect/critic feedback → [Owner: ]
- **PR template + checklist**: Implement infrastructure change gate → [Owner: ]
- **Pre-commit hook extension**: Add change detection → [Owner: ]
- **Threat model documentation**: Write infrastructure security docs → [Owner: ]
- **ADR-011**: Formalize governance decision → [Owner: ]

### Upstream (rjmurillo/vs-code-agents)

- **File 5 issues**: File issues for consolidation, metrics, threat models → [Owner: ]
- **Engage steering committee**: Discuss agent complexity/consolidation → [Owner: ]
- **Reference Qwiq experience**: Share retrospective as case study → [Owner: ]

---

## Lessons Learned & Principles

### Key Takeaway #1: Documentation ≠ Operations

Systems must be **operationalized**, not just documented. Reference architectures need operational enforcement to be effective.

### Key Takeaway #2: Implicit Models Are Dangerous

Threat models, design assumptions, and architectural decisions must be **explicit**, not implicit. Write them down where they're used.

### Key Takeaway #3: Complexity Requires Governance

Unconstrained growth (15 agents, multiple overlapping agents) requires active governance, not passive documentation.

### Key Takeaway #4: Feedback Loops Matter

Invisible process gaps (agent skips) only get caught reactively (external PR review). Build feedback loops to detect gaps proactively.

### Principle: Shift-Left Requires Gates + Metrics + Enforcement

Aspirational workflows don't self-execute. Operationalize through: decision gates (when to invoke) + metrics (measuring effectiveness) + enforcement (preventing violations).

---

## Conclusion

The agent system's deployment gap stems from architectural complexity, process gaps, and organizational assumptions. This RCA identifies 10 specific remediation items across four severity levels, with 5 upstream issues for `rjmurillo/vs-code-agents`.

**Critical path**: CRIT-001 (security gate) + CRIT-003 (metrics) + HIGH-002 (ADR) in next sprint.

**Timeline**: Phases 1-2 (foundation + detection) in next 2 weeks. Phases 3-4 (enforcement + optimization) following month.

**Success metric**: 100% of infrastructure changes reviewed by security agent within 3 months.
