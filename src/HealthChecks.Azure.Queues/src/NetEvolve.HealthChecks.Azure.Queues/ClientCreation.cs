namespace NetEvolve.HealthChecks.Azure.Queues;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using global::Azure;
using global::Azure.Core;
using global::Azure.Identity;
using global::Azure.Storage;
using global::Azure.Storage.Queues;
using Microsoft.Extensions.DependencyInjection;

internal static class ClientCreation
{
    private static ConcurrentDictionary<string, QueueServiceClient>? _queueServiceClients;

    internal static QueueServiceClient GetQueueServiceClient<TOptions>(
        string name,
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, IQueueOptions
    {
        if (options.Mode == QueueClientCreationMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<QueueServiceClient>();
        }

        if (_queueServiceClients is null)
        {
            _queueServiceClients = new ConcurrentDictionary<string, QueueServiceClient>(
                StringComparer.OrdinalIgnoreCase
            );
        }

        return _queueServiceClients.GetOrAdd(
            name,
            _ => CreateQueueServiceClient(options, serviceProvider)
        );
    }

    internal static QueueServiceClient CreateQueueServiceClient<TOptions>(
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, IQueueOptions
    {
        QueueClientOptions? clientOptions = null;
        if (options.ConfigureClientOptions is not null)
        {
            clientOptions = new QueueClientOptions();
            options.ConfigureClientOptions(clientOptions);
        }

        switch (options.Mode)
        {
            case QueueClientCreationMode.DefaultAzureCredentials:
                var tokenCredential =
                    serviceProvider.GetService<TokenCredential>() ?? new DefaultAzureCredential();
                return new QueueServiceClient(options.ServiceUri, tokenCredential, clientOptions);
            case QueueClientCreationMode.ConnectionString:
                return new QueueServiceClient(options.ConnectionString, clientOptions);
            case QueueClientCreationMode.SharedKey:
                var sharedKeyCredential = new StorageSharedKeyCredential(
                    options.AccountName,
                    options.AccountKey
                );
                return new QueueServiceClient(
                    options.ServiceUri,
                    sharedKeyCredential,
                    clientOptions
                );
            case QueueClientCreationMode.AzureSasCredential:
                var queueUriBuilder = new QueueUriBuilder(options.ServiceUri) { Sas = null };
                var azureSasCredential = new AzureSasCredential(options.ServiceUri!.Query);

                return new QueueServiceClient(
                    queueUriBuilder.ToUri(),
                    azureSasCredential,
                    clientOptions
                );
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
    }
}
