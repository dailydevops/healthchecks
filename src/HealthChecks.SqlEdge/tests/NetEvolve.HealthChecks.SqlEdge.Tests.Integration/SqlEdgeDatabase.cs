namespace NetEvolve.HealthChecks.SqlEdge.Tests.Integration;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Testcontainers.SqlEdge;
using Xunit;

[ExcludeFromCodeCoverage]
public sealed class SqlEdgeDatabase : IAsyncLifetime
{
    private readonly SqlEdgeContainer _database = new SqlEdgeBuilder().Build();

    public string ConnectionString => _database.GetConnectionString();

    public async Task DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
