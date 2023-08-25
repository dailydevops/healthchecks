namespace NetEvolve.HealthChecks.SqlServer.Tests.Integration;

using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Tests;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;

public sealed class Database : IAsyncLifetime
{
    private readonly MsSqlContainer _database = new MsSqlBuilder()
        .WithPassword("P4ssw0rd!")
        .Build();

    public string GetConnectionString() => _database.GetConnectionString();

    public async Task DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}

[IntegrationTest]
[ExcludeFromCodeCoverage]
public class SqlServerCheckTests : HealthCheckTestBase, IClassFixture<Database>
{
    private readonly Database _database;

    public SqlServerCheckTests(Database database) => _database = database;

    [Fact]
    public async Task AddSqlServer_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
            {
                _ = healthChecks.AddSqlServer(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                    }
                );
            })
            .ConfigureAwait(false);

    [Fact]
    public async Task AddSqlServer_Timeout_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
            {
                _ = healthChecks.AddSqlServer(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                        options.Timeout = 0;
                    }
                );
            })
            .ConfigureAwait(false);
}
