namespace NetEvolve.HealthChecks.Tests.Integration.Azure.ServiceBus;

using Microsoft.Extensions.Azure;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
public class ServiceBusTopicHealthCheckTests : HealthCheckTestBase, IClassFixture<ServiceBusContainer>
{
    private readonly ServiceBusContainer _container;

    public ServiceBusTopicHealthCheckTests(ServiceBusContainer container) => _container = container;

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusTopicHealthCheck_UseOptions_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusTopicHealthCheck(
                    "ServiceBusTopicServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.TopicName = ServiceBusContainer.TopicName;
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddServiceBusAdministrationClient(_container.ConnectionString)
                );
            }
        );

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusTopicHealthCheck_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddServiceBusTopicHealthCheck(
                "ServiceBusTopicConnectionStringHealthy",
                options =>
                {
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.ConnectionString = _container.ConnectionString;
                    options.TopicName = ServiceBusContainer.TopicName;
                }
            );
        });

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusTopicHealthCheck_UseOptions_ModeServiceProvider_TopicNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusTopicHealthCheck(
                    "ServiceBusTopicServiceProviderUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.TopicName = "nonexistent-topic";
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddServiceBusAdministrationClient(_container.ConnectionString)
                );
            }
        );

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusTopicHealthCheck_UseOptions_ModeConnectionString_TopicNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddServiceBusTopicHealthCheck(
                "ServiceBusTopicConnectionStringUnhealthy",
                options =>
                {
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.ConnectionString = _container.ConnectionString;
                    options.TopicName = "nonexistent-topic";
                }
            );
        });

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusTopicHealthCheck_UseOptions_ModeServiceProvider_Timeout_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusTopicHealthCheck(
                    "ServiceBusTopicServiceProviderDegraded",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.Timeout = 0; // Set timeout to 0 to force timeout
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddServiceBusAdministrationClient(_container.ConnectionString)
                );
            }
        );

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusTopicHealthCheck_UseOptions_ModeConnectionString_Timeout_ShouldReturnDegraded() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddServiceBusTopicHealthCheck(
                "ServiceBusTopicConnectionStringDegraded",
                options =>
                {
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.ConnectionString = _container.ConnectionString;
                    options.TopicName = ServiceBusContainer.TopicName;
                    options.Timeout = 0; // Set timeout to 0 to force timeout
                }
            );
        });

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusTopicHealthCheck_UseOptions_ModeDefaultAzureCredentials_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddServiceBusTopicHealthCheck(
                "ServiceBusTopicDefaultAzureCredentialsUnhealthy",
                options =>
                {
                    options.Mode = ClientCreationMode.DefaultAzureCredentials;
                    options.FullyQualifiedNamespace = "namespace.servicebus.windows.net";
                    options.TopicName = ServiceBusContainer.TopicName;
                }
            );
        });
}
