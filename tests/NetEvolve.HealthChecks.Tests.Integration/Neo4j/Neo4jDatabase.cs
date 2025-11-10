namespace NetEvolve.HealthChecks.Tests.Integration.Neo4j;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Neo4j;

public sealed class Neo4jDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly Neo4jContainer _database = new Neo4jBuilder().WithLogger(NullLogger.Instance).Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
