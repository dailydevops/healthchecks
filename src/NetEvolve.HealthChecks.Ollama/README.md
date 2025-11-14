# NetEvolve.HealthChecks.Ollama

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Ollama?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Ollama/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Ollama?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Ollama/)

This package provides a health check for Ollama, based on the [OllamaSharp](https://www.nuget.org/packages/OllamaSharp/) package.
The main purpose is to check that the Ollama instance is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Ollama
```

## Health Check - Ollama Liveness
The health check is a liveness check. It will check that the Ollama instance is reachable and that the client can connect to it.
If the instance needs longer than the configured timeout to respond, the health check will return `Degraded`.
If the instance is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Ollama` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Ollama;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `ollama`, `ai`, and `llm` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:Ollama` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddOllama("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "Ollama": {
      "<name>": {
        "Uri": "<ollama uri>", // required, e.g., "http://localhost:11434"
        "Timeout": "<timeout>" // optional, default is 5000 milliseconds
      }
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one Ollama instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddOllama("<name>", options =>
{
    options.Uri = "<ollama uri>"; // required, e.g., "http://localhost:11434"
    options.Timeout = "<timeout>"; // optional, default is 5000 milliseconds
    ... // other configuration
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddOllama("<name>", options => ..., "ollama");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
