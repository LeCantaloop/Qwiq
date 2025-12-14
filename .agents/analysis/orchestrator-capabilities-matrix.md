# Orchestrator Agent: Capabilities Matrix & Coordination Strategy

**Date**: 2025-12-13
**Analysis**: Why orchestrator was not invoked in this session and how to improve
**Status**: Upstream issue candidate for `rjmurillo/vs-code-agents`

---

## Executive Summary

In this session, the **orchestrator agent** should have been invoked at the very first user request: "Create a new GitHub Actions workflow for markdown linting." This is a classic multi-step, multi-specialty task that requires:

1. Change type assessment
2. Delegation to appropriate agents (architect, devops, security)
3. Coordination across design, security, and implementation concerns
4. Synthesis of results into a cohesive implementation plan

**Instead**, the task was directly implemented without orchestrator coordination, leading to shell injection vulnerabilities that were only caught during external PR review.

This analysis proposes:

- **Orchestrator entry criteria** - when orchestrator MUST be invoked
- **Comprehensive capabilities matrix** - what each agent can do
- **Orchestrator decision logic** - how to route tasks to agents
- **Upstream improvements** - enhancements needed in `rjmurillo/vs-code-agents`

---

## When Orchestrator Should Be Invoked

### Entry Criteria: Complex Multi-Specialty Tasks

The orchestrator should be **mandatory** (not optional) when a task has ANY of these characteristics:

```text
Orchestrator Entry Criteria:

Task affects multiple domains?
  ├─ YES → INVOKE ORCHESTRATOR
  ├─ Examples:
  │   - CI change (devops, security, architecture)
  │   - Feature spanning frontend/backend/db
  │   - Refactoring affecting multiple systems
  └─ NO → Proceed to domain-specific agent

Task requires design validation?
  ├─ YES → INVOKE ORCHESTRATOR
  ├─ Examples:
  │   - New workflow, new pattern, architectural change
  │   - Infrastructure code, hooks, build scripts
  └─ NO → Proceed to domain-specific agent

Task involves security implications?
  ├─ YES → INVOKE ORCHESTRATOR
  ├─ Examples:
  │   - Authentication changes
  │   - Infrastructure code (runs with privilege)
  │   - External service integration
  └─ NO → Proceed to domain-specific agent

Task needs pre-implementation review?
  ├─ YES → INVOKE ORCHESTRATOR
  ├─ Examples:
  │   - Large features, architectural decisions
  │   - Any task where design review saves implementation time
  └─ NO → Proceed to domain-specific agent
```

### Session Example: When Orchestrator Should Have Been Used

**User Request**: "Create a new GitHub Actions workflow for markdown linting"

**Should Have Triggered**:

```
✓ Affects multiple domains?
  YES - CI/workflow (devops), architecture (design), security (runs scripts)

✓ Requires design validation?
  YES - Workflow design impacts all developers, CI security model

✓ Involves security?
  YES - CI runs linting scripts with developer privilege, pre-commit hooks

✓ Needs pre-implementation review?
  YES - CI/workflow changes should be designed before implementation
```

**Correct Action**: Invoke orchestrator immediately

**What Actually Happened**: Skipped orchestrator, implemented directly

**Cost**: Shell injection vulnerability discovered only during PR review by external bot

---

## Agent Capabilities Matrix

The orchestrator needs comprehensive knowledge of what each agent can (and cannot) do. This section documents each agent's:

- **Specialty**: Core domain
- **Capabilities**: What it excels at
- **Limitations**: What it cannot do
- **Best paired with**: Which agents to chain to
- **Invocation rules**: When to use this agent

### TIER 1: Core/Mandatory Agents

#### general-purpose

| Property | Value |
|----------|-------|
| **Specialty** | Research, investigation, complex reasoning |
| **Capabilities** | <ul><li>Code search and exploration</li><li>Codebase structure analysis</li><li>Root cause investigation</li><li>Complex multi-step research</li><li>Documentation review</li></ul> |
| **Limitations** | <ul><li>Cannot write production code</li><li>Cannot guarantee complete understanding of complex systems</li><li>May miss edge cases without explicit guidance</li><li>Not specialized for security/architecture decisions</li></ul> |
| **Input** | Research questions, exploration tasks, "investigate why X fails" |
| **Output** | Analysis, findings, architectural insights, implementation strategies |
| **Best paired with** | architect (for design), csharp-expert (for implementation) |
| **Invocation rule** | When investigation/research needed before implementation |

