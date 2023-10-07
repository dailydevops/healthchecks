namespace NetEvolve.HealthChecks.Azure.Blobs;

using global::Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetEvolve.Arguments;
using System;

using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class BlobContainerAvailableConfigure
    : IConfigureNamedOptions<BlobContainerAvailableOptions>,
        IPostConfigureOptions<BlobContainerAvailableOptions>,
        IValidateOptions<BlobContainerAvailableOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public BlobContainerAvailableConfigure(
        IConfiguration configuration,
        IServiceProvider serviceProvider
    )
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, BlobContainerAvailableOptions options)
    {
        Argument.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:{name}", options);
    }

    public void Configure(BlobContainerAvailableOptions options) =>
        Configure(Options.DefaultName, options);

    public void PostConfigure(string? name, BlobContainerAvailableOptions options)
    {
        if (string.IsNullOrWhiteSpace(name) || options.Mode == ClientCreationMode.ServiceProvider)
        {
            return;
        }
    }

    public ValidateOptionsResult Validate(string? name, BlobContainerAvailableOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        var mode = options.Mode;

        return options.Mode switch
        {
            ClientCreationMode.ServiceProvider => ValidateModeServiceProvider(options),
            ClientCreationMode.ConnectionString => ValidateModeConnectionString(options),
            ClientCreationMode.DefaultAzureCredentials
                => ValidateModeDefaultAzureCredentials(options),
            ClientCreationMode.SharedKey => ValidateModeSharedKey(options),
            ClientCreationMode.AzureSasCredential => ValidateModeAzureSasCredential(options),
            _ => Fail($"The mode `{mode}` is not supported."),
        };
    }

    private static ValidateOptionsResult ValidateModeAzureSasCredential(
        BlobContainerAvailableOptions options
    )
    {
        if (options.ServiceUri is null)
        {
            return Fail(
                $"The service url cannot be null when using `{nameof(ClientCreationMode.AzureSasCredential)}` mode."
            );
        }

        if (!options.ServiceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The service url must be an absolute url when using `{nameof(ClientCreationMode.AzureSasCredential)}` mode."
            );
        }

        if (string.IsNullOrWhiteSpace(options.ServiceUri.Query))
        {
            return Fail(
                $"The sas query token cannot be null or whitespace when using `{nameof(ClientCreationMode.AzureSasCredential)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeSharedKey(
        BlobContainerAvailableOptions options
    )
    {
        if (options.ServiceUri is null)
        {
            return Fail(
                $"The service url cannot be null when using `{nameof(ClientCreationMode.SharedKey)}` mode."
            );
        }

        if (!options.ServiceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The service url must be an absolute url when using `{nameof(ClientCreationMode.SharedKey)}` mode."
            );
        }

        if (string.IsNullOrWhiteSpace(options.AccountName))
        {
            return Fail(
                $"The account name cannot be null or whitespace when using `{nameof(ClientCreationMode.SharedKey)}` mode."
            );
        }

        if (string.IsNullOrWhiteSpace(options.AccountKey))
        {
            return Fail(
                $"The account key cannot be null or whitespace when using `{nameof(ClientCreationMode.SharedKey)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeDefaultAzureCredentials(
        BlobContainerAvailableOptions options
    )
    {
        if (options.ServiceUri is null)
        {
            return Fail(
                $"The service url cannot be null when using `{nameof(ClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        if (!options.ServiceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The service url must be an absolute url when using `{nameof(ClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeConnectionString(
        BlobContainerAvailableOptions options
    )
    {
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return Fail(
                $"The connection string cannot be null or whitespace when using `{nameof(ClientCreationMode.ConnectionString)}` mode."
            );
        }

        return Success;
    }

    private ValidateOptionsResult ValidateModeServiceProvider(BlobContainerAvailableOptions options)
    {
        if (_serviceProvider.GetService<BlobServiceClient>() is null)
        {
            return Fail(
                $"No service of type `{nameof(BlobServiceClient)}` registered. Please execute `builder.AddAzureClients()`."
            );
        }

        return Success;
    }
}
