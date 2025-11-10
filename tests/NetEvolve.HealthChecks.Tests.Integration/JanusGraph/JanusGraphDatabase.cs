namespace NetEvolve.HealthChecks.Tests.Integration.JanusGraph;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.JanusGraph;

public sealed class JanusGraphDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly JanusGraphContainer _database = new JanusGraphBuilder().WithLogger(NullLogger.Instance).Build();

    public string Hostname => _database.Hostname;

    public int Port => _database.GetMappedPublicPort(JanusGraphBuilder.JanusGraphPort);

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
