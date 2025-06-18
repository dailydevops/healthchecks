namespace NetEvolve.HealthChecks.Tests.Integration.MongoDb;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.MongoDb;

public sealed class MongoDbDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly MongoDbContainer _database = new MongoDbBuilder().WithLogger(NullLogger.Instance).Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
