namespace NetEvolve.HealthChecks.Tests.Integration.Firebird;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Firebird;

[TestGroup(nameof(Firebird))]
[ClassDataSource<FirebirdDatabase>(Shared = InstanceSharedType.Firebird)]
public sealed class FirebirdHealthCheckTests : HealthCheckTestBase
{
    private readonly FirebirdDatabase _database;

    public FirebirdHealthCheckTests(FirebirdDatabase database) => _database = database;

    [Test]
    public async Task AddFirebird_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddFirebird(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Timeout = 1000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddFirebird_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
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
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddFirebird_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddFirebird(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.Command = "EXCEPTION connect_reject ThisIsATest;";
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddFirebird_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddFirebird("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Firebird:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:Firebird:TestContainerHealthy:TimeOut", "1000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddFirebird_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddFirebird("TestContainerDegraded"),
            HealthStatus.Degraded,
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

    [Test]
    public async Task AddFirebird_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddFirebird("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:Firebird:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddFirebird_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddFirebird("TestNoValues"),
            HealthStatus.Unhealthy,
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
