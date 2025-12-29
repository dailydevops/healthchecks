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
    public const string AccountName = AzuriteBuilder.AccountName;
    public const string AccountKey = AzuriteBuilder.AccountKey;

    private readonly AzuriteContainer _container = new AzuriteBuilder()
        .WithImage("mcr.microsoft.com/azure-storage/azurite:latest")
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

        _ = await tableServiceClient.CreateTableIfNotExistsAsync("test").ConfigureAwait(false);

        TableAccountSasUri = tableServiceClient.GenerateSasUri(
            TableAccountSasPermissions.All,
            TableAccountSasResourceTypes.All,
            DateTimeOffset.UtcNow.AddDays(1)
        );
    }

    private async Task PrepareQueueRequirements()
    {
        var queueServiceClient = new QueueServiceClient(ConnectionString);

        _ = await queueServiceClient.CreateQueueAsync("test").ConfigureAwait(false);

        QueueAccountSasUri = queueServiceClient.GenerateAccountSasUri(
            AccountSasPermissions.All,
            DateTimeOffset.UtcNow.AddDays(1),
            AccountSasResourceTypes.All
        );
    }

    private async Task PrepareBlobRequirements()
    {
        var blobServiceClient = new BlobServiceClient(ConnectionString);

        _ = await blobServiceClient.CreateBlobContainerAsync("test").ConfigureAwait(false);

        BlobAccountSasUri = blobServiceClient.GenerateAccountSasUri(
            AccountSasPermissions.All,
            DateTimeOffset.UtcNow.AddDays(1),
            AccountSasResourceTypes.All
        );
    }
}
