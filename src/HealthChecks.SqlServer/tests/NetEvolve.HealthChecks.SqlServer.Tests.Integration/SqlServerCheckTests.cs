namespace NetEvolve.HealthChecks.SqlServer.Tests.Integration;

using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Tests;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

[IntegrationTest]
[ExcludeFromCodeCoverage]
public class SqlServerCheckTests : HealthCheckTestBase, IClassFixture<SqlServerDatabase>
{
    private readonly SqlServerDatabase _database;

    public SqlServerCheckTests(SqlServerDatabase database) => _database = database;

    [Fact]
    public async Task AddSqlServer_UseOptions_ShouldReturnHealthy() =>
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
    public async Task AddSqlServer_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
            {
                _ = healthChecks.AddSqlServer(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                        options.Command = "SELECT 1; WAITFOR DELAY '00:00:00.100';";
                        options.Timeout = 1;
                    }
                );
            })
            .ConfigureAwait(false);

    [Fact]
    public async Task AddSqlServer_UseOptions_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
            {
                _ = healthChecks.AddSqlServer(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.GetConnectionString();
                        options.Command = "RAISERROR('This is a test.',16,1)";
                    }
                );
            })
            .ConfigureAwait(false);

    [Fact]
    public async Task AddSqlServer_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddSqlServer("TestContainerHealthy");
                },
                config =>
                {
                    var values = new Dictionary<string, string?>
                    {
                        {
                            "HealthChecks:SqlServerTestContainerHealthy:ConnectionString",
                            _database.GetConnectionString()
                        }
                    };
                    _ = config.AddInMemoryCollection(values);
                }
            )
            .ConfigureAwait(false);

    [Fact]
    public async Task AddSqlServer_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddSqlServer("TestContainerDegraded");
                },
                config =>
                {
                    var values = new Dictionary<string, string?>
                    {
                        {
                            "HealthChecks:SqlServerTestContainerDegraded:ConnectionString",
                            _database.GetConnectionString()
                        },
                        { "HealthChecks:SqlServerTestContainerDegraded:Timeout", "0" }
                    };
                    _ = config.AddInMemoryCollection(values);
                }
            )
            .ConfigureAwait(false);
}
