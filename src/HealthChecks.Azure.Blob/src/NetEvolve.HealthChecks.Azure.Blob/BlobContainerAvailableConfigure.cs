namespace NetEvolve.HealthChecks.Azure.Blob;

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

        if (
            mode == ClientCreationMode.ServiceProvider
            && _serviceProvider.GetService<BlobServiceClient>() is null
        )
        {
            return Fail(
                $"No service of type `{nameof(BlobServiceClient)}` registered. Please execute `builder.AddAzureClients()`."
            );
        }

        if (
            mode == ClientCreationMode.ConnectionString
            && string.IsNullOrWhiteSpace(options.ConnectionString)
        )
        {
            return Fail(
                $"The connection string cannot be null or whitespace when using `{nameof(ClientCreationMode.ConnectionString)}` mode."
            );
        }

        if (
            (
                mode == ClientCreationMode.DefaultAzureCredentials
                || mode == ClientCreationMode.SharedKey
                || mode == ClientCreationMode.AzureSasCredential
            ) && string.IsNullOrWhiteSpace(options.ServiceUri)
        )
        {
            return Fail(
                $"The service url cannot be null or whitspace when using `{options.Mode}` mode."
            );
        }

        return Success;
    }
}
