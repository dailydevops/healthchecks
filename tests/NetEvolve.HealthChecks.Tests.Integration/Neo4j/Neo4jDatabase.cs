namespace NetEvolve.HealthChecks.Tests.Integration.Neo4j;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Neo4j;

#pragma warning disable S101 // Types should be named in PascalCase
public sealed class Neo4jDatabase : IAsyncInitializer, IAsyncDisposable
#pragma warning restore S101 // Types should be named in PascalCase
{
    private readonly Neo4jContainer _database = new Neo4jBuilder(
        /*dockerimage*/"neo4j:5.4.0"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
