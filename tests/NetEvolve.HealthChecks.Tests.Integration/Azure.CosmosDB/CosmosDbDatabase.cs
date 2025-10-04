namespace NetEvolve.HealthChecks.Tests.Integration.Azure.CosmosDB;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.CosmosDb;

public sealed class CosmosDbDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly CosmosDbContainer _database = new CosmosDbBuilder().WithLogger(NullLogger.Instance).Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
