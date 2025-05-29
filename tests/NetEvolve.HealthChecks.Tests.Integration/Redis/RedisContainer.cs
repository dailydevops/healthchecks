namespace NetEvolve.HealthChecks.Tests.Integration.Redis;

using System.Threading.Tasks;
using Testcontainers.Redis;
using Xunit;

public sealed class RedisContainer : IAsyncLifetime
{
    private readonly Testcontainers.Redis.RedisContainer _database = new RedisBuilder().Build();

    public string GetConnectionString() => _database.GetConnectionString();

    public async Task DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
