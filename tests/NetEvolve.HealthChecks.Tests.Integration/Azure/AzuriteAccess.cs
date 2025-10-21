namespace NetEvolve.HealthChecks.Tests.Integration.Azure;

using System;
using System.Threading.Tasks;
using global::Azure.Data.Tables;
using global::Azure.Data.Tables.Sas;
using global::Azure.Storage.Blobs;
using global::Azure.Storage.Queues;
using global::Azure.Storage.Sas;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Azurite;

public sealed class AzuriteAccess : IAsyncInitializer, IAsyncDisposable
{
    public const string AccountName = "devstoreaccount1";
    public const string AccountKey =
        "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

    private readonly AzuriteContainer _container = new AzuriteBuilder()
        .WithLogger(NullLogger.Instance)
        .WithCommand("--skipApiVersionCheck")
        .Build();

    public Uri BlobAccountSasUri { get; private set; } = default!;

    public Uri BlobServiceEndpoint => new Uri(_container.GetBlobEndpoint());

    public string ConnectionString => _container.GetConnectionString();

    public Uri QueueAccountSasUri { get; private set; } = default!;

    public Uri QueueServiceEndpoint => new Uri(_container.GetQueueEndpoint());

    public Uri TableAccountSasUri { get; private set; } = default!;

    public Uri TableServiceEndpoint => new Uri(_container.GetTableEndpoint());

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);

        await Task.WhenAll(PrepareBlobRequirements(), PrepareQueueRequirements(), PrepareTableRequirements())
            .ConfigureAwait(false);
    }

    private async Task PrepareTableRequirements()
    {
        var tableServiceClient = new TableServiceClient(ConnectionString);

        var tableClient = tableServiceClient.GetTableClient("test");
        _ = await tableClient.CreateIfNotExistsAsync().ConfigureAwait(false);

        TableAccountSasUri = tableClient.GenerateSasUri(TableSasPermissions.All, DateTimeOffset.UtcNow.AddDays(1));
    }

    private async Task PrepareQueueRequirements()
    {
        var queueServiceClient = new QueueServiceClient(ConnectionString);

        var queueClient = queueServiceClient.GetQueueClient("test");
        _ = await queueClient.CreateIfNotExistsAsync().ConfigureAwait(false);

        QueueAccountSasUri = queueServiceClient.GenerateAccountSasUri(
            AccountSasPermissions.All,
            DateTimeOffset.UtcNow.AddDays(1),
            AccountSasResourceTypes.All
        );
    }

    private async Task PrepareBlobRequirements()
    {
        var blobServiceClient = new BlobServiceClient(ConnectionString);

        var blobContainerClient = blobServiceClient.GetBlobContainerClient("test");
        _ = await blobContainerClient.CreateIfNotExistsAsync().ConfigureAwait(false);

        BlobAccountSasUri = blobServiceClient.GenerateAccountSasUri(
            AccountSasPermissions.All,
            DateTimeOffset.UtcNow.AddDays(1),
            AccountSasResourceTypes.All
        );
    }
}
