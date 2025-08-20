<!-- List of authors who contributed to this decision. Include full names and roles if applicable. -->
authors:
- Martin Stühmer

<!--
The patterns this decision applies to. Each entry is a glob pattern that matches files affected by this decision.
-->
applyTo:
- "decisions/**/*"
- "decisions/assets/**/*"

<!-- The date this ADR was initially created in YYYY-MM-DD format. -->
created: 2025-07-10

<!--
The most recent date this ADR was updated in YYYY-MM-DD format.
IMPORTANT: Update this field whenever the decision is modified.
-->
lastModified: 2025-07-14

<!--
The current state of this ADR. If superseded, include references to the superseding ADR.
Valid values: proposed, accepted, deprecated, superseded
-->
state: accepted

<!--
A compact AI LLM compatible definition of this decision.
This should be a precise, structured description that AI systems can easily parse and understand.
Include the core decision, key rationale, and primary impact in 1-2 concise sentences.
-->
instructions: |
  Store all decision-related assets in decisions/assets/ folder and link them using relative paths to ensure proper organization and version control.
  Use format [Description](assets/filename.extension) with no external hosting permitted for decision assets.
---
# Decision: Decision Assets Storage and Linking

This decision establishes the standardized approach for storing and referencing assets related to architectural decisions within the project's decision documentation system.

## Context

As the project evolves, architectural decisions often require supporting materials such as diagrams, flowcharts, images, PDFs, spreadsheets, and other documentation artifacts. These assets need to be properly organized, version-controlled, and easily accessible from within decision documents. Without a standardized approach, assets may become scattered across different locations, making them difficult to find and maintain over time.

The current project structure includes a `decisions/` folder for storing architectural decision records (ADRs), and there's a need to establish where and how to store related assets while ensuring they remain linked and accessible.

## Decision

All decision-related assets MUST be stored in the `decisions/assets/` folder and MUST be directly linked within decision documents using relative paths.

### Storage Requirements:
- All assets supporting architectural decisions (diagrams, images, PDFs, etc.) MUST be placed in `decisions/assets/`
- Asset filenames SHOULD use their original names without requiring any specific naming convention
- Assets MAY be organized into subdirectories within `decisions/assets/` if needed for better organization

### Linking Requirements:
- All assets MUST be referenced using relative paths from the decision document
- Links MUST use the format: `[Asset Description](assets/filename.extension)` for files in the root assets folder
- For assets in subdirectories, use: `[Asset Description](assets/subdirectory/filename.extension)`
- Assets MUST be directly embedded or linked within the relevant decision document
- No external hosting or cloud storage links are permitted for decision assets

### Example Structure:
```
decisions/
├── 2025-07-10-decision-assets.md
├── 2025-07-10-database-architecture.md
└── assets/
    ├── er-diagram.png
    ├── performance-analysis.pdf
```

### Example Linking:
```markdown
The proposed database schema is illustrated in the following diagram:

![Database ER Diagram](assets/er-diagram.png)

For detailed performance analysis, refer to the [Performance Analysis Report](assets/performance-analysis.pdf).
```

## Consequences

### Positive Consequences:
- **Centralized Asset Management**: All decision-related assets are stored in a single, predictable location
- **Version Control**: Assets are version-controlled alongside the decision documents
- **Accessibility**: Assets remain accessible even when working offline or in different environments
- **Maintenance**: Easier to maintain and update assets as they're co-located with decisions
- **Documentation Integrity**: Direct linking ensures assets and decisions remain connected

### Potential Risks:
- **Repository Size**: Large assets may increase repository size, though this is manageable with proper asset optimization
- **Binary File Management**: Git may not handle large binary files efficiently, but this can be mitigated with Git LFS if needed
- **Asset Duplication**: Same assets might be referenced by multiple decisions, though this ensures each decision remains self-contained

## Alternatives Considered

### External Asset Hosting

Using cloud storage or external hosting services for assets was considered but rejected because:

- Creates external dependencies that may become unavailable
- Requires additional access management and permissions
- Breaks the self-contained nature of the decision repository
- May introduce security and compliance concerns

### Separate Asset Repository

Creating a separate repository for assets was considered but rejected because:

- Increases complexity in maintaining relationships between decisions and assets
- Makes it harder to ensure assets remain synchronized with decisions
- Complicates the review process for decision changes that include asset updates

### Flat Asset Structure

Storing all assets directly in the `decisions/` folder was considered but rejected because:

- Creates clutter in the main decisions directory
- Makes it harder to distinguish between decision documents and supporting assets
- Reduces organization and discoverability of assets

## Related Decisions

None at this time. This decision is independent and focuses solely on asset storage and organization practices.
