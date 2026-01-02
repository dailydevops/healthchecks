namespace NetEvolve.HealthChecks.Tests.Integration.Cassandra;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Cassandra;

public sealed class CassandraDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly CassandraContainer _database = new CassandraBuilder(
        /*dockerimage*/"cassandra:5.0.0"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