#### csharp-expert

| Property | Value |
|----------|-------|
| **Specialty** | C# code implementation, .NET patterns, production quality |
| **Capabilities** | <ul><li>Write production-ready C# code</li><li>Review code for quality/patterns</li><li>Implement features according to spec</li><li>Optimize performance</li><li>Apply SOLID principles</li><li>Write comprehensive tests</li></ul> |
| **Limitations** | <ul><li>Cannot make architectural decisions alone</li><li>Cannot determine security requirements without input</li><li>Cannot assess business impact</li><li>Not a security specialist</li></ul> |
| **Input** | Implementation tasks, code reviews, feature requests with design |
| **Output** | Production-ready code, test suite, performance optimizations |
| **Best paired with** | architect (before coding), qa (after coding) |
| **Invocation rule** | When implementation work needed for C# features/fixes |

#### qa

| Property | Value |
|----------|-------|
| **Specialty** | Testing strategy, verification, quality assurance |
| **Capabilities** | <ul><li>Design comprehensive test strategies</li><li>Identify test gaps</li><li>Verify implementations meet requirements</li><li>Test edge cases</li><li>Security testing perspective</li><li>Performance testing</li></ul> |
| **Limitations** | <ul><li>Cannot guarantee complete test coverage</li><li>May miss domain-specific edge cases</li><li>Cannot fix code, only test and report</li><li>Cannot make architectural decisions</li></ul> |
| **Input** | Implementations to test, test strategies to validate, "verify this works" |
| **Output** | Test cases, test reports, gap identification, verification pass/fail |
| **Best paired with** | csharp-expert (before) and security (for security tests) |
| **Invocation rule** | ALWAYS after implementation before release |

### TIER 2: Common Agents (Use in ~20-80% of workflows)

#### architect

| Property | Value |
|----------|-------|
| **Specialty** | Architecture decisions, design patterns, system design |
| **Capabilities** | <ul><li>Design system architecture</li><li>Evaluate architectural trade-offs</li><li>Review designs against principles (SOLID, DRY, etc.)</li><li>Create ADRs</li><li>Assess design quality</li><li>Recommend design patterns</li></ul> |
| **Limitations** | <ul><li>Cannot implement code</li><li>Cannot test implementations</li><li>Cannot guarantee security (needs security agent)</li><li>Cannot validate business value</li></ul> |
| **Input** | Design decisions, architecture reviews, "is this design good?" |
| **Output** | ADRs, design recommendations, architectural assessment |
| **Best paired with** | critic (to validate), csharp-expert (for implementation) |
| **Invocation rule** | When architectural decisions needed OR infrastructure changes made |

#### analyst

| Property | Value |
|----------|-------|
| **Specialty** | Root cause analysis, problem investigation, requirements understanding |
| **Capabilities** | <ul><li>Investigate why something fails</li><li>Root cause analysis</li><li>Break down complex problems</li><li>Understand requirements deeply</li><li>Identify hidden assumptions</li></ul> |
| **Limitations** | <ul><li>Cannot implement solutions</li><li>Cannot test solutions</li><li>Cannot make architectural decisions alone</li><li>Cannot provide specific code examples</li></ul> |
| **Input** | "Why does this fail?", problem statements, ambiguous requirements |
| **Output** | Root cause analysis, problem decomposition, requirements clarification |
| **Best paired with** | architect (for design), csharp-expert (for implementation) |
| **Invocation rule** | When root cause investigation needed before fixing |

#### critic

| Property | Value |
|----------|-------|
| **Specialty** | Plan validation, design review, quality gate |
| **Capabilities** | <ul><li>Validate plans before implementation</li><li>Review designs for completeness</li><li>Identify gaps and risks</li><li>Challenge assumptions</li><li>Assess feasibility</li></ul> |
| **Limitations** | <ul><li>Cannot implement</li><li>Cannot provide detailed solutions</li><li>Cannot test</li><li>Cannot make architectural decisions</li></ul> |
| **Input** | Plans to review, designs to validate, "is this complete?" |
| **Output** | Validation pass/fail, gap identification, risk assessment |
| **Best paired with** | architect/planner (before), csharp-expert (after) |
| **Invocation rule** | Before implementation on risky/complex changes |

