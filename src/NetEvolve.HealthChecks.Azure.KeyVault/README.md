# NetEvolve.HealthChecks.Azure.KeyVault

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.HealthChecks.Azure.KeyVault?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.KeyVault/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.HealthChecks.Azure.KeyVault?logo=nuget)](https://www.nuget.org/packages/NetEvolve.HealthChecks.Azure.KeyVault/)

This package provides a health check for Azure Key Vault, based on the [Azure.Security.KeyVault.Secrets](https://www.nuget.org/packages/Azure.Security.KeyVault.Secrets/) package. The main purpose is to check that the Azure Key Vault secret store is reachable and that the client can connect to it.

:bulb: This package is available for .NET 8.0 and later.

## Installation
To use this package, you need to add the package to your project. You can do this by using the NuGet package manager or by using the dotnet CLI.
```powershell
dotnet add package NetEvolve.HealthChecks.Azure.KeyVault
```

## Health Check - Azure Key Vault Secret Availability
The health check is a liveness check. It will check that the Azure Key Vault secret store is reachable and that the client can connect to it. If the service needs longer than the configured timeout to respond, the health check will return `Degraded`. If the service is not reachable, the health check will return `Unhealthy`.

### Usage
After adding the package, you need to import the namespace `NetEvolve.HealthChecks.Azure.KeyVault` and add the health check to the service collection.
```csharp
using NetEvolve.HealthChecks.Azure.KeyVault;
```
Therefore, you can use two different approaches. In both approaches you have to provide a name for the health check.

### Parameters
- `name`: The name of the health check. The name is used to identify the configuration object. It is required and must be unique within the application.
- `options`: The configuration options for the health check. If you don't provide any options, the health check will use the configuration based approach.
- `tags`: The tags for the health check. The tags `security`, `azure` and `keyvault` are always used as default and combined with the user input. You can provide additional tags to group or filter the health checks.

### Variant 1: Configuration based
The first one is to use the configuration based approach. Therefore, you have to add the configuration section `HealthChecks:AzureKeyVaultSecret` to your `appsettings.json` file.
```csharp
var builder = services.AddHealthChecks();

builder.AddKeyVaultSecretAvailability("<name>");
```

The configuration looks like this:
```json
{
  ..., // other configuration
  "HealthChecks": {
    "AzureKeyVaultSecret": {
      "<name>": {
        "VaultUri": "https://your-keyvault.vault.azure.net/", // required for non-ServiceProvider modes
        "Mode": "DefaultAzureCredentials", // optional, default is ServiceProvider
        "TenantId": "<tenant-id>", // optional, required when Mode is ClientSecretCredential
        "ClientId": "<client-id>", // optional, required when Mode is ClientSecretCredential
        "ClientSecret": "<client-secret>", // optional, required when Mode is ClientSecretCredential
        "Timeout": "<timeout>" // optional, default is 100 milliseconds
      }
    }
  }
}
```

### Variant 2: Options based
The second one is to use the options based approach. Therefore, you have to create an instance of `KeyVaultSecretAvailableOptions` and provide the configuration.
```csharp
var builder = services.AddHealthChecks();

builder.AddKeyVaultSecretAvailability("<name>", options =>
{
    options.VaultUri = new Uri("https://your-keyvault.vault.azure.net/");
    options.Mode = KeyVaultClientCreationMode.DefaultAzureCredentials;
    ...
    options.Timeout = "<timeout>";
});
```

## Configuration

### Client Creation Modes

The package supports three different modes to create the `SecretClient`:

- **ServiceProvider** (default): Retrieves the client from the service provider. This requires registering `SecretClient` in the dependency injection container.
- **DefaultAzureCredentials**: Creates a client using Azure Default Credentials. This is recommended for production environments and supports managed identities.
- **ClientSecretCredential**: Creates a client using a client secret. This is useful for service-to-service authentication scenarios.

### Mode: ServiceProvider

When using the `ServiceProvider` mode, you need to register the `SecretClient` in the service collection:

```csharp
// Register SecretClient
services.AddSingleton(sp =>
    new SecretClient(
        new Uri("https://your-keyvault.vault.azure.net/"),
        new DefaultAzureCredential()));

// Add health check
services
    .AddHealthChecks()
    .AddKeyVaultSecretAvailability(
        "KeyVaultSecret",
        options =>
        {
            options.Mode = KeyVaultClientCreationMode.ServiceProvider;
        });
```

You can also use keyed services for multiple Key Vault clients:

```csharp
// Register keyed SecretClient
services.AddKeyedSingleton("MyKeyVaultClient", (sp, key) =>
    new SecretClient(
        new Uri("https://your-keyvault.vault.azure.net/"),
        new DefaultAzureCredential()));

// Add health check with keyed service
services
    .AddHealthChecks()
    .AddKeyVaultSecretAvailability(
        "KeyVaultSecret",
        options =>
        {
            options.Mode = KeyVaultClientCreationMode.ServiceProvider;
            options.KeyedService = "MyKeyVaultClient";
        });
```

### Mode: DefaultAzureCredentials

When using the `DefaultAzureCredentials` mode, the health check will create a `SecretClient` using the `DefaultAzureCredential`:

```csharp
services
    .AddHealthChecks()
    .AddKeyVaultSecretAvailability(
        "KeyVaultSecret",
        options =>
        {
            options.VaultUri = new Uri("https://your-keyvault.vault.azure.net/");
            options.Mode = KeyVaultClientCreationMode.DefaultAzureCredentials;
        });
```

### Mode: ClientSecretCredential

When using the `ClientSecretCredential` mode, the health check will create a `SecretClient` using the provided client credentials:

```csharp
services
    .AddHealthChecks()
    .AddKeyVaultSecretAvailability(
        "KeyVaultSecret",
        options =>
        {
            options.VaultUri = new Uri("https://your-keyvault.vault.azure.net/");
            options.TenantId = "your-tenant-id";
            options.ClientId = "your-client-id";
            options.ClientSecret = "your-client-secret";
            options.Mode = KeyVaultClientCreationMode.ClientSecretCredential;
        });
```

### Advanced Configuration

You can configure the `SecretClientOptions` by providing a configuration action:

```csharp
services
    .AddHealthChecks()
    .AddKeyVaultSecretAvailability(
        "KeyVaultSecret",
        options =>
        {
            options.VaultUri = new Uri("https://your-keyvault.vault.azure.net/");
            options.Mode = KeyVaultClientCreationMode.DefaultAzureCredentials;
            options.ConfigureClientOptions = clientOptions =>
            {
                clientOptions.Retry.MaxRetries = 3;
                clientOptions.Retry.Delay = TimeSpan.FromSeconds(1);
            };
        });
```

### :bulb: You can always provide tags to all health checks, for grouping or filtering.

```csharp
var builder = services.AddHealthChecks();

builder.AddKeyVaultSecretAvailability("<name>", options => ..., "vault");
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/dailydevops/healthchecks/refs/heads/main/LICENSE) file for details.
