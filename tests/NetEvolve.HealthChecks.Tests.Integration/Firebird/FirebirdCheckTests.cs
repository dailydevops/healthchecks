namespace NetEvolve.HealthChecks.Tests.Integration.Firebird;

using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Firebird;

[TestGroup(nameof(Firebird))]
public sealed class FirebirdCheckTests : HealthCheckTestBase, IClassFixture<FirebirdDatabase>
{
    private readonly FirebirdDatabase _database;

    public FirebirdCheckTests(FirebirdDatabase database) => _database = database;

    [Fact]
    public async Task AddFirebird_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddFirebird(
                "TestContainerHealthy",
                options => options.ConnectionString = _database.ConnectionString
            );
        });

    [Fact]
    public async Task AddFirebird_UseOptionsDoubleRegistered_ShouldReturnHealthy() =>
        _ = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
            {
                await RunAndVerify(healthChecks =>
                    healthChecks.AddFirebird("TestContainerHealthy").AddFirebird("TestContainerHealthy")
                );
            }
        );

    [Fact]
    public async Task AddFirebird_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddFirebird(
                "TestContainerDegraded",
                options =>
                {
                    options.ConnectionString = _database.ConnectionString;
                    options.Command = "SELECT 1 FROM RDB$DATABASE WHERE 1 <> 1;";
                    options.Timeout = 0;
                }
            );
        });

    [Fact]
    public async Task AddFirebird_UseOptions_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddFirebird(
                "TestContainerUnhealthy",
                options =>
                {
                    options.ConnectionString = _database.ConnectionString;
                    options.Command = "EXCEPTION connect_reject ThisIsATest;";
                }
            );
        });

    [Fact]
    public async Task AddFirebird_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddFirebird("TestContainerHealthy"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Firebird:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddFirebird_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddFirebird("TestContainerDegraded"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Firebird:TestContainerDegraded:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:Firebird:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddFirebird_UseConfigration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddFirebird("TestNoValues"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Firebird:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddFirebird_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddFirebird("TestNoValues"),
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Firebird:TestNoValues:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:Firebird:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