#### planner

| Property | Value |
|----------|-------|
| **Specialty** | Task breakdown, work package creation, milestone planning |
| **Capabilities** | <ul><li>Break large tasks into smaller work packages</li><li>Create implementation roadmaps</li><li>Estimate complexity</li><li>Identify dependencies</li><li>Plan phased rollouts</li></ul> |
| **Limitations** | <ul><li>Cannot implement</li><li>Cannot test</li><li>Cannot make architectural decisions</li><li>Cannot guarantee accuracy of estimates</li></ul> |
| **Input** | Large features/epics, complex projects, "how should we do this?" |
| **Output** | Implementation plan, work packages, milestone schedule |
| **Best paired with** | csharp-expert (for implementation) |
| **Invocation rule** | For large features or multi-sprint work |

#### retrospective

| Property | Value |
|----------|-------|
| **Specialty** | Learning extraction, process improvement, outcome analysis |
| **Capabilities** | <ul><li>Extract lessons from completed work</li><li>Analyze what went well/badly</li><li>Identify improvement opportunities</li><li>Document outcomes</li><li>Root cause analysis on processes</li></ul> |
| **Limitations** | <ul><li>Cannot implement improvements</li><li>Cannot execute on learning</li><li>Cannot predict future issues</li><li>Requires accurate input data</li></ul> |
| **Input** | Session summaries, completed projects, "what did we learn?" |
| **Output** | Retrospective reports, improvement recommendations, skills extracted |
| **Best paired with** | architect/planner (to implement improvements) |
| **Invocation rule** | At session end or major milestone completion |

### TIER 3: Specialist Agents (Use in <20% of workflows)

#### security

| Property | Value |
|----------|-------|
| **Specialty** | Security review, vulnerability assessment, threat modeling |
| **Capabilities** | <ul><li>Threat modeling</li><li>Security review (code/design)</li><li>CWE/CVE analysis</li><li>Vulnerability assessment</li><li>Security testing recommendations</li></ul> |
| **Limitations** | <ul><li>Cannot implement code</li><li>Cannot guarantee zero vulnerabilities</li><li>Cannot test implementations</li><li>Requires expert security knowledge</li></ul> |
| **Input** | Security concerns, threat models, code/design to review |
| **Output** | Threat assessment, vulnerability identification, security recommendations |
| **Best paired with** | qa (for security testing), csharp-expert (for fixes) |
| **Invocation rule** | MANDATORY for infrastructure changes, authentication, crypto |

#### high-level-advisor

| Property | Value |
|----------|-------|
| **Specialty** | Strategic decisions, technology choices, architectural direction |
| **Capabilities** | <ul><li>Evaluate technology choices</li><li>Strategic architecture decisions</li><li>Long-term planning</li><li>Risk assessment of major changes</li><li>ROI analysis</li></ul> |
| **Limitations** | <ul><li>Cannot implement</li><li>Cannot test</li><li>Cannot provide detailed design</li><li>Cannot guarantee perfect decisions</li></ul> |
| **Input** | "Should we use X or Y technology?", strategic questions |
| **Output** | Technology assessment, strategic recommendations |
| **Best paired with** | architect (for detailed design) |
| **Invocation rule** | For major technology/architectural decisions |

#### independent-thinker

| Property | Value |
|----------|-------|
| **Specialty** | Challenge assumptions, unfiltered feedback, alternative perspectives |
| **Capabilities** | <ul><li>Challenge proposed solutions</li><li>Provide unfiltered critique</li><li>Suggest alternatives</li><li>Identify blind spots</li><li>Play devil's advocate</li></ul> |
| **Limitations** | <ul><li>Cannot implement</li><li>Cannot guarantee correctness</li><li>Can be contrarian without value</li><li>Requires good framing of problem</li></ul> |
| **Input** | Plans, designs, proposed solutions to challenge |
| **Output** | Alternative perspectives, critiques, assumptions challenged |
| **Best paired with** | architect/critic (to validate assumptions) |
| **Invocation rule** | For risky decisions or when second opinion needed |

