namespace NetEvolve.HealthChecks.Tests.Integration.Azure.EventHubs;

using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.EventHubs;

[TestGroup($"{nameof(Azure)}.{nameof(EventHubs)}")]
[ClassDataSource<EventHubsContainer>(Shared = InstanceSharedType.AzureEventHubs)]
public class EventHubsHealthCheckTests : HealthCheckTestBase
{
    private readonly EventHubsContainer _container;

    public EventHubsHealthCheckTests(EventHubsContainer container) => _container = container;

    [Test]
    public async Task AddAzureEventHubs_UseOptions_ModeConnectionString_Healthy() =>
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
            HealthStatus.Healthy
        );

    [Test]
    public async Task AddAzureEventHubs_UseOptions_ModeServiceProvider_Healthy() =>
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
                )
        );

    [Test]
    public async Task AddAzureEventHubs_UseConfiguration_ModeConnectionString_Healthy()
    {
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
            config: configBuilder => configBuilder.AddInMemoryCollection(configValues)
        );
    }

    [Test]
    public async Task AddAzureEventHubs_UseOptions_ModeConnectionString_Unhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddAzureEventHubs(
                    "EventHubsConnectionStringUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ConnectionString;
                        options.ConnectionString =
                            "Endpoint=sb://invalid.servicebus.windows.net/;SharedAccessKeyName=invalid;SharedAccessKey=invalid";
                        options.EventHubName = "invalid";
                        options.Timeout = 1000;
                    }
                );
            },
            HealthStatus.Unhealthy
        );
}
