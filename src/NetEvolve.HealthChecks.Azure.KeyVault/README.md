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

## Health Check - Azure Key Vault Availability
The health check is a liveness check. It will check that the Azure Key Vault is reachable and that the client can connect to it. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.KeyVault;
```

Depending on the type of authentication you want to use, you have to add the health check accordingly.

### Parameters
- `name`: The name of the health check.
- `options`: The configuration options for the health check. You can configure the following:
  - `VaultUri`: The URI of the Azure Key Vault. (Required when using `DefaultAzureCredentials` mode)
  - `Mode`: The mode to create the client. (Required, default is `ServiceProvider`)
  - `Timeout`: The timeout in milliseconds to use when connecting and executing tasks against the Key Vault. (Optional, default is 100 milliseconds)
  - `ConfigureClientOptions`: The lambda to configure the `SecretClientOptions`. (Optional)
- `tags`: The tags to add to the health check. (Optional)

Supported client creation modes:
- `ServiceProvider`: Uses a `SecretClient` that is already registered in the service provider.
- `DefaultAzureCredentials`: Uses the `DefaultAzureCredential` to authenticate with Azure Key Vault.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureKeyVault` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureKeyVault("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureKeyVault": {
      "<name>": {
        "VaultUri": "<vault-uri>", // required
        "Mode": "<mode>", // required, to specify the client creation mode, either `ServiceProvider` or `DefaultAzureCredentials`
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `KeyVaultOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddAzureKeyVault("<name>", options =>
{
    options.VaultUri = new Uri("<vault-uri>");
    options.Mode = KeyVaultClientCreationMode.DefaultAzureCredentials;
    options.Timeout = 100; // optional, default is 100 milliseconds
});
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddAzureKeyVault("<name>", options => ..., "azure");
```