#### create-explainer

| Property | Value |
|----------|-------|
| **Specialty** | Feature documentation, PRDs, specifications |
| **Capabilities** | <ul><li>Write product specs</li><li>Create feature documentation</li><li>Document requirements clearly</li><li>Clarify ambiguous specifications</li></ul> |
| **Limitations** | <ul><li>Cannot implement</li><li>Cannot test</li><li>Cannot make architectural decisions</li><li>Cannot guarantee spec completeness</li></ul> |
| **Input** | Feature ideas, unclear requirements, "document this feature" |
| **Output** | PRDs, specifications, feature documentation |
| **Best paired with** | architect (for design), planner (for roadmap) |
| **Invocation rule** | When documenting features or clarifying specs |

#### generate-tasks

| Property | Value |
|----------|-------|
| **Specialty** | Atomic task creation, task list generation |
| **Capabilities** | <ul><li>Break PRDs into atomic tasks</li><li>Create actionable task lists</li><li>Estimate task size</li><li>Create subtasks</li></ul> |
| **Limitations** | <ul><li>Cannot implement tasks</li><li>Cannot validate plans</li><li>Cannot estimate accurately</li></ul> |
| **Input** | PRDs, feature specs, "generate tasks for this" |
| **Output** | Atomic task lists, estimation, subtask hierarchy |
| **Best paired with** | csharp-expert (for implementation) |
| **Invocation rule** | After PRD/spec created, before implementation |

#### feature-request-review

| Property | Value |
|----------|-------|
| **Specialty** | Feature request evaluation, gap identification |
| **Capabilities** | <ul><li>Review feature requests</li><li>Identify gaps</li><li>Assess feasibility</li><li>Identify risks</li></ul> |
| **Limitations** | <ul><li>Cannot implement</li><li>Cannot guarantee completeness</li><li>Cannot make final decisions</li></ul> |
| **Input** | Feature requests, enhancement requests |
| **Output** | Feature assessment, gap identification, feasibility analysis |
| **Best paired with** | analyst (for investigation) |
| **Invocation rule** | When evaluating new feature requests |

#### memory

| Property | Value |
|----------|-------|
| **Specialty** | Cross-session context, knowledge persistence |
| **Capabilities** | <ul><li>Store learned strategies</li><li>Retrieve context from past sessions</li><li>Maintain knowledge graph</li></ul> |
| **Limitations** | <ul><li>Cannot implement</li><li>Cannot learn without explicit instruction</li><li>Cannot query without defined entities</li></ul> |
| **Input** | "Store this strategy", entity/relationship queries |
| **Output** | Context retrieval, learned strategies, relationships |
| **Best paired with** | retrospective (to store learnings) |
| **Invocation rule** | For cross-session learning/context |

#### skillbook

| Property | Value |
|----------|-------|
| **Specialty** | Skill management, strategy curation |
| **Capabilities** | <ul><li>Manage learned skills</li><li>Curate strategies</li><li>Track skill effectiveness</li></ul> |
| **Limitations** | <ul><li>Cannot implement</li><li>Cannot create skills from scratch</li></ul> |
| **Input** | Skill data, "manage our skills" |
| **Output** | Skill inventory, curation |
| **Best paired with** | retrospective (skill extraction) |
| **Invocation rule** | For skill catalog management |

---

## Orchestrator Decision Logic

### Algorithm: Route Complex Tasks to Appropriate Agents

