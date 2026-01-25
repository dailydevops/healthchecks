---
agent: 'agent'
tools: ['vscode', 'execute', 'read', 'edit', 'search', 'web', 'deepwiki/*', 'fetch/*', 'microsoft-docs/*', 'nuget-server/*', 'agent', 'todo']
description: 'Ensure that C# types are documented with XML comments following Microsoft documentation standards and project guidelines.'
---

# C# Documentation Best Practices

## General Principles

* MUST document all public members with XML comments to ensure comprehensive API documentation.
* SHOULD document internal members, especially those that are complex or implement non-obvious behavior.
* MUST use English exclusively for all documentation comments (per project language standard).
* MUST follow the C# 13 and modern language conventions when creating code examples within documentation.
* MUST ensure documentation comments are concise, clear, and professional in tone.

## XML Documentation Tags

### Core Documentation Tags

* MUST use `<summary>` to provide a brief, one-line description of what the type or member does. Start the summary with a present-tense, third-person verb.
* MUST use `<remarks>` for additional information, which can include implementation details, usage notes, platform-specific behavior, or other relevant context.
* MUST use `<see langword>` for language-specific keywords like `null`, `true`, `false`, `int`, `bool`, and other C# language keywords.
* MUST use `<c>` for inline code snippets and identifiers within text.
* SHOULD use `<example>` to provide usage examples demonstrating how to use the member.
  * MUST place `<code>` blocks within `<example>` tags and add the `language="csharp"` attribute to enable proper syntax highlighting.
  * SHOULD include brief explanatory comments in code examples to clarify context or intent.
* MUST use `<see cref>` to reference other types or members inline (within sentences) for cross-referencing.
* MUST use `<seealso>` for standalone references to related types or members in the "See also" section of generated documentation.
* MUST prefer `<see cref>`/`<seealso cref>` for internal references; MUST use `<see href="https://...">` or `<seealso href="https://...">` only for external URLs.
  * MUST use `https` URLs and include concise link text; MUST NOT emit raw `<a href>` tags in XML documentation.
* SHOULD use `<inheritdoc />` to inherit documentation from base classes or interfaces when the behavior is identical.
  * MUST document behavior differences explicitly if significant changes exist from the base implementation.

## Type Documentation

* MUST document the purpose and responsibility of each public class, struct, interface, delegate, and enum.
* MUST include usage examples for complex types that require specific initialization patterns.
* SHOULD mention any platform-specific requirements or constraints.
* SHOULD reference related types using `<seealso>` tags.

## Method Documentation

* MUST use `<param>` to describe each method parameter:
  * MUST phrase parameter descriptions as noun phrases without specifying the data type.
  * MUST begin descriptions with an appropriate introductory article (a, an, the, or none for countable items).
  * For flag enum parameters, MUST start with: "A bitwise combination of the enumeration values that specifies..."
  * For non-flag enum parameters, MUST start with: "One of the enumeration values that specifies..."
  * For Boolean parameters, MUST use the form: "`<see langword="true" />` to ...; otherwise, `<see langword="false" />`."
  * For "out" parameters, MUST use the form: "When this method returns, contains .... This parameter is treated as uninitialized."
* MUST use `<paramref>` to reference parameter names when mentioned in documentation text.
* MUST use `<typeparam>` to describe type parameters in generic types and generic methods.
* MUST use `<typeparamref>` to reference type parameters when mentioned in documentation text.
* MUST use `<returns>` to describe the return value:
  * MUST phrase return descriptions as noun phrases without specifying the data type.
  * MUST begin descriptions with an appropriate introductory article.
  * For Boolean return types, MUST use the form: "`<see langword="true" />` if ...; otherwise, `<see langword="false" />`."
  * For null return values, MUST clarify when `null` can be returned and under what conditions.
* SHOULD use `<exception cref>` to document exceptions thrown directly by the method.
  * MUST only document exceptions that callers can reasonably handle.
  * MUST omit "Thrown if..." or "If..." prefixes; state the condition directly, for example: "An error occurred when accessing a required resource."

## Constructor Documentation

* MUST use the summary format: "Initializes a new instance of the `<ClassName>` class" or "struct" as appropriate.
* SHOULD include information about initial state, default values, or invariants established during construction.
* MUST document all constructor parameters using `<param>` tags.
* SHOULD document constructor exceptions if applicable.

## Property Documentation

* MUST use `<summary>` starting with appropriate verb phrases based on property access:
  * "Gets or sets..." for read-write properties.
  * "Gets..." for read-only properties.
  * "Gets or sets a value that indicates whether..." for Boolean properties.
* MUST use `<value>` to describe the property's value:
  * MUST phrase value descriptions as noun phrases without specifying the data type.
  * If the property has a default value, MUST include it in a separate sentence: "The default is `<see langword="false" />`."
  * For Boolean properties, MUST use the form: "`<see langword="true" />` if ...; otherwise, `<see langword="false" />`. The default is ..."
* SHOULD document property exceptions if the getter or setter can throw exceptions.

## Field Documentation

* MUST document public fields with `<summary>` and `<remarks>` tags where appropriate.
* SHOULD document constant fields to clarify their purpose and usage.
* MUST use `<value>` to describe the field's content.

## Event Documentation

* MUST use `<summary>` to describe when the event is raised.
* MUST use `<remarks>` to provide context about the event's behavior and typical usage patterns.
* SHOULD document the event arguments using `<param>` tags if using custom EventArgs.

## Exception Documentation

* MUST use `<exception cref>` to document exceptions thrown by constructors, properties, indexers, methods, operators, and events.
* MUST document all exceptions thrown directly by the member that callers can meaningfully handle.
* For exceptions thrown by nested or indirect calls, SHOULD only document the exceptions users are most likely to encounter.
* MUST phrase exception descriptions to describe the condition that triggers the exception:
  * Omit conditional prefixes; state the condition directly.
  * Example: "A required parameter is `null`." instead of "Thrown if a required parameter is null."

## Generic Types and Methods

* MUST use `<typeparam>` to document type parameters in generic types and methods:
  * MUST describe the constraints and purpose of each type parameter.
  * SHOULD clarify any performance or memory implications of type parameter choices.
* MUST use `<typeparamref>` to reference type parameters in other documentation text.

## Code Examples in Documentation

* MUST use valid, compilable C# 13 code in examples.
* SHOULD follow project coding conventions in examples (including null-checking with `is null`, modern patterns, switch expressions, etc.).
* SHOULD include complete, runnable examples rather than fragments when the example is non-trivial.
* MUST include explanatory comments for complex logic, but MUST NOT use "Act", "Arrange", or "Assert" labels in documentation examples.
* SHOULD demonstrate both common and edge-case usage patterns.

## Special Considerations

* MUST apply nullable reference type annotations (`?`) to all public API documentation.
* MUST use `NotNullAttribute` and related nullable attributes in public API documentation when appropriate.
* MUST use `nameof()` operator in examples instead of string literals when referring to member names.
* MUST ensure that documentation examples properly demonstrate null-checking patterns using `is null` and `is not null`.
* SHOULD reference related decisions or architectural guidelines when relevant to the API's design or usage.

## Accessibility and Localization

* MUST ensure all documentation is written in clear, professional English suitable for a global audience.
* SHOULD avoid idioms, colloquialisms, and culturally specific references.
* MUST use simple, direct language while maintaining technical precision.
* SHOULD define technical terms on first usage or reference external documentation.
