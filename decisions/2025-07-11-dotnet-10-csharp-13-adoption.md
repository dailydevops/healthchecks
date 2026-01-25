---
authors:
  - Martin Stühmer

applyTo:
  - "**/*.csproj"
  - "**/*.fsproj"
  - "**/*.vbproj"
  - "Directory.Build.props"
  - "Directory.Build.targets"
  - "global.json"
  - "**/*.cs"

created: 2025-07-11

lastModified: 2025-07-14

state: accepted

instructions: |
  Adopt .NET 10 as the target framework and C# 13 as the primary programming language version to leverage latest performance improvements, cloud-native capabilities, and modern language features.
  All projects must target net10.0 with C# 13 preview features enabled for enhanced developer productivity and future-proofing.
---

# Decision: .NET 10 and C# 13 Adoption

A decision to adopt .NET 10 as the target framework and C# 13 as the primary programming language version for the Spix Spreed platform, leveraging the latest performance improvements, language features, and cloud-native capabilities.

## Context

The Spix Spreed platform is a modern social media management platform that requires:

1. **High Performance**: The platform handles high-frequency social media interactions, content processing, and real-time analytics
2. **Cloud-Native Architecture**: Must support microservices, containerization, and distributed deployments across multiple cloud providers
3. **Modern Development Experience**: Development team needs access to the latest language features for improved productivity and code maintainability
4. **Long-Term Support**: The chosen .NET version must provide a stable foundation for the platform's multi-year development lifecycle
5. **Compatibility Requirements**: Must support .NET Aspire for cloud-native orchestration and modern tooling ecosystem

Key considerations for framework selection:

- Performance improvements in runtime and garbage collection
- Enhanced cloud-native features and container optimization
- Language improvements in C# 13 for better developer productivity
- Native Ahead-of-Time (AOT) compilation capabilities for faster startup times
- Enhanced observability and diagnostics features
- Compatibility with .NET Aspire and modern cloud-native patterns

## Decision

We have decided to adopt **.NET 10** as the target framework and **C# 13** as the primary programming language version for the Spix Spreed platform. This includes:

### Framework Configuration

- **Target Framework**: `net10.0` for all projects within the solution
- **Language Version**: C# 13 (preview) to access the latest language features
- **Runtime**: .NET 10 runtime with optimizations for cloud-native workloads

### Implementation Details

- Configure `Directory.Build.props` with `<TargetFrameworks>net10.0</TargetFrameworks>`
- Set `<LangVersion>preview</LangVersion>` to enable C# 13 features during development
- Maintain compatibility with .NET Aspire orchestration requirements
- Utilize GitVersion workaround for `net9.0` target framework compatibility until official support

### Key Features to Leverage

- **Performance Improvements**: Utilize enhanced garbage collection and runtime optimizations
- **Native AOT**: Implement for performance-critical services where applicable
- **C# 13 Language Features**: Adopt new syntax improvements and performance enhancements
- **Cloud-Native Optimizations**: Leverage container-aware configurations and improved startup performance
- **Enhanced Observability**: Use built-in diagnostics and monitoring capabilities

## Consequences

### Positive Consequences

- **Performance Benefits**: Significant improvements in throughput, memory usage, and startup times
- **Developer Productivity**: Access to latest C# 13 language features improves code readability and maintainability
- **Future-Proofing**: Positions the platform for long-term compatibility with evolving .NET ecosystem
- **Cloud-Native Optimization**: Better container performance and resource utilization
- **Aspire Compatibility**: Full support for .NET Aspire cloud-native orchestration patterns
- **AOT Capabilities**: Option for Native Ahead-of-Time compilation for performance-critical scenarios

### Potential Risks

- **Preview Stability**: C# 13 preview features may have stability considerations during development
- **Ecosystem Compatibility**: Some third-party libraries may not immediately support .NET 10
- **Migration Complexity**: Future upgrades from preview to stable versions may require code adjustments
- **Tooling Maturity**: Development tools and IDE support may be evolving for latest features

### Mitigation Strategies

- Monitor C# 13 language feature stability and adjust usage based on production readiness
- Maintain compatibility testing with critical third-party dependencies
- Implement comprehensive testing strategies to validate preview feature usage
- Plan for migration path from preview to stable C# 13 release

## Alternatives Considered

### .NET 8 LTS with C# 12

**Pros:**

- Long-term support guarantee until November 2026
- Proven stability and extensive ecosystem compatibility
- Full production readiness with comprehensive tooling support

**Cons:**

- Missing latest performance improvements available in .NET 10
- Limited access to newest language features that improve developer productivity
- Less optimal cloud-native and container performance
- Potential need for earlier migration to newer versions

**Decision:** Rejected due to performance requirements and desire for latest cloud-native optimizations

### .NET 9 with C# 12

**Pros:**

- Standard Term Support (STS) with 18-month support lifecycle
- Good balance of stability and modern features
- Broad ecosystem compatibility

**Cons:**

- Shorter support lifecycle compared to LTS versions
- Missing .NET 10 performance improvements
- Limited future-proofing for long-term platform development
- Less optimal for high-performance social media processing requirements

**Decision:** Rejected in favor of latest performance and feature benefits

## Related Decisions

- [.NET Aspire for Cloud-Native Orchestration](./2025-07-11-dotnet-aspire-cloud-native-orchestration.md) - This decision depends on .NET 10 compatibility for optimal Aspire integration
- [Centralized Package Version Management](./2025-07-10-centralized-package-version-management.md) - Framework version impacts package compatibility and version management strategies
- [GitVersion Automated Semantic Versioning](./2025-07-10-gitversion-automated-semantic-versioning.md) - Requires compatibility workaround for .NET 10 target framework
