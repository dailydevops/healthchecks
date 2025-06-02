namespace NetEvolve.HealthChecks.Tests.Integration.MongoDb;

using System.Threading.Tasks;
using Testcontainers.MongoDb;

public sealed class MongoDbDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly MongoDbContainer _database = new MongoDbBuilder().Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