```text
Orchestrator receives complex task:

Step 1: Assess Change Type
  ├─ Infrastructure (CI, workflows, hooks)?
  │  └─ ROUTE: architect, devops, security, critic → IMPLEMENT → qa
  │
  ├─ Feature (new functionality)?
  │  └─ ROUTE: analyst → architect → planner → critic → csharp-expert → qa
  │
  ├─ Bug fix?
  │  └─ ROUTE: analyst (if RCA needed) → csharp-expert → qa
  │
  ├─ Security change?
  │  └─ ROUTE: security (FIRST) → architect → csharp-expert → qa
  │
  ├─ Architectural decision?
  │  └─ ROUTE: architect → ADR → high-level-advisor (if strategic)
  │
  └─ Strategic decision?
     └─ ROUTE: high-level-advisor → architect → impact analysis

Step 2: Identify Dependencies
  ├─ Design blocks implementation?
  │  └─ SERIAL: design first, then implement
  │
  ├─ Implementation can start while design discussed?
  │  └─ PARALLEL: design and implement together
  │
  └─ No dependencies?
     └─ PARALLEL: all agents work independently

Step 3: Coordinate Results
  ├─ Gather outputs from all agents
  ├─ Identify conflicts
  ├─ Reconcile findings
  ├─ Synthesize plan
  └─ PRESENT: coherent implementation plan to user

Step 4: Monitor Execution
  ├─ Ensure agents stick to plan
  ├─ Escalate blockers
  ├─ Coordinate handoffs
  └─ Feed back into next iteration
```

### Example: This Session's Task

**Task**: "Create new GitHub Actions workflow for markdown linting"

**Orchestrator Should Have Done**:

```
Step 1: Assess Change Type
  → Infrastructure (CI workflow change)
  → Requires: architect, devops, security, critic, csharp-expert, qa

Step 2: Identify Dependencies
  → Architecture blocks implementation: YES
  → Security review blocks implementation: YES
  → Design and security can happen in PARALLEL
  → Implementation AFTER reviews

Step 3: Coordinate
  devops:    Review workflow design
  security:  Review script execution model (hooks)
  architect: Review separation of concerns
  critic:    Validate design
      ↓
  csharp-expert: Implement workflow + hook fixes
      ↓
  qa:        Test and verify

Step 4: Result
  [Comprehensive plan with security implications identified]
  [Shell injection vulnerability caught BEFORE implementation]
  [No external PR review needed]
```

**What Actually Happened**:

```
User request
  ↓
Direct implementation (skipped orchestrator)
  ↓
Commit workflow change (no security review)
  ↓
PR created
  ↓
External GitHub Copilot bot catches shell injection
  ↓
Reactive fix required
```

---

## Upstream Issues for rjmurillo/vs-code-agents

### Issue 1: Define Orchestrator Entry Criteria

**Title**: `enhancement: Document when orchestrator MUST be invoked`

**Description**:

```markdown
## Problem
The orchestrator agent is powerful but underutilized. Developers don't know
when to invoke it, so complex multi-specialty tasks are attempted without
orchestration coordination.

## Current State
No explicit entry criteria documented. Developers make ad-hoc decisions about
whether to use orchestrator.

## Proposed Solution
Create `.agents/ORCHESTRATOR-ENTRY-CRITERIA.md` documenting:

1. When orchestrator MUST be invoked (mandatory criteria)
2. When orchestrator SHOULD be invoked (recommended)
3. Example decisions tree for different task types
4. Integration with change-type assessment

Entry criteria should trigger automatically on file changes (CI gate).

## Example from Qwiq session:
- CI workflow change should TRIGGER orchestrator invocation
- Currently skipped, causing shell injection vulnerability
- With mandatory entry: vulnerability caught before implementation
```

### Issue 2: Create & Publish Comprehensive Capabilities Matrix

**Title**: `documentation: Publish agent capabilities matrix with limitations`

**Description**:

```markdown
## Problem
Orchestrator (and developers) don't have comprehensive knowledge of what each
agent can and cannot do. This leads to:
- Incorrect agent routing (using wrong agent for task)
- Missed opportunities (not knowing agent can help)
- Inefficient coordination

## Current State
AGENT-SYSTEM.md describes agents but lacks:
- Explicit capabilities list
- Explicit limitations (when to NOT use agent)
- What agents work well together
- Input/output formats

## Proposed Solution
Create comprehensive capabilities matrix documenting for each agent:
1. **Specialty**: Core domain
2. **Capabilities**: What it excels at (5-10 bullet points)
3. **Limitations**: What it CANNOT do (important!)
4. **Best paired with**: Which agents coordinate well
5. **Input format**: What to ask/provide
6. **Output format**: What to expect
7. **Invocation rule**: When to use this agent

## Benefit
Orchestrator can make intelligent routing decisions because it knows
each agent's actual capabilities, not just the name.

## Reference
See: Qwiq/.agents/analysis/orchestrator-capabilities-matrix.md
```

