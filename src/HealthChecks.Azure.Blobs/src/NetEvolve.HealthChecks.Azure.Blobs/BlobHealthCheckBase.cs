namespace NetEvolve.HealthChecks.Azure.Blobs;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using global::Azure;
using global::Azure.Core;
using global::Azure.Identity;
using global::Azure.Storage;
using global::Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;

internal abstract class BlobHealthCheckBase<TOptions> : ConfigurableHealthCheckBase<TOptions>
    where TOptions : class, IBlobOptions
{
    protected readonly IServiceProvider _serviceProvider;
    private static ConcurrentDictionary<string, BlobServiceClient>? _blobServiceClients;

    /// <inheritdoc/>
    protected BlobHealthCheckBase(
        IServiceProvider serviceProvider,
        IOptionsMonitor<TOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    internal static BlobServiceClient GetBlobServiceClient(
        string name,
        TOptions options,
        IServiceProvider serviceProvider
    )
    {
        if (options.Mode == ClientCreationMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<BlobServiceClient>();
        }

        if (_blobServiceClients is null)
        {
            _blobServiceClients = new ConcurrentDictionary<string, BlobServiceClient>(
                StringComparer.OrdinalIgnoreCase
            );
        }

        return _blobServiceClients.GetOrAdd(
            name,
            _ => CreateBlobServiceClient(options, serviceProvider)
        );
    }

    internal static BlobServiceClient CreateBlobServiceClient(
        TOptions options,
        IServiceProvider serviceProvider
    )
    {
        BlobClientOptions? clientOptions = null;
        if (options.ConfigureClientOptions is not null)
        {
            clientOptions = new BlobClientOptions();
            options.ConfigureClientOptions(clientOptions);
        }

        switch (options.Mode)
        {
            case ClientCreationMode.DefaultAzureCredentials:
                var tokenCredential =
                    serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new BlobServiceClient(options.ServiceUri, tokenCredential, clientOptions);
            case ClientCreationMode.ConnectionString:
                return new BlobServiceClient(options.ConnectionString, clientOptions);
            case ClientCreationMode.SharedKey:
                var sharedKeyCredential = new StorageSharedKeyCredential(
                    options.AccountName,
                    options.AccountKey
                );
                return new BlobServiceClient(
                    options.ServiceUri,
                    sharedKeyCredential,
                    clientOptions
                );
            case ClientCreationMode.AzureSasCredential:
                var blobUriBuilder = new BlobUriBuilder(options.ServiceUri) { Sas = null };
                var azureSasCredential = new AzureSasCredential(options.ServiceUri!.Query);

                return new BlobServiceClient(
                    blobUriBuilder.ToUri(),
                    azureSasCredential,
                    clientOptions
                );
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
    }
}
