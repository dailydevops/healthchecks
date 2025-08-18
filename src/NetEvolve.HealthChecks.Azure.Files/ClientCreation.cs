namespace NetEvolve.HealthChecks.Azure.Files;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using global::Azure;
using global::Azure.Core;
using global::Azure.Identity;
using global::Azure.Storage;
using global::Azure.Storage.Files.Shares;
using Microsoft.Extensions.DependencyInjection;

internal class ClientCreation
{
    private ConcurrentDictionary<string, ShareServiceClient>? _shareServiceClients;

    internal ShareServiceClient GetShareServiceClient<TOptions>(
        string name,
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, IFileOptions
    {
        if (options.Mode == FileClientCreationMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<ShareServiceClient>();
        }

        _shareServiceClients ??= new ConcurrentDictionary<string, ShareServiceClient>(StringComparer.OrdinalIgnoreCase);

        return _shareServiceClients.GetOrAdd(name, _ => CreateShareServiceClient(options, serviceProvider));
    }

    internal static ShareServiceClient CreateShareServiceClient<TOptions>(
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, IFileOptions
    {
        ShareClientOptions? clientOptions = null;
        if (options.ConfigureClientOptions is not null)
        {
            clientOptions = new ShareClientOptions();
            options.ConfigureClientOptions(clientOptions);
        }

#pragma warning disable IDE0010 // Add missing cases
        switch (options.Mode)
        {
            case FileClientCreationMode.DefaultAzureCredentials:
                var tokenCredential = serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new ShareServiceClient(options.ServiceUri, tokenCredential, clientOptions);
            case FileClientCreationMode.ConnectionString:
                return new ShareServiceClient(options.ConnectionString, clientOptions);
            case FileClientCreationMode.SharedKey:
                var sharedKeyCredential = new StorageSharedKeyCredential(options.AccountName, options.AccountKey);
                return new ShareServiceClient(options.ServiceUri, sharedKeyCredential, clientOptions);
            case FileClientCreationMode.AzureSasCredential:
                var shareUriBuilder = new ShareUriBuilder(options.ServiceUri) { Sas = null };
                var azureSasCredential = new AzureSasCredential(options.ServiceUri!.Query);

                return new ShareServiceClient(shareUriBuilder.ToUri(), azureSasCredential, clientOptions);
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
#pragma warning restore IDE0010 // Add missing cases
    }
}