﻿namespace NetEvolve.HealthChecks.Azure.Blobs;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using global::Azure;
using global::Azure.Core;
using global::Azure.Identity;
using global::Azure.Storage;
using global::Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class BlobServiceAvailableHealthCheck
    : ConfigurableHealthCheckBase<BlobServiceAvailableOptions>
{
    private static ConcurrentDictionary<string, BlobServiceClient>? _blobServiceClients;
    private readonly IServiceProvider _serviceProvider;

    public BlobServiceAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<BlobServiceAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        BlobServiceAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var blobClient = GetBlobServiceClient(name, options, _serviceProvider);

        var blobTask = blobClient
            .GetBlobContainersAsync(cancellationToken: cancellationToken)
            .AsPages(pageSizeHint: 1)
            .GetAsyncEnumerator(cancellationToken)
            .MoveNextAsync();

        var (isValid, result) = await blobTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return (isValid && result)
            ? HealthCheckResult.Healthy($"{name}: Healthy")
            : HealthCheckResult.Degraded($"{name}: Degraded");
    }

    private static BlobServiceClient GetBlobServiceClient(
        string name,
        BlobServiceAvailableOptions options,
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

    private static BlobServiceClient CreateBlobServiceClient(
        BlobServiceAvailableOptions options,
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
