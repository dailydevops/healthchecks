namespace NetEvolve.HealthChecks.Tests.Integration.JanusGraph;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.JanusGraph;

public sealed class JanusGraphDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly JanusGraphContainer _database = new JanusGraphBuilder().WithLogger(NullLogger.Instance).Build();

#pragma warning disable CA1056 // URI-like properties should not be strings
    public string HostUrl => _database.Hostname;
#pragma warning restore CA1056 // URI-like properties should not be strings

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
