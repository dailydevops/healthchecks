namespace NetEvolve.HealthChecks.Tests.Integration.CouchDb;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.CouchDb;

public sealed class CouchDbDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly CouchDbContainer _database = new CouchDbBuilder().WithLogger(NullLogger.Instance).Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
