namespace NetEvolve.HealthChecks.Tests.Integration.QuestDB;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.QuestDB;
using NetEvolve.HealthChecks.Tests.Integration.QuestDB.Container;

[TestGroup(nameof(QuestDB))]
[TestGroup("Z03TestGroup")]
[ClassDataSource<QuestDBDatabase>(Shared = SharedType.PerClass)]
public sealed class QuestDBHealthCheckTests : HealthCheckTestBase
{
    private readonly QuestDBDatabase _database;

    public QuestDBHealthCheckTests(QuestDBDatabase database) => _database = database;

    [Test]
    public async Task AddQuestDB_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddQuestDB(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.Timeout = 10000;
                        options.StatusUri = _database.StatusUri.ToString();
                    }
                ),
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddQuestDB_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddQuestDB(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.Timeout = 0;
                        options.StatusUri = _database.StatusUri.ToString();
                    }
                ),
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddQuestDB_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddQuestDB(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.Timeout = 0;
                        options.StatusUri = "invalid";
                    }
                ),
            HealthStatus.Unhealthy
        );

    [Test]
    public async Task AddQuestDB_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddQuestDB("TestContainerConfigHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:QuestDB:TestContainerConfigHealthy:Timeout", "10000" },
                    { "HealthChecks:QuestDB:TestContainerConfigHealthy:StatusUri", _database.StatusUri.ToString() },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddQuestDB_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddQuestDB("TestContainerConfigDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:QuestDB:TestContainerConfigDegraded:Timeout", "0" },
                    { "HealthChecks:QuestDB:TestContainerConfigHealthy:StatusUri", _database.StatusUri.ToString() },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
