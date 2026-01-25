---
authors:
  - Martin Stühmer

applyTo:
  - "**/*.cs"
  - "**/*.razor"
  - "**/*.razor.cs"

created: 2026-01-21

lastModified: 2026-01-21

state: accepted

instructions: |
  MUST NOT use code regions (`#region`/`#endregion`) under any circumstances.
  Regions obscure code structure, hinder navigation, and indicate poor code organization that should be resolved through proper refactoring.
---

# Decision: No Code Regions

A decision to prohibit the use of code regions (`#region`/`#endregion`) in all C# and Razor source files to improve code readability, maintainability, and navigation.

## Context

Code regions in C# allow developers to group sections of code that can be collapsed in the IDE using `#region` and `#endregion` directives:

```csharp
#region Private Methods
private void DoSomething() { }
private void DoSomethingElse() { }
#endregion
```

While regions were historically used to organize code, modern development practices and tooling have evolved significantly:

1. **Code Smell Indicator**: Regions often indicate that a class or file has grown too large and should be refactored into smaller, more focused components.
2. **Navigation Obstruction**: Collapsed regions hide code, making it harder to navigate and understand the full structure of a file.
3. **False Organization**: Regions provide a false sense of organization without addressing the underlying structural issues.
4. **Modern IDE Capabilities**: Current IDEs provide superior navigation tools (outline views, symbol search, code folding based on syntax) that make regions unnecessary.
5. **Code Review Impact**: Regions can hide code during reviews, potentially allowing problematic code to go unnoticed.
6. **Partial Class Alternative**: When code organization is genuinely needed, partial classes provide a better solution with proper file separation.

## Decision

We have decided to **prohibit the use of code regions** (`#region`/`#endregion`) in all C# and Razor source files under all circumstances.

### Implementation Requirements

- MUST NOT use `#region` or `#endregion` directives in any C# or Razor file.
- MUST remove existing regions during code refactoring or maintenance.
- MUST refactor large files into smaller, focused classes or partial classes instead of using regions.
- MUST use proper class design and separation of concerns as organizational strategies.

### Enforcement

- Code reviewers MUST reject pull requests containing new region directives.
- Static analysis tools SHOULD be configured to flag region usage as a warning or error.
- Existing regions SHOULD be removed opportunistically during regular maintenance.

### Acceptable Alternatives

Instead of using regions, developers MUST apply these organizational strategies:

| Problem                              | Solution                                             |
| ------------------------------------ | ---------------------------------------------------- |
| File is too long                     | Split into multiple classes or partial classes       |
| Grouping related methods             | Use proper class design with single responsibility   |
| Separating interface implementations | Use partial classes in separate files                |
| Organizing generated code            | Place generated code in separate partial class files |

## Consequences

### Positive Consequences

- **Improved Readability**: All code is visible and accessible without expanding collapsed sections.
- **Better Code Structure**: Forces developers to address underlying organizational issues through proper refactoring.
- **Enhanced Navigation**: IDE navigation features work consistently without hidden code sections.
- **Clearer Code Reviews**: Reviewers see all code without needing to expand regions.
- **Consistent Codebase**: Uniform code organization approach across the entire project.

### Negative Consequences

- **Initial Refactoring Effort**: Existing code with regions requires refactoring to comply with this decision.
- **Learning Curve**: Developers accustomed to using regions must adopt alternative organizational patterns.
- **Potential Resistance**: Some developers may prefer regions for organizing large files.

### Trade-offs

- Short-term effort to remove existing regions versus long-term maintainability benefits.
- Requires discipline to maintain small, focused classes instead of relying on regions for organization.

## Alternatives Considered

### Allow Regions for Specific Use Cases

**Rejected** because:

- Creates ambiguity about when regions are acceptable
- Leads to inconsistent codebase
- Does not address the fundamental issues regions introduce

### Use EditorConfig to Disable Region Folding

**Rejected** because:

- Only affects IDE display, does not prevent region creation
- Does not enforce the underlying code quality principles
- Regions still clutter source files

### Allow Regions Only in Generated Code

**Rejected** because:

- Generated code should be in separate files or partial classes
- Maintaining exceptions complicates the rule
- Modern code generation tools support partial class patterns

## Related Decisions (Optional)

None at this time.
