namespace NetEvolve.HealthChecks.Azure.KeyVault;

using System;
using global::Azure.Identity;
using global::Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.DependencyInjection;

internal static class KeyVaultClientFactory
{
    public static SecretClient CreateSecretClient(
        string name,
        KeyVaultOptions options,
        IServiceProvider serviceProvider
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        return options.Mode switch
        {
            KeyVaultClientCreationMode.ServiceProvider => GetFromServiceProvider(serviceProvider),
            KeyVaultClientCreationMode.DefaultAzureCredentials => CreateWithDefaultCredentials(options),
            _ => throw new NotSupportedException($"Creation mode `{options.Mode}` is not supported."),
        };
    }

    private static SecretClient GetFromServiceProvider(IServiceProvider serviceProvider) =>
        serviceProvider.GetRequiredService<SecretClient>();

    private static SecretClient CreateWithDefaultCredentials(KeyVaultOptions options)
    {
        if (options.VaultUri is null)
        {
            throw new InvalidOperationException("VaultUri cannot be null when using DefaultAzureCredentials mode.");
        }

        var credential = new DefaultAzureCredential();
        var clientOptions = new SecretClientOptions();
        options.ConfigureClientOptions?.Invoke(clientOptions);

        return new SecretClient(options.VaultUri, credential, clientOptions);
    }
}
