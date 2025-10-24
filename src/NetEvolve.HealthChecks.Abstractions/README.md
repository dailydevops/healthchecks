# NetEvolve.HealthChecks.Abstractions

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Abstractions?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Abstractions/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Abstractions?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Abstractions/)

This package provides a set of abstractions for creating health checks in .NET applications. It is based on the [Microsoft.Extensions.Diagnostics.HealthChecks](https://www.nuget.org/packages/Microsoft.Extensions.Diagnostics.HealthChecks/) package and used for creating the NetEvolve.HealthChecks libraries.

:bulb: This package is available for .NET 8.0 and later.

## Prerequisites

- .NET 8.0 or later

> **Note:** This package provides abstractions and base types for health checks. It is typically used as a dependency by other health check packages and not directly by application code.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Abstractions
```