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

internal static class ClientCreation
{
    private static ConcurrentDictionary<string, BlobServiceClient>? _blobServiceClients;

    internal static BlobServiceClient GetBlobServiceClient<TOptions>(
        string name,
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, IBlobOptions
    {
        if (options.Mode == BlobClientCreationMode.ServiceProvider)
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

    internal static BlobServiceClient CreateBlobServiceClient<TOptions>(
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, IBlobOptions
    {
        BlobClientOptions? clientOptions = null;
        if (options.ConfigureClientOptions is not null)
        {
            clientOptions = new BlobClientOptions();
            options.ConfigureClientOptions(clientOptions);
        }

#pragma warning disable IDE0010 // Add missing cases
        switch (options.Mode)
        {
            case BlobClientCreationMode.DefaultAzureCredentials:
                var tokenCredential =
                    serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new BlobServiceClient(options.ServiceUri, tokenCredential, clientOptions);
            case BlobClientCreationMode.ConnectionString:
                return new BlobServiceClient(options.ConnectionString, clientOptions);
            case BlobClientCreationMode.SharedKey:
                var sharedKeyCredential = new StorageSharedKeyCredential(
                    options.AccountName,
                    options.AccountKey
                );
                return new BlobServiceClient(
                    options.ServiceUri,
                    sharedKeyCredential,
                    clientOptions
                );
            case BlobClientCreationMode.AzureSasCredential:
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
#pragma warning restore IDE0010 // Add missing cases
    }
}
