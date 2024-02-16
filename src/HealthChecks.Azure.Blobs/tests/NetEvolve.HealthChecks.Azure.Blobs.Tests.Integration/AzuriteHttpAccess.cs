namespace NetEvolve.HealthChecks.Azure.Blobs.Tests.Integration;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Testcontainers.Azurite;
using Xunit;

[ExcludeFromCodeCoverage]
public sealed class AzuriteHttpAccess : IAsyncLifetime
{
    public const string AccountName = "testaccount1";
    public const string AccountKey = "SGVsbG8gV29ybGQ=";

    private readonly AzuriteContainer _container = new AzuriteBuilder()
    //.WithAccountCredentials(AccountName, AccountKey)
    .Build();

    public string ConnectionString => _container.GetConnectionString();

    //public Uri BlobEndpoint => new Uri(_container.GetBlobEndpoint(), UriKind.Absolute);
    //public Uri QueueEndpoint => new Uri(_container.GetQueueEndpoint(), UriKind.Absolute);
    //public Uri TableEndpoint => new Uri(_container.GetTableEndpoint(), UriKind.Absolute);

    public async Task DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
