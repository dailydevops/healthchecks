namespace NetEvolve.HealthChecks.Tests.Integration.ArangoDb;

using System.Threading.Tasks;
using Testcontainers.ArangoDb;

public sealed class ArangoDbDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly ArangoDbContainer _database;

    public ArangoDbDatabase() => _database = new ArangoDbBuilder().WithPassword(Password).Build();

    public string Password { get; } = "password";

    public string TransportAddress => _database.GetTransportAddress();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
