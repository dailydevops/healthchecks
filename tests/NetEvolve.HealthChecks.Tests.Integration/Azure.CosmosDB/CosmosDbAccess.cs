namespace NetEvolve.HealthChecks.Tests.Integration.Azure.CosmosDB;

using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.CosmosDb;

public sealed class CosmosDbAccess : IAsyncInitializer, IAsyncDisposable
{
    private readonly CosmosDbContainer _container = new CosmosDbBuilder()
        .WithLogger(NullLogger.Instance)
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);

        var client = new CosmosClient(ConnectionString);
        _ = await client.CreateDatabaseIfNotExistsAsync("testdb").ConfigureAwait(false);
    }
}
