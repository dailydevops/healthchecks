namespace NetEvolve.HealthChecks.Tests.Integration.InfluxDB;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.InfluxDb;

public sealed class InfluxDBDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly InfluxDbContainer _database = new InfluxDbBuilder().WithLogger(NullLogger.Instance).Build();

    public string ConnectionString => _database.GetAddress();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
