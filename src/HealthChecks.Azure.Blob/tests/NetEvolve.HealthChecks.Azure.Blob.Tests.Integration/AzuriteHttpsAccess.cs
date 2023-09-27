namespace NetEvolve.HealthChecks.Azure.Blob.Tests.Integration;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Testcontainers.Azurite;
using Xunit;

[ExcludeFromCodeCoverage]
public sealed class AzuriteHttpsAccess : IAsyncLifetime
{
    public const string AccountName = "testaccount1";
    public const string AccountKey = "SGVsbG8gV29ybGQ=";

    private readonly AzuriteContainer _container = new AzuriteBuilder()
        .WithImage("mcr.microsoft.com/azure-storage/azurite:3.26.0")
        .WithAccountCredentials(AccountName, AccountKey)
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public string BlobEndpoint => _container.GetBlobEndpoint();
    public string QueueEndpoint => _container.GetQueueEndpoint();
    public string TableEndpoint => _container.GetTableEndpoint();

    public async Task DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
