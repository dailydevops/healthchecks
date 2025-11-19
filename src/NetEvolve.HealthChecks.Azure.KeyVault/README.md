# NetEvolve.HealthChecks.Azure.KeyVault

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.KeyVault?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.KeyVault/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.KeyVault?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.KeyVault/)

This package provides a health check for Azure Key Vault, based on the [Azure.Security.KeyVault.Secrets](https://www.nuget.org/packages/Azure.Security.KeyVault.Secrets/) package. The main purpose is to check that the Azure Key Vault is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Azure.KeyVault
```

## Health Check - Azure Key Vault Secrets Availability
The health check is a liveness check. It will check that the Azure Key Vault is reachable and that the client can connect to it. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Azure.KeyVault` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.KeyVault;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `azure`, `keyvault` and `secrets` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureKeyVault` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddKeyVaultSecretsAvailability("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureKeyVault": {
      "<name>": {
        "VaultUri": "<vault-uri>", // required
        ..., // other configuration
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `KeyVaultSecretsAvailableOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddKeyVaultSecretsAvailability("<name>", options =>
{
    options.VaultUri = new Uri("<vault-uri>");
    ...
    options.Timeout = "<timeout>";
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddKeyVaultSecretsAvailability("<name>", options => ..., "azure");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
