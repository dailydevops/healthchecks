namespace NetEvolve.HealthChecks.Azure.KeyVault;

using System;
using System.Threading;
using global::Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class KeyVaultOptionsConfigure
    : IConfigureNamedOptions<KeyVaultOptions>,
        IValidateOptions<KeyVaultOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public KeyVaultOptionsConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, KeyVaultOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureKeyVault:{name}", options);
    }

    public void Configure(KeyVaultOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, KeyVaultOptions options)
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

        if (options.Mode is null)
        {
            return Fail("The client creation mode cannot be null.");
        }

        return options.Mode switch
        {
            KeyVaultClientCreationMode.ServiceProvider => ValidateModeServiceProvider(),
            KeyVaultClientCreationMode.DefaultAzureCredentials => ValidateModeDefaultAzureCredentials(options),
            _ => Fail($"The mode `{options.Mode}` is not supported."),
        };
    }

    private static ValidateOptionsResult ValidateModeDefaultAzureCredentials(KeyVaultOptions options)
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
                $"No service of type `{nameof(SecretClient)}` registered. Please register a SecretClient instance."
            );
        }

        return Success;
    }
}
