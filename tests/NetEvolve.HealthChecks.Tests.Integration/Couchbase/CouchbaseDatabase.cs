namespace NetEvolve.HealthChecks.Tests.Integration.Couchbase;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Couchbase;

public sealed class CouchbaseDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly CouchbaseContainer _database = new CouchbaseBuilder().WithLogger(NullLogger.Instance).Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
