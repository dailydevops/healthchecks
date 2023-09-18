namespace NetEvolve.HealthChecks.Azure.Blob;

using global::Azure;
using global::Azure.Identity;
using global::Azure.Storage;
using global::Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

internal sealed class BlobContainerAvailableHealthCheck
    : ConfigurableHealthCheckBase<BlobContainerAvailableOptions>
{
    private ConcurrentDictionary<string, BlobServiceClient>? _blobServiceClients;
    private readonly IServiceProvider _serviceProvider;

    public BlobContainerAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<BlobContainerAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        BlobContainerAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var blobClient = GetBlobServiceClient(name, options);

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

    private BlobServiceClient GetBlobServiceClient(
        string name,
        BlobContainerAvailableOptions options
    )
    {
        if (options.Mode == ClientCreationMode.ServiceProvider)
        {
            return _serviceProvider.GetRequiredService<BlobServiceClient>();
        }

        if (_blobServiceClients is null)
        {
            _blobServiceClients = new ConcurrentDictionary<string, BlobServiceClient>(
                StringComparer.OrdinalIgnoreCase
            );
        }

        return _blobServiceClients.GetOrAdd(name, _ => CreateBlobServiceClient(options));
    }

    private static BlobServiceClient CreateBlobServiceClient(BlobContainerAvailableOptions options)
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
            {
                var serviceUri = new Uri(options.ServiceUri!, UriKind.Absolute);
                var tokenCredential = new DefaultAzureCredential();
                return new BlobServiceClient(serviceUri, tokenCredential, clientOptions);
            }
            case ClientCreationMode.ConnectionString:
            {
                return new BlobServiceClient(options.ConnectionString, clientOptions);
            }
            case ClientCreationMode.SharedKey:
            {
                var serviceUri = new Uri(options.ServiceUri!, UriKind.Absolute);
                var sharedKeyCredential = new StorageSharedKeyCredential(
                    options.AccountName,
                    options.SharedAccessToken
                );
                return new BlobServiceClient(serviceUri, sharedKeyCredential, clientOptions);
            }
            case ClientCreationMode.AzureSasCredential:
            {
                var serviceUri = new Uri(options.ServiceUri!, UriKind.Absolute);
                var azureSasCredential = new AzureSasCredential(options.SharedAccessToken!);
                return new BlobServiceClient(serviceUri, azureSasCredential, clientOptions);
            }
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
    }
}
