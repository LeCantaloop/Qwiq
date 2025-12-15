# Strategy Skills

## Skill-Strategic-001

**Entity Type**: Skill
**Statement**: Always verify deployment scale (100+ team members vs external adoption) before declaring maintenance mode
**Atomicity**: 97%
**Category**: Strategy
**Context**: When making strategic project decisions
**Evidence**: Session 27-28 - Wrong maintenance mode decision reversed after clarification
**Tag**: helpful
**Impact**: 10
**Validated**: 1

**Summary**: Session 27 concluded project was in "maintenance mode" based on incomplete context. Session 28 revealed actual usage: 100+ team members, active external adoption. Wrong conclusion would have halted modernization efforts. Correct decision: Continue modernization with strategic focus.

**Decision Criteria**:

| Factor            | Maintenance Mode | Active Development |
| ----------------- | ---------------- | ------------------ |
| Internal Users    | <10 team members | 100+ team members  |
| External Adoption | None/minimal     | Active/growing     |
| Bug Fix Rate      | <1 per month     | Multiple per week  |
| Feature Requests  | Rare             | Regular            |
| Upgrade Frequency | Annual patches   | Quarterly releases |
| Support Load      | Minimal          | High               |

**Application**:

Before making strategic decisions about a project:

1. **Gather Evidence**:

   - Count internal team members using the library
   - Check external adoption metrics (NuGet downloads, GitHub stars)
   - Review issue/bug report frequency
   - Assess feature request volume

2. **Avoid Assumptions**:

   - Don't assume low GitHub activity = low usage
   - Don't assume old last release = no active development
   - Don't assume no recent commits = no interest

3. **Make Informed Decision**:
   - With full context, choose appropriate strategy
   - Communicate decision to stakeholders
   - Plan accordingly

**Lesson**: Always verify with stakeholders before declaring major strategic changes

---

## Skill-Strategic-002

**Entity Type**: Skill
**Statement**: SOAP clients cannot deploy to Kubernetes (net472 Windows-only); deprecate in favor of REST
**Atomicity**: 94%
**Category**: Strategy
**Context**: When planning container deployment strategy
**Evidence**: ADR-010 - SOAP deprecation strategy for v12.0.0
**Tag**: helpful
**Impact**: 9
**Validated**: 1

**Summary**: SOAP client built for .NET Framework 4.7.2 (Windows-only). Kubernetes deployments require Linux container support. .NET Framework cannot run on Linux. REST client targets modern .NET (cross-platform). Strategic choice: deprecate SOAP, invest in REST.

**Comparison**:

| Aspect           | SOAP Client          | REST Client    |
| ---------------- | -------------------- | -------------- |
| Target Framework | .NET Framework 4.7.2 | net6.0+        |
| Platform Support | Windows-only         | Cross-platform |
| Container Ready  | ❌ No                | ✅ Yes         |
| Kubernetes       | ❌ No                | ✅ Yes         |
| Modern .NET      | ❌ No                | ✅ Yes         |
| Maintenance      | Deprecating          | Active         |

**Deprecation Strategy**:

1. **Current** (v11.x):

   - Both clients available
   - SOAP marked as obsolete in docs

2. **Transition** (v12.0):

   - SOAP client removed from package
   - Migration guide provided
   - REST client as primary

3. **Future** (v13.0+):
   - REST-only implementation
   - Cloud-ready by default

**Note**: Strategic infrastructure decisions should inform library architecture, not vice versa

---

## Skill-Proj-CTX-001

**Entity Type**: Skill
**Statement**: Revalidate priority context when foundational deployment scope or security requirements change between sessions
**Atomicity**: 88%
**Category**: Strategy
**Context**: At session start when context may have shifted (adoption metrics, deployment targets, security reviews)
**Evidence**: Session 27→41 - "Maintenance mode" based on incorrect adoption metrics; production context (100+ users, Kubernetes, enterprise security review) discovered later
**Tag**: helpful
**Impact**: 9
**Validated**: 1

**Summary**: Context drift accumulates across sessions. Enterprise vs. open-source deployment affects priority. Internal vs. external usage impacts feature requirements. Container vs. traditional hosting changes TFM targets. Explicit revalidation is cheaper than mid-sprint misalignment.

**Revalidation Protocol**:

1. **Query: "Has deployment scope changed?"**

   - Internal team only → External adoption
   - Traditional hosting → Container/Kubernetes
   - Single platform → Multi-platform

2. **Query: "Has security requirements changed?"**

   - OSS standards → Enterprise security review
   - Public packages → Private registry
   - No signing → Package signing required

3. **If YES to any**: Re-align priorities accordingly

**Real Example** (Sessions 27-41):

| Session | Context Assumption     | Actual Context        | Impact                   |
| ------- | ---------------------- | --------------------- | ------------------------ |
| 27      | Low adoption, OSS-only | 100+ team members     | Declared maintenance     |
| 28      | (Clarified by user)    | Enterprise Kubernetes | Re-activated Waves 3-5   |
| 41      | Priorities confirmed   | Production v11.0.0    | Strategic deferrals made |

**Priority Deferrals Enabled by Context Clarity** (Session 41):

- W3.10 (Package Signing) → W5.10 (defer to late Wave 5)
- W3.8 (Observability) → Deferred indefinitely
- W5.2 (Container Deployment) → Removed (consumer concern)
- W2.33 (NuGet Publish) → W5.99 (absolute last task)