### Issue 3: Implement Orchestrator Routing Decision Logic

**Title**: `enhancement: Implement orchestrator decision logic for task routing`

**Description**:

```markdown
## Problem
Orchestrator needs systematic decision logic for routing tasks to appropriate
agents. Currently relies on implicit heuristics.

## Proposed Solution
1. Create routing algorithm (flowchart/pseudocode)
2. Implement as executable logic in orchestrator invocation
3. Route based on:
   - Task type (feature, bug, infrastructure, security, strategic)
   - Complexity (simple vs. multi-step vs. multi-domain)
   - Risk level (low, medium, high, critical)
   - Dependencies (serial vs. parallel agents)

4. Coordinate execution:
   - SERIAL: Design → Review → Implement → Test
   - PARALLEL: Design & Security review simultaneously
   - ASYNC: Components that can proceed independently

5. Synthesize results:
   - Gather all agent outputs
   - Identify conflicts
   - Create unified plan
```

### Issue 4: Create "Interview" Protocol for Capabilities Discovery

**Title**: `enhancement: Implement capabilities discovery protocol for agents`

**Description**:

```markdown
## Problem
As agents are added/updated, their capabilities need to be formally documented.
Current process is informal and incomplete.

## Proposed Solution
Create "agent interview" protocol where:
1. New agent describes itself
2. Interview questions:
   - What is your core specialty?
   - What specific tasks can you handle?
   - What are your limitations?
   - Which agents work well with you?
   - What input format do you expect?
   - What should I expect in your output?
   - When should you be used?
   - When should you NOT be used?

3. Results fed into orchestrator decision logic
4. Capabilities matrix updated
5. Limitations explicitly documented

## Benefit
Living documentation of agent capabilities, kept current as agents evolve.
```

---

## Operational Changes Needed

### In rjmurillo/vs-code-agents (Upstream)

1. **Document orchestrator entry criteria** - Make mandatory for multi-specialty tasks
2. **Publish capabilities matrix** - With explicit limitations
3. **Implement routing logic** - Orchestrator can make intelligent decisions
4. **Create agent interview protocol** - Discover capabilities formally

### In rjmurillo/Qwiq (This Repository)

1. **Pre-commit hook**: Detect infrastructure changes → warn to invoke orchestrator
2. **PR template**: Checkbox for "orchestrator was invoked for this change"
3. **CI gate**: Warn if infrastructure change made without orchestrator invocation (Phase 2: enforce)
4. **Developer docs**: Document "when to invoke orchestrator" in CONTRIBUTING.md

---

## Success Metrics

| Metric | Current | Target (3 months) |
|--------|---------|------------------|
| Orchestrator invocation rate | ~5% | 80%+ for multi-specialty tasks |
| Security vulnerabilities caught pre-implementation | 0% | 90%+ |
| Time to design multi-specialty tasks | Unknown | Reduced (orchestrator efficiently routes) |
| Developer satisfaction: "I know when to use orchestrator" | Unknown | 4/5+ |

---

## Key Insight: Orchestrator Is Not Just Another Agent

The orchestrator is **meta** - it should understand and coordinate other agents. Its value is not in domain expertise but in:

1. **Knowing when others are needed** - Entry criteria
2. **Knowing what they can do** - Capabilities matrix
3. **Routing efficiently** - Decision logic
4. **Synthesizing results** - Coordination

This is fundamentally different from domain agents (csharp-expert, security, architect) which have expertise in specific domains.

**Current problem**: Orchestrator treated like just another agent → underutilized

**Solution**: Position orchestrator as the **routing layer** → every complex task flows through it

---

## Conclusion

The orchestrator is the missing piece in the agent system. This session's shell injection vulnerability would have been caught if:

1. Orchestrator was invoked (entry criteria existing)
2. Orchestrator knew to route to security agent (capabilities matrix)
3. Security review was automatic for infrastructure changes (routing logic)

Upstream issues should focus on operationalizing the orchestrator as a true coordination layer, not just another agent in the catalog.
