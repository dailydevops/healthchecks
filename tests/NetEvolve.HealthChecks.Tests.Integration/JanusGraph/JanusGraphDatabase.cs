namespace NetEvolve.HealthChecks.Tests.Integration.JanusGraph;

using System.Threading.Tasks;
using Gremlin.Net.Driver;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.JanusGraph;

public sealed class JanusGraphDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly JanusGraphContainer _database = new JanusGraphBuilder(
        /*dockerimage*/"janusgraph/janusgraph:1.2.0-20251114-142114.b424a8f"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    public GremlinServer Server =>
        new GremlinServer(_database.Hostname, _database.GetMappedPublicPort(JanusGraphBuilder.JanusGraphPort));

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
