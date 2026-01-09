<!-- List of authors who contributed to this decision. Include full names and roles if applicable. -->
authors:
- Martin Stühmer

<!--
The patterns this decision applies to. Each entry is a glob pattern that matches files affected by this decision.
-->
applyTo:
- "**/README.md"
- "src/**/README.md"

<!-- The date this ADR was initially created in YYYY-MM-DD format. -->
created: 2026-01-09

<!--
The most recent date this ADR was updated in YYYY-MM-DD format.
IMPORTANT: Update this field whenever the decision is modified.
-->
lastModified: 2026-01-09

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
  Use templates/readme-project.md as the mandatory template for all NuGet package README files to ensure consistent documentation structure, comprehensive content, and absolute URLs for cross-platform compatibility.
  All project READMEs MUST include features, installation instructions, usage examples, requirements, and support information following the template structure.
  MUST preserve all automated content markers (e.g., <!-- packages:start -->) under all circumstances when updating existing README files.
---
<!-- REQUIRED: Filename MUST follow the format: YYYY-MM-DD-Title (replace all spaces with hyphens) -->
# NuGet Package README Template

Establish a standardized template for NuGet package README files to ensure consistent, comprehensive, and professional documentation across all packages in the project ecosystem.

## Context

NuGet packages require clear and comprehensive documentation to help users understand the package functionality, installation process, and usage patterns. Without a standardized template, several issues arise:

1. **Inconsistent Documentation**: Different packages have varying documentation quality and structure, making it difficult for users to find information consistently across packages
2. **Missing Critical Information**: Ad-hoc README files often omit essential sections such as requirements, configuration, or troubleshooting guidance
3. **URL Management**: Relative URLs break when viewed on NuGet.org, GitHub, or other platforms that display README content in different contexts
4. **Maintenance Overhead**: Without a template, maintaining documentation standards requires manual review and correction of each README file
5. **Onboarding Complexity**: New contributors and package authors must determine documentation structure and content independently, leading to variations and potential gaps
6. **Professional Appearance**: Inconsistent documentation reduces the perceived quality and professionalism of packages
7. **Discoverability**: Well-structured documentation with standard sections improves package discoverability and user adoption

The need for a standardized approach became apparent as the project ecosystem expanded with multiple NuGet packages requiring consistent documentation standards.

## Decision

All NuGet package README files MUST use the standardized template located at `templates/readme-project.md` as the foundation for project documentation.

The template provides a comprehensive structure including:

* **Package Metadata**: Badges for version, downloads, and license status
* **Project Description**: Clear explanation of package purpose and functionality
* **Features**: Bullet-point list of key capabilities
* **Installation Instructions**: Multiple installation methods (NuGet Package Manager, .NET CLI, PackageReference)
* **Quick Start**: Minimal working example for immediate usage
* **Usage Examples**: Detailed examples covering basic and advanced scenarios
* **Configuration**: Setup and configuration guidance
* **Requirements**: Minimum framework versions and dependencies
* **Related Packages**: Cross-references to related packages in the ecosystem
* **Documentation Links**: References to comprehensive documentation
* **Contributing Guidelines**: Links to contribution processes
* **Support Channels**: GitHub Issues, and documentation links
* **License Information**: Clear license declaration and reference

All URLs in package READMEs MUST be absolute URLs pointing to GitHub or NuGet.org to ensure compatibility across all platforms where the README may be displayed.

The template includes comprehensive comments explaining each section's purpose and providing guidance for content creation.

### Preservation of Automated Content Markers

When updating existing README files, all automated content markers MUST be preserved under all circumstances. These markers are HTML comments that define sections managed by automated tools or processes.

Common marker patterns include:
* `<!-- packages:start -->` and `<!-- packages:end -->` - Automated package list sections
* `<!-- section:start -->` and `<!-- section:end -->` - Generic automated sections
* Any other `<!-- marker:start -->` and `<!-- marker:end -->` pairs

These markers and their content MUST NOT be modified or removed during manual template updates. Automated tools rely on these markers to maintain dynamic content sections.

## Consequences

### Positive

* **Consistency**: All package READMEs follow the same structure, making information easy to find
* **Completeness**: Template ensures all critical sections are included and properly documented
* **Professional Quality**: Standardized documentation improves the perceived quality and professionalism of all packages
* **Reduced Maintenance**: Template updates automatically improve documentation guidance for all packages
* **Cross-Platform Compatibility**: Absolute URLs ensure README content displays correctly on NuGet.org, GitHub, and other platforms
* **Faster Onboarding**: New package authors have clear guidance for creating documentation
* **Better Discoverability**: Consistent structure with badges and clear descriptions improves package discoverability
* **User Experience**: Users benefit from predictable documentation structure across all packages
* **Quality Assurance**: Template comments guide authors toward complete and accurate documentation
* **Automated Content Protection**: Marker preservation ensures automated tooling continues to function correctly

### Negative

* **Initial Effort**: Existing packages require updates to conform to the new template structure
* **Flexibility Constraints**: Specialized packages may need to deviate from the standard template structure
* **Maintenance Obligation**: Template changes require updating existing package READMEs to maintain consistency
* **Learning Curve**: Authors must familiarize themselves with the template structure and guidelines
* **Marker Awareness**: Authors must be aware of automated content markers and avoid modifying them manually

## Alternatives Considered

### Manual Documentation Without Template

Each package author creates README files independently without a standardized template.

**Rejected because**:
* Leads to inconsistent documentation quality and structure
* Critical information often omitted or poorly organized
* Increases maintenance burden for ensuring documentation standards
* Reduces professional appearance of package ecosystem
* Makes it difficult for users to find information consistently

### Minimal Template

Provide a minimal template with only basic sections (title, description, installation).

**Rejected because**:
* Insufficient guidance for comprehensive documentation
* Important sections like configuration, requirements, and support would be frequently omitted
* Does not address cross-platform URL compatibility issues
* Provides less value in standardizing documentation quality

### Multiple Templates by Package Type

Create different templates for different types of packages (libraries, tools, extensions).

**Rejected because**:
* Adds complexity in choosing the correct template
* Reduces consistency across the package ecosystem
* Most sections are relevant to all package types
* Authors can omit irrelevant sections from the standard template

### External Documentation Generation Tools

Use automated documentation generation tools (DocFX, Sandcastle) instead of manual README files.

**Rejected because**:
* NuGet.org displays README content prominently, requiring manually crafted content
* Generated documentation is often too technical or detailed for quick package evaluation
* Adds build complexity and tooling dependencies
* Does not replace the need for user-friendly introductory content

## Related Decisions

* [English as Project Language](./2025-07-11-english-as-project-language.md) - All README content must be written in English following this decision
* [Folder Structure and Naming Conventions](./2025-07-10-folder-structure-and-naming-conventions.md) - README files are placed at the root of each project directory following folder structure conventions
