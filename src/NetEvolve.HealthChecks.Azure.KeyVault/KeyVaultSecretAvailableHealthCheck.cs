namespace NetEvolve.HealthChecks.Azure.KeyVault;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using global::Azure.Core;
using global::Azure.Identity;
using global::Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(KeyVaultSecretAvailableOptions))]
internal sealed partial class KeyVaultSecretAvailableHealthCheck
{
    private ConcurrentDictionary<string, SecretClient>? _secretClients;

    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        KeyVaultSecretAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var secretClient = GetSecretClient(name, options, _serviceProvider);

        var (isTimelyResponse, result) = await secretClient
            .GetPropertiesOfSecretsAsync(cancellationToken: cancellationToken)
            .AsPages(pageSizeHint: 1)
            .GetAsyncEnumerator(cancellationToken)
            .MoveNextAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Failed to retrieve secrets from Key Vault.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    private SecretClient GetSecretClient(
        string name,
        KeyVaultSecretAvailableOptions options,
        IServiceProvider serviceProvider
    )
    {
        if (options.Mode == KeyVaultClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<SecretClient>()
                : serviceProvider.GetRequiredKeyedService<SecretClient>(options.KeyedService);
        }

        _secretClients ??= new ConcurrentDictionary<string, SecretClient>(StringComparer.OrdinalIgnoreCase);

        return _secretClients.GetOrAdd(name, _ => CreateSecretClient(options, serviceProvider));
    }

    private static SecretClient CreateSecretClient(
        KeyVaultSecretAvailableOptions options,
        IServiceProvider serviceProvider
    )
    {
        SecretClientOptions? clientOptions = null;
        if (options.ConfigureClientOptions is not null)
        {
            clientOptions = new SecretClientOptions();
            options.ConfigureClientOptions(clientOptions);
        }

        switch (options.Mode)
        {
            case KeyVaultClientCreationMode.DefaultAzureCredentials:
                var tokenCredential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new SecretClient(options.VaultUri, tokenCredential, clientOptions);
            case KeyVaultClientCreationMode.ClientSecretCredential:
                var credential = new global::Azure.Identity.ClientSecretCredential(
                    options.TenantId,
                    options.ClientId,
                    options.ClientSecret
                );
                return new SecretClient(options.VaultUri, credential, clientOptions);
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
    }
}
