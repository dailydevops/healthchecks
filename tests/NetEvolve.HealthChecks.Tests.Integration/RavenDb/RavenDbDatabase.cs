namespace NetEvolve.HealthChecks.Tests.Integration.RavenDb;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.RavenDb;

public sealed class RavenDbDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly RavenDbContainer _database = new RavenDbBuilder().WithLogger(NullLogger.Instance).Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
