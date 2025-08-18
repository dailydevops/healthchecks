namespace NetEvolve.HealthChecks.Azure.Files;

using System;
using System.Threading;
using global::Azure.Storage.Files.Shares;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class FileServiceAvailableConfigure
    : IConfigureNamedOptions<FileServiceAvailableOptions>,
        IValidateOptions<FileServiceAvailableOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public FileServiceAvailableConfigure(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, FileServiceAvailableOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:AzureFileService:{name}", options);
    }

    public void Configure(FileServiceAvailableOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, FileServiceAvailableOptions options)
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
            FileClientCreationMode.ServiceProvider => ValidateModeServiceProvider(),
            FileClientCreationMode.ConnectionString => ValidateModeConnectionString(options),
            FileClientCreationMode.DefaultAzureCredentials => ValidateModeDefaultAzureCredentials(options),
            FileClientCreationMode.SharedKey => ValidateModeSharedKey(options),
            FileClientCreationMode.AzureSasCredential => ValidateModeAzureSasCredential(options),
            _ => Fail($"The mode `{mode}` is not supported."),
        };
    }

    private static ValidateOptionsResult ValidateModeAzureSasCredential(FileServiceAvailableOptions options)
    {
        if (options.ServiceUri is null)
        {
            return Fail(
                $"The service url cannot be null when using `{nameof(FileClientCreationMode.AzureSasCredential)}` mode."
            );
        }

        if (!options.ServiceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The service url must be an absolute url when using `{nameof(FileClientCreationMode.AzureSasCredential)}` mode."
            );
        }

        if (string.IsNullOrWhiteSpace(options.ServiceUri.Query))
        {
            return Fail(
                $"The sas query token cannot be null or whitespace when using `{nameof(FileClientCreationMode.AzureSasCredential)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeSharedKey(FileServiceAvailableOptions options)
    {
        if (options.ServiceUri is null)
        {
            return Fail(
                $"The service url cannot be null when using `{nameof(FileClientCreationMode.SharedKey)}` mode."
            );
        }

        if (!options.ServiceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The service url must be an absolute url when using `{nameof(FileClientCreationMode.SharedKey)}` mode."
            );
        }

        if (string.IsNullOrWhiteSpace(options.AccountName))
        {
            return Fail(
                $"The account name cannot be null or whitespace when using `{nameof(FileClientCreationMode.SharedKey)}` mode."
            );
        }

        if (string.IsNullOrWhiteSpace(options.AccountKey))
        {
            return Fail(
                $"The account key cannot be null or whitespace when using `{nameof(FileClientCreationMode.SharedKey)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeDefaultAzureCredentials(FileServiceAvailableOptions options)
    {
        if (options.ServiceUri is null)
        {
            return Fail(
                $"The service url cannot be null when using `{nameof(FileClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        if (!options.ServiceUri.IsAbsoluteUri)
        {
            return Fail(
                $"The service url must be an absolute url when using `{nameof(FileClientCreationMode.DefaultAzureCredentials)}` mode."
            );
        }

        return Success;
    }

    private static ValidateOptionsResult ValidateModeConnectionString(FileServiceAvailableOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return Fail(
                $"The connection string cannot be null or whitespace when using `{nameof(FileClientCreationMode.ConnectionString)}` mode."
            );
        }

        return Success;
    }

    private ValidateOptionsResult ValidateModeServiceProvider()
    {
        if (_serviceProvider.GetService<ShareServiceClient>() is null)
        {
            return Fail(
                $"No service of type `{nameof(ShareServiceClient)}` registered. Please execute `builder.AddAzureClients()`."
            );
        }

        return Success;
    }
}
