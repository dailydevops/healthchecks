namespace NetEvolve.HealthChecks.Tests.Integration.Azure.Cosmos;

using System;
using System.Threading.Tasks;
using Testcontainers.CosmosDb;
using TestContainer = Testcontainers.CosmosDb.CosmosDbContainer;

public sealed class CosmosDbContainer : IAsyncLifetime, IAsyncDisposable
{
    private readonly TestContainer _container = new CosmosDbBuilder().Build();

    public async Task DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);

    async ValueTask IAsyncDisposable.DisposeAsync() =>
        await _container.DisposeAsync().ConfigureAwait(false);
}
