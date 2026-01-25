---
authors:
  - Martin Stühmer

applyTo:
  - "**/*"

created: 2025-11-15

lastModified: 2025-11-15

state: accepted

instructions: |
  GitHub Discussions is not used for team communication. Issues serve as the central point of contact for all project-related discussions and coordination.
  This decision reduces tool fragmentation and ensures all communication flows through a single, tracked channel.
---

# Decision: No GitHub Discussions for Communication

We do not use GitHub Discussions as a communication channel for this project. All team communication related to project decisions, issues, and coordination occurs through GitHub Issues.

## Context

The project needed to establish clear guidelines for where team communication should occur. GitHub provides multiple communication mechanisms including Discussions, Issues, and Pull Request comments. Without clear boundaries, communication can become fragmented across multiple channels, making it difficult to track decisions and maintain context.

## Decision

GitHub Discussions will not be used for project communication. GitHub Issues will serve as the central point of contact for all project-related discussions and coordination.

## Consequences

**Positive:**

- Single source of truth for all project communication
- Improved traceability and searchability of decisions
- Reduced cognitive overhead from having fewer communication channels
- Better integration with workflow automation tied to Issues

**Negative:**

- Requires discipline to redirect team members away from Discussions
- May require additional effort to maintain Issues as the communication hub
- Issues-only approach requires clear issue templates and categorization to prevent clutter

## Alternatives Considered

**Use GitHub Discussions as the primary communication channel:**

- Rejected because Issues are already established as the central contact point
- Would create fragmentation between Issues and Discussions, making it harder to track decisions

**Use external communication tools (Slack, Teams, etc.) for discussions:**

- Rejected as these tools are not being considered for this project
- Communication must remain within the GitHub ecosystem to maintain traceability

**Use both Issues and Discussions for different purposes:**

- Rejected because maintaining two communication channels adds unnecessary complexity
- Issues provide sufficient functionality for all communication needs

## Related Decisions

None at this time.
