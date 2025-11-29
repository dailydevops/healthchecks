namespace NetEvolve.HealthChecks.Tests.Integration.InfluxDB;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using global::InfluxDB.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.InfluxDB;

[ClassDataSource<InfluxDBDatabase>(Shared = SharedType.PerClass)]
[TestGroup(nameof(InfluxDB))]
[TestGroup("Z03TestGroup")]
public class InfluxDBHealthCheckTests : HealthCheckTestBase, IAsyncInitializer, IDisposable
{
    private readonly InfluxDBDatabase _database;
#pragma warning disable TUnit0023 // Member should be disposed within a clean up method
    private InfluxDBClient _client = default!;
#pragma warning restore TUnit0023 // Member should be disposed within a clean up method
    private bool _disposed;

    public InfluxDBHealthCheckTests(InfluxDBDatabase database) => _database = database;

    public Task InitializeAsync()
    {
        _client = new InfluxDBClient(_database.ConnectionString);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _client?.Dispose();
            }

            _disposed = true;
        }
    }

    [Test]
    public async Task AddInfluxDB_UseOptions_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddInfluxDB("TestContainerHealthy", options => options.Timeout = 10000),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddSingleton<IInfluxDBClient>(_client)
        );

    [Test]
    public async Task AddInfluxDB_UseOptionsWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddInfluxDB(
                    "TestContainerKeyedHealthy",
                    options =>
                    {
                        options.KeyedService = "influxdb-test";
                        options.Timeout = 10000;
                    }
                ),
            HealthStatus.Healthy,
            serviceBuilder: services => services.AddKeyedSingleton<IInfluxDBClient>("influxdb-test", (_, _) => _client)
        );

    [Test]
    public async Task AddInfluxDB_UseOptionsDoubleRegistered_Healthy() =>
        await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
                await RunAndVerify(
                    healthChecks =>
                        healthChecks.AddInfluxDB("TestContainerHealthy").AddInfluxDB("TestContainerHealthy"),
                    HealthStatus.Healthy,
                    serviceBuilder: services => services.AddSingleton<IInfluxDBClient>(_client)
                )
        );

    [Test]
    public async Task AddInfluxDB_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.AddInfluxDB(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.PingAsync = async (client, cancellationToken) =>
                        {
                            await Task.Delay(1000, cancellationToken);

                            return await client.PingAsync().ConfigureAwait(false);
                        };
                        options.Timeout = 0;
                    }
                ),
            HealthStatus.Degraded,
            serviceBuilder: services => services.AddSingleton<IInfluxDBClient>(_client)
        );

    [Test]
    public async Task AddInfluxDB_UseOptions_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddInfluxDB(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.PingAsync = async (client, cancellationToken) =>
                        {
                            await Task.Delay(0, cancellationToken);
                            throw new InvalidOperationException("test");
                        };
                    }
                );
            },
            HealthStatus.Unhealthy,
            serviceBuilder: services => services.AddSingleton<IInfluxDBClient>(_client)
        );

    [Test]
    public async Task AddInfluxDB_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddInfluxDB("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:InfluxDB:TestContainerHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton<IInfluxDBClient>(_client)
        );

    [Test]
    public async Task AddInfluxDB_UseConfigurationWithKeyedService_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddInfluxDB("TestContainerKeyedHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:InfluxDB:TestContainerKeyedHealthy:KeyedService", "influxdb-test-config" },
                    { "HealthChecks:InfluxDB:TestContainerKeyedHealthy:Timeout", "10000" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services =>
                services.AddKeyedSingleton<IInfluxDBClient>("influxdb-test-config", (_, _) => _client)
        );

    [Test]
    public async Task AddInfluxDB_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddInfluxDB("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:InfluxDB:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton<IInfluxDBClient>(_client)
        );

    [Test]
    public async Task AddInfluxDB_UseConfiguration_ConnectionStringEmpty_ShouldThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddInfluxDB("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:InfluxDB:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton<IInfluxDBClient>(_client)
        );

    [Test]
    public async Task AddInfluxDB_UseConfiguration_TimeoutMinusTwo_ShouldThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddInfluxDB("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:InfluxDB:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            },
            serviceBuilder: services => services.AddSingleton<IInfluxDBClient>(_client)
        );
}
