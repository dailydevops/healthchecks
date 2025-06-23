# NetEvolve.HealthChecks.Keycloak

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Keycloak?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Keycloak/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Keycloak?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Keycloak/)

This package provides a health check for Keycloak services, based on the [Keycloak.Net.Core](https://www.nuget.org/packages/Keycloak.Net.Core/) package. The main purpose is to check if the server is available and the realms reachable.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Keycloak
```

## Health Check - Keycloak Liveness
The health check is a liveness check. It checks if the Keycloak Server is available and if the database is online.
If the query needs longer than the configured timeout, the health check will return `Degraded`.
If the query fails, for whatever reason, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace and add the health check to the health check builder.
```csharp
using NetEvolve.HealthChecks.Keycloak;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `keycloak` and `iam` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. This approach is recommended if you have multiple Keycloak instances to check.
```csharp
var builder = services.AddHealthChecks();

builder.AddKeycloak("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "Keycloak": {
      "<name>": {
        "Mode": "<client_creation_mode>", // Optional, defaults to 'KeycloakClientCreationMode.ServiceProvider'
        "KeyedService": "<key>", // Optional, used when Mode set to 'KeycloakClientCreationMode.ServiceProvider'
        "BaseAddress": "<base_address>", // Required when Mode set to 'KeycloakClientCreationMode.Internal'
        "Username": "<username>", // Required when Mode set to 'KeycloakClientCreationMode.Internal'
        "Password": "<password>", // Required when Mode set to 'KeycloakClientCreationMode.Internal'
        "Timeout": "<timeout>" // Optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Builder based
The second approach is to use the builder based approach. This approach is recommended if you only have one server instance to check or dynamic programmatic values.
```csharp
var builder = services.AddHealthChecks();

builder.AddKeycloak("<name>", options =>
{
    options.Mode = <client_creation_mode>; // Optional, defaults to 'KeycloakClientCreationMode.ServiceProvider'
    options.KeyedService = "<key>"; // Optional, used when Mode set to 'KeycloakClientCreationMode.ServiceProvider'
    options.BaseAddress = "<base_address>"; // Required when Mode set to 'KeycloakClientCreationMode.Internal'
    options.Username = "<username>"; // Required when Mode set to 'KeycloakClientCreationMode.Internal'
    options.Password = "<password>"; // Required when Mode set to 'KeycloakClientCreationMode.Internal'
    options.Timeout = <timeout>; // Optional, defaults to 100 milliseconds

    // Optional, defaults to NetEvolve.HealthChecks.Keycloak.DefaultCommandAsync
    options.CommandAsync = async (client, cancellationToken) => {
        // Your custom server pinging logic goes here.
        // Should return true if the command result is valid, false otherwise.
    };
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddKeycloak("<name>", options => ..., "Keycloak", "auth");
```