namespace NetEvolve.HealthChecks.Oracle.Tests.Integration;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Testcontainers.Oracle;
using Xunit;

[ExcludeFromCodeCoverage]
public sealed class OracleDatabase : IAsyncLifetime
{
    private readonly OracleContainer _database = new OracleBuilder().Build();

    public string GetConnectionString() => _database.GetConnectionString();

    public async Task DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
