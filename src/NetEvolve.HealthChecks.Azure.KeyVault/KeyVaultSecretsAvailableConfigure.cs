namespace NetEvolve.HealthChecks.Azure.KeyVault;

using System;
using System.Threading;
using global::Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class KeyVaultSecretsAvailableConfigure
    : IConfigureNamedOptions<KeyVaultSecretsAvailableOptions>,
        IValidateOptions<KeyVaultSecretsAvailableOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public KeyVaultSecretsAvailableConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, KeyVaultSecretsAvailableOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureKeyVault:{name}", options);
    }

    public void Configure(KeyVaultSecretsAvailableOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, KeyVaultSecretsAvailableOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        if (options.Timeout < Timeout.Infinite)
        {
            return Fail("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.");
        }

        var mode = options.Mode;

        return options.Mode switch
        {
            KeyVaultClientCreationMode.ServiceProvider => ValidateModeServiceProvider(),
            KeyVaultClientCreationMode.DefaultAzureCredentials => ValidateModeDefaultAzureCredentials(options),
            _ => Fail($"The mode `{mode}` is not supported."),
        };
    }

    private static ValidateOptionsResult ValidateModeDefaultAzureCredentials(KeyVaultSecretsAvailableOptions options)
    {
        if (options.VaultUri is null)
        {
            return Fail(
                $"The vault URI cannot be null when using `{nameof(KeyVaultClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        if (!options.VaultUri.IsAbsoluteUri)
        {
            return Fail(
                $"The vault URI must be an absolute URI when using `{nameof(KeyVaultClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        return Success;
    }

    private ValidateOptionsResult ValidateModeServiceProvider()
    {
        if (_serviceProvider.GetService<SecretClient>() is null)
        {
            return Fail(
                $"No service of type `{nameof(SecretClient)}` registered. Please execute `builder.AddAzureClients()`."
            );
        }

        return Success;
    }
}
