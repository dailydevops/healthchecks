namespace NetEvolve.HealthChecks.Tests.Integration.GCP.Firestore;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.GCP.Firestore;

[TestGroup(nameof(Firestore))]
[ClassDataSource<FirestoreDatabase>(Shared = InstanceSharedType.Firestore)]
public sealed class FirestoreHealthCheckTests : HealthCheckTestBase
{
    private readonly FirestoreDatabase _database;

    public FirestoreHealthCheckTests(FirestoreDatabase database) => _database = database;

    [Test]
    public async Task AddFirestore_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddFirestore("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services =>
            {
                Environment.SetEnvironmentVariable("FIRESTORE_EMULATOR_HOST", _database.EmulatorHost);
                _ = services.AddSingleton(_ => FirestoreDb.Create(_database.ProjectId));
            }
        );

    [Test]
    public async Task AddFirestore_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddFirestore("TestContainerDegraded", options => options.Timeout = 0),
            HealthStatus.Degraded,
            serviceBuilder: services =>
            {
                Environment.SetEnvironmentVariable("FIRESTORE_EMULATOR_HOST", _database.EmulatorHost);
                _ = services.AddSingleton(_ => FirestoreDb.Create(_database.ProjectId));
            }
        );

    [Test]
    public async Task AddFirestore_UseOptionsWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddFirestore(
                    "TestContainerKeyedServiceHealthy",
                    options =>
                    {
                        options.Timeout = 10000;
                        options.KeyedService = "firestore";
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
            {
                Environment.SetEnvironmentVariable("FIRESTORE_EMULATOR_HOST", _database.EmulatorHost);
                _ = services.AddKeyedSingleton("firestore", (_, _) => FirestoreDb.Create(_database.ProjectId));
            }
        );

    [Test]
    public async Task AddFirestore_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddFirestore("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:Firestore:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
            {
                Environment.SetEnvironmentVariable("FIRESTORE_EMULATOR_HOST", _database.EmulatorHost);
                _ = services.AddSingleton(_ => FirestoreDb.Create(_database.ProjectId));
            }
        );

    [Test]
    public async Task AddFirestore_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddFirestore("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:Firestore:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
            {
                Environment.SetEnvironmentVariable("FIRESTORE_EMULATOR_HOST", _database.EmulatorHost);
                _ = services.AddSingleton(_ => FirestoreDb.Create(_database.ProjectId));
            }
        );

    [Test]
    public async Task AddFirestore_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddFirestore("TestNoValues"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:GCP:Firestore:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
            {
                Environment.SetEnvironmentVariable("FIRESTORE_EMULATOR_HOST", _database.EmulatorHost);
                _ = services.AddSingleton(_ => FirestoreDb.Create(_database.ProjectId));
            }
        );
}
