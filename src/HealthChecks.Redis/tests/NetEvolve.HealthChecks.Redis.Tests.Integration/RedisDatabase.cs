namespace NetEvolve.HealthChecks.Redis.Tests.Integration;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Testcontainers.Redis;
using Xunit;

[ExcludeFromCodeCoverage]
public sealed class RedisDatabase : IAsyncLifetime
{
    private readonly RedisContainer _database = new RedisBuilder().Build();

    public string GetConnectionString() => _database.GetConnectionString();

    public async Task DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
