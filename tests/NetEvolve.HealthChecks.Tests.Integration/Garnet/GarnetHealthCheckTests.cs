namespace NetEvolve.HealthChecks.Tests.Integration.Garnet;

using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using global::Garnet.client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Garnet;

[TestGroup(nameof(Garnet))]
[TestGroup("Z03TestGroup")]
[ClassDataSource<GarnetContainer>(Shared = SharedType.PerClass)]
public class GarnetHealthCheckTests : HealthCheckTestBase
{
    private readonly GarnetContainer _database;

    public GarnetHealthCheckTests(GarnetContainer database) => _database = database;

    [Test]
    public async Task AddGarnet_UseOptionsCreate_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddGarnet(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.Hostname = _database.Hostname;
                        options.Port = _database.Port;
                        options.Mode = ConnectionHandleMode.Create;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddGarnet_UseOptionsServiceProvider_Healthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddGarnet(
                    "TestContainerHealthy",
                    options =>
                    {
                        options.Mode = ConnectionHandleMode.ServiceProvider;
                        options.Timeout = 10000; // Set a reasonable timeout
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: builder =>
            {
                _ = builder.AddSingleton(services => new GarnetClient(
                    new IPEndPoint(IPAddress.Parse(_database.Hostname), _database.Port)
                //new DnsEndPoint(_database.Hostname, _database.Port)
                ));
            }
        );

    [Test]
    public async Task AddGarnet_UseOptions_Degraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddGarnet(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.Hostname = _database.Hostname;
                        options.Port = _database.Port;
                        options.Timeout = 0;
                        options.Mode = ConnectionHandleMode.Create;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Test]
    public async Task AddGarnet_UseConfiguration_Healthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddGarnet("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:GarnetDatabase:TestContainerHealthy:Hostname", _database.Hostname },
                    { "HealthChecks:GarnetDatabase:TestContainerHealthy:Port", $"{_database.Port}" },
                    { "HealthChecks:GarnetDatabase:TestContainerHealthy:Timeout", "10000" },
                    { "HealthChecks:GarnetDatabase:TestContainerHealthy:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddGarnet_UseConfiguration_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddGarnet("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:GarnetDatabase:TestContainerDegraded:Hostname", _database.Hostname },
                    { "HealthChecks:GarnetDatabase:TestContainerDegraded:Port", $"{_database.Port}" },
                    { "HealthChecks:GarnetDatabase:TestContainerDegraded:Timeout", "0" },
                    { "HealthChecks:GarnetDatabase:TestContainerDegraded:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddGarnet_UseConfigurationWithLocalhost_Degraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddGarnet("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:GarnetDatabase:TestContainerDegraded:Hostname", "localhost" },
                    { "HealthChecks:GarnetDatabase:TestContainerDegraded:Port", $"{_database.Port}" },
                    { "HealthChecks:GarnetDatabase:TestContainerDegraded:Timeout", "0" },
                    { "HealthChecks:GarnetDatabase:TestContainerDegraded:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddGarnet_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddGarnet("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:GarnetDatabase:TestNoValues:ConnectionString", "" },
                    { "HealthChecks:GarnetDatabase:TestNoValues:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddGarnet_UseConfiguration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddGarnet("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>
                {
                    { "HealthChecks:GarnetDatabase:TestNoValues:Hostname", _database.Hostname },
                    {
                        "HealthChecks:GarnetDatabase:TestNoValues:Port",
                        _database.Port.ToString(CultureInfo.InvariantCulture)
                    },
                    { "HealthChecks:GarnetDatabase:TestNoValues:Timeout", "-2" },
                    { "HealthChecks:GarnetDatabase:TestNoValues:Mode", "Create" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
