namespace NetEvolve.HealthChecks.SqlServer.Tests.Integration;

using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Tests;
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
    public async Task AddSqlServer_ShouldReturnDegraded() =>
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
    public async Task AddSqlServer_ShouldReturnUnhealthy() =>
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
}
