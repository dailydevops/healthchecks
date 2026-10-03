namespace NetEvolve.HealthChecks.Tests.Integration.Azure.EventHubs;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.EventHubs;
using TUnit.Core.Executors;

[TestGroup($"{nameof(Azure)}.{nameof(EventHubs)}")]
[TestGroup("Z02TestGroup")]
[ClassDataSource<EventHubsContainer>(Shared = SharedType.PerClass)]
[Culture("en-US")]
public class EventHubsHealthCheckTests : HealthCheckTestBase
{
    private readonly EventHubsContainer _container;

    public EventHubsHealthCheckTests(EventHubsContainer container) => _container = container;

    [Test]
    public async Task AddAzureEventHubs_UseOptions_ModeConnectionString_Healthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureEventHubs(
                    "EventHubsConnectionStringHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.EventHubName = EventHubsContainer.EventHubName;
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAzureEventHubs_UseOptions_ModeServiceProvider_Healthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureEventHubs(
                    "EventHubsServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EventHubName = EventHubsContainer.EventHubName;
                        options.Timeout = 10000;
                    }
                );
            },
            HealthStatus.Healthy,
            serviceBuilder: services =>
                services.AddAzureClients(clients =>
                    _ = clients.AddEventHubProducerClient(_container.ConnectionString, EventHubsContainer.EventHubName)
                ),
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAzureEventHubs_UseConfiguration_ModeConnectionString_Healthy(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var configValues = new Dictionary<string, string?>
        {
            ["HealthChecks:AzureEventHubs:EventHubsConfigHealthy:Mode"] = "ConnectionString",
            ["HealthChecks:AzureEventHubs:EventHubsConfigHealthy:ConnectionString"] = _container.ConnectionString,
            ["HealthChecks:AzureEventHubs:EventHubsConfigHealthy:EventHubName"] = EventHubsContainer.EventHubName,
            ["HealthChecks:AzureEventHubs:EventHubsConfigHealthy:Timeout"] = "10000",
        };

        await RunAndVerify(
            healthChecks => _ = healthChecks.AddAzureEventHubs("EventHubsConfigHealthy"),
            HealthStatus.Healthy,
            config: configBuilder => configBuilder.AddInMemoryCollection(configValues),
            cancellationToken: cancellationToken
        );
    }

    [Test]
    public async Task AddAzureEventHubs_UseOptions_ModeConnectionString_Unhealthy(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureEventHubs(
                    "EventHubsConnectionStringUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.EventHubName = "invalid";
                        options.Timeout = 1000;
                    }
                );
            },
            HealthStatus.Unhealthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAzureEventHubs_UseOptions_ModeConnectionString_WithCustomTimeout_Degraded(
        CancellationToken cancellationToken = default
    ) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureEventHubs(
                    "EventHubsTimeoutDegraded",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.EventHubName = EventHubsContainer.EventHubName;
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAzureEventHubs_UseOptions_WithTags_Healthy(CancellationToken cancellationToken = default) =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureEventHubs(
                    "EventHubsWithTags",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.ConnectionString = _container.ConnectionString;
                        options.EventHubName = EventHubsContainer.EventHubName;
                        options.Timeout = 10000;
                    },
                    "custom-tag",
                    "integration-test"
                );
            },
            HealthStatus.Healthy,
            cancellationToken: cancellationToken
        );

    [Test]
    public async Task AddAzureEventHubs_UseConfiguration_ModeServiceProvider_Healthy(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var configValues = new Dictionary<string, string?>
        {
            ["HealthChecks:AzureEventHubs:EventHubsServiceProviderConfig:Mode"] = "ServiceProvider",
            ["HealthChecks:AzureEventHubs:EventHubsServiceProviderConfig:EventHubName"] =
                EventHubsContainer.EventHubName,
            ["HealthChecks:AzureEventHubs:EventHubsServiceProviderConfig:Timeout"] = "10000",
        };

        await RunAndVerify(
            healthChecks => _ = healthChecks.AddAzureEventHubs("EventHubsServiceProviderConfig"),
            HealthStatus.Healthy,
            config: configBuilder => configBuilder.AddInMemoryCollection(configValues),
            serviceBuilder: services =>
                services.AddAzureClients(clients =>
                    _ = clients.AddEventHubProducerClient(_container.ConnectionString, EventHubsContainer.EventHubName)
                ),
            cancellationToken: cancellationToken
        );
    }

    [Test]
    public async Task AddAzureEventHubs_UseConfiguration_InvalidEventHubName_Unhealthy(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var configValues = new Dictionary<string, string?>
        {
            ["HealthChecks:AzureEventHubs:EventHubsInvalidName:Mode"] = "ConnectionString",
            ["HealthChecks:AzureEventHubs:EventHubsInvalidName:ConnectionString"] = _container.ConnectionString,
            ["HealthChecks:AzureEventHubs:EventHubsInvalidName:EventHubName"] = "nonexistent-hub",
            ["HealthChecks:AzureEventHubs:EventHubsInvalidName:Timeout"] = "5000",
        };

        await RunAndVerify(
            healthChecks => _ = healthChecks.AddAzureEventHubs("EventHubsInvalidName"),
            HealthStatus.Unhealthy,
            config: configBuilder => configBuilder.AddInMemoryCollection(configValues),
            cancellationToken: cancellationToken
        );
    }
}
