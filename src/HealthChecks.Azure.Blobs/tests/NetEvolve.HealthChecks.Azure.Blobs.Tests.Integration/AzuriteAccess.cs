namespace NetEvolve.HealthChecks.Azure.Tests.Integration;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Testcontainers.Azurite;
using Xunit;

[ExcludeFromCodeCoverage]
public sealed class AzuriteAccess : IAsyncLifetime, IAsyncDisposable
{
    // TODO: Change when Testcontainers is supporting this
    //public const string AccountName = "testaccount1";
    //public const string AccountKey = "SGVsbG8gV29ybGQ=";
    public const string AccountName = "devstoreaccount1";
    public const string AccountKey =
        "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

    private readonly AzuriteContainer _container = new AzuriteBuilder()
    // TODO: Change when Testcontainers is supporting this
    // .WithAccountCredentials(AccountName, AccountKey)
    .Build();

    public string ConnectionString => _container.GetConnectionString();

    //public Uri BlobEndpoint => new Uri(_container.GetBlobEndpoint(), UriKind.Absolute);
    //public Uri QueueEndpoint => new Uri(_container.GetQueueEndpoint(), UriKind.Absolute);
    //public Uri TableEndpoint => new Uri(_container.GetTableEndpoint(), UriKind.Absolute);

    public async Task DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);

    async ValueTask IAsyncDisposable.DisposeAsync() =>
        await _container.DisposeAsync().ConfigureAwait(false);
}
