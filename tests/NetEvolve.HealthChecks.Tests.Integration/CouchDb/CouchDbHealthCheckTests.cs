namespace NetEvolve.HealthChecks.Tests.Integration.CouchDb;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MyCouch;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.CouchDb;

[ClassDataSource<CouchDbDatabase>]
[TestGroup(nameof(CouchDb))]
[TestGroup("Z04TestGroup")]
public class CouchDbHealthCheckTests : HealthCheckTestBase
{
    private readonly CouchDbDatabase _database;

    public CouchDbHealthCheckTests(CouchDbDatabase database) => _database = database;

    [Test]
    public async Task AddCouchDb_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddCouchDb(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.DatabaseName = "db";
                        options.Timeout = 10000;
                    }
                ),
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddCouchDb_UseOptionsDoubleRegistered_Healthy() =>
        await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks => healthChecks.AddCouchDb("TestContainerHealthy").AddCouchDb("TestContainerHealthy"),
                    HealthStatus.Healthy
                )
        );

    [Test]
    public async Task AddCouchDb_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddCouchDb(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.DatabaseName = "db";
                        options.CommandAsync = async (client, cancellationToken) =>
                        {
                            await Task.Delay(1000, cancellationToken);
                            _ = await client.Database.HeadAsync(cancellationToken).ConfigureAwait(false);
                        };
                        options.Timeout = 0;
                    }
                ),
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddCouchDb_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddCouchDb(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = "http://invalid:5984";
                        options.Timeout = 100;
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddCouchDb_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCouchDb("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:CouchDb:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:CouchDb:TestContainerHealthy:DatabaseName", "db" },
                    { "HealthChecks:CouchDb:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddCouchDb_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCouchDb("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:CouchDb:TestContainerDegraded:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:CouchDb:TestContainerDegraded:DatabaseName", "db" },
                    { "HealthChecks:CouchDb:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddCouchDb_UseConfiguration_ConnectionStringEmpty_ShouldThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCouchDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:CouchDb:TestNoValues:ConnectionString", "" },
                    { "HealthChecks:CouchDb:TestNoValues:DatabaseName", "db" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddCouchDb_UseConfiguration_TimeoutMinusTwo_ShouldThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddCouchDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:CouchDb:TestNoValues:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:CouchDb:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
