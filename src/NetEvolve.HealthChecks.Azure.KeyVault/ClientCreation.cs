namespace NetEvolve.HealthChecks.Azure.KeyVault;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using global::Azure.Core;
using global::Azure.Identity;
using global::Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.DependencyInjection;

internal class ClientCreation
{
    private ConcurrentDictionary<string, SecretClient>? _secretClients;

    internal SecretClient GetSecretClient<TOptions>(
        string name,
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, IKeyVaultOptions
    {
        if (options.Mode == KeyVaultClientCreationMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<SecretClient>();
        }

        _secretClients ??= new ConcurrentDictionary<string, SecretClient>(StringComparer.OrdinalIgnoreCase);

        return _secretClients.GetOrAdd(name, _ => CreateSecretClient(options, serviceProvider));
    }

    internal static SecretClient CreateSecretClient<TOptions>(
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, IKeyVaultOptions
    {
        SecretClientOptions? clientOptions = null;
        if (options.ConfigureClientOptions is not null)
        {
            clientOptions = new SecretClientOptions();
            options.ConfigureClientOptions(clientOptions);
        }

#pragma warning disable IDE0010 // Add missing cases
        switch (options.Mode)
        {
            case KeyVaultClientCreationMode.DefaultAzureCredentials:
                var tokenCredential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new SecretClient(options.VaultUri, tokenCredential, clientOptions);
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
#pragma warning restore IDE0010 // Add missing cases
    }
}
