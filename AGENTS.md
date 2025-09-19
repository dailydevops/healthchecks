# Instructions

Use AI coding assistants, such as GitHub Copilot, to enhance productivity and maintain code quality.

## General

* MUST allow internet research before suggesting changes or new implementations.

## Technology Research

* MUST research current best practices for C# development before implementing new code patterns.
* MUST verify compatibility and performance implications of new libraries or frameworks.
* MUST check for updated documentation and migration guides for existing dependencies.

## Business Context Research

* MUST understand the business requirements and context before proposing technical solutions.
* MUST consider the impact on existing workflows and user experience.
* MUST evaluate the cost-benefit ratio of proposed changes or new implementations.

## Decision References

* MUST document all decisions in `decisions/` folder using `templates/adr.md` format.
* MUST treat "accepted" decisions as mandatory requirements with highest precedence.
* MUST respect decision states:
  - **accepted**: mandatory requirements
  - **proposed**: optional considerations
  - **deprecated**: avoid in new implementations
  - **superseded**: forbidden, follow superseding decision instead
* MUST use the `instructions` frontmatter property as primary AI guidance for each decision.

## Configuration Files

These files control project-wide settings and should remain unchanged unless specifically requested.

* MUST NEVER change `.editorconfig` unless explicitly asked to.
* MUST NEVER change `.gitignore` unless explicitly asked to.
* MUST NEVER change `global.json` unless explicitly asked to.
* MUST NEVER change `Directory.Build.props` unless explicitly asked to.
* MUST NEVER change `Directory.Build.targets` unless explicitly asked to.
* MUST NEVER change `Directory.Packages.props` unless explicitly asked to.

## Code Reviews and Implementation

* MUST always review AI-generated code for correctness, security, and performance.
* MUST provide constructive feedback and suggestions for improvement.
* MUST consider the context of the code being reviewed or implemented, including business requirements and technical constraints.
* MUST apply all relevant instructions and guidelines from `.github/instructions/*.instructions.md` files during both code review and implementation.