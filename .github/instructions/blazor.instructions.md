---
applyTo: '**/*.razor, **/*.razor.cs, **/*.razor.css'

description: This file contains instructions for Blazor development. It includes guidelines for component development, performance optimization, and following Blazor coding standards. Ensure to follow the practices outlined in this file to maintain code quality and consistency.
---

# Blazor Development Instructions

## General

* MUST write idiomatic and efficient Blazor and C# code.
* MUST follow .NET and Blazor conventions and best practices.
* MUST use Razor Components appropriately for component-based UI development.
* MUST use the latest version C#, currently C# 13 features like record types, pattern matching, and global usings (see [.NET 10 and C# 13 Adoption](../../decisions/2025-07-11-dotnet-10-csharp-13-adoption.md)).
* MUST use async/await where applicable to ensure non-blocking UI operations.

## Code Style and Structure

* MUST prefer inline functions for smaller components but separate complex logic into code-behind or service classes.
* MUST structure Blazor components and services following Separation of Concerns principles.
* MUST follow PascalCase for component names, method names, and public members.
* MUST use camelCase for private fields and local variables.
* MUST prefix interface names with "I" (e.g., `IUserService`).

## Component Lifecycle and Data Binding

* MUST utilize Blazor's built-in features for component lifecycle (e.g., `OnInitializedAsync`, `OnParametersSetAsync`).
* MUST use data binding effectively with `@bind` directive.
* MUST leverage Dependency Injection for services in Blazor components.
* MUST use `EventCallbacks` for handling user interactions efficiently, passing only minimal data when triggering events.

## Performance Optimization

* MUST optimize Razor components by reducing unnecessary renders and using `StateHasChanged()` efficiently.
* MUST minimize the component render tree by avoiding re-renders unless necessary, using `ShouldRender()` where appropriate.
* MUST use asynchronous methods (`async`/`await`) for API calls or UI actions that could block the main thread.
* MUST utilize Blazor server-side or WebAssembly optimally based on the project requirements.

## Error Handling and Validation

* MUST implement proper error handling for Blazor pages and API calls using try-catch blocks.
* MUST use logging for error tracking in the backend and consider capturing UI-level errors in Blazor with tools like `ErrorBoundary`.
* MUST implement validation using FluentValidation or DataAnnotations in forms.
* MUST provide proper user feedback in the UI for error conditions.

## Caching Strategies

* MUST implement in-memory caching for frequently used data, especially for Blazor Server apps using `IMemoryCache`.
* MUST utilize `localStorage` or `sessionStorage` to cache application state between user sessions for Blazor WebAssembly.
* MUST consider Distributed Cache strategies (like Redis or SQL Server Cache) for larger applications that need shared state.
* MUST cache API calls by storing responses to avoid redundant calls when data is unlikely to change.

## State Management

* MUST use Blazor's built-in Cascading Parameters and EventCallbacks for basic state sharing across components.
* MUST implement advanced state management solutions using libraries like Fluxor or BlazorState when the application grows in complexity.
* MUST use Blazored.LocalStorage or Blazored.SessionStorage for client-side state persistence in Blazor WebAssembly.
* MUST use Scoped Services and the StateContainer pattern for server-side Blazor to manage state within user sessions.

## API Integration

* MUST use `HttpClient` or other appropriate services to communicate with external APIs or backend services.
* MUST implement comprehensive error handling for API calls and provide meaningful user feedback.
* MUST use HTTPS for all web communication and ensure proper CORS policies are implemented.

## Security and Authentication

* MUST implement Authentication and Authorization in Blazor applications where necessary using ASP.NET Identity or JWT tokens.
* MUST ensure proper security measures are in place for API authentication and data protection.
* MUST validate all user inputs and implement proper authorization checks.

## Testing and Debugging

* MUST perform all unit testing and integration testing using Visual Studio Enterprise.
* MUST test Blazor components and services using TUnit (preferred) or xUnit/NUnit/MSTest.
* MUST use appropriate mocking frameworks for testing dependencies.
* MUST debug Blazor UI issues using browser developer tools and Visual Studio's debugging tools.
* MUST use Visual Studio's diagnostics tools for performance profiling and optimization.

## Documentation

* MUST use Swagger/OpenAPI for API documentation for backend API services.
* MUST ensure XML documentation for models and API methods to enhance Swagger documentation.
* MUST document complex component logic and business rules for maintainability.