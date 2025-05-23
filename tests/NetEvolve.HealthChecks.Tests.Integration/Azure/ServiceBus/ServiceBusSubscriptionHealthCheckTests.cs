namespace NetEvolve.HealthChecks.Tests.Integration.Azure.ServiceBus;

using Microsoft.Extensions.Azure;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
public class ServiceBusSubscriptionHealthCheckTests : HealthCheckTestBase, IClassFixture<ServiceBusContainer>
{
    private readonly ServiceBusContainer _container;

    public ServiceBusSubscriptionHealthCheckTests(ServiceBusContainer container) => _container = container;

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusSubscriptionHealthCheck_UseOptions_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusSubscriptionHealthCheck(
                    "ServiceBusSubscriptionServiceProviderHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.SubscriptionName = ServiceBusContainer.SubscriptionName;
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

    [Fact]
    public async Task AddServiceBusSubscriptionHealthCheck_UseOptions_EnablePeekModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusSubscriptionHealthCheck(
                    "ServiceBusSubscriptionServiceProviderPeekHealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EnablePeekMode = true;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.SubscriptionName = ServiceBusContainer.SubscriptionName;
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString));
            }
        );

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusSubscriptionHealthCheck_UseOptions_ModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddServiceBusSubscriptionHealthCheck(
                "ServiceBusSubscriptionConnectionStringHealthy",
                options =>
                {
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.ConnectionString = _container.ConnectionString;
                    options.TopicName = ServiceBusContainer.TopicName;
                    options.SubscriptionName = ServiceBusContainer.SubscriptionName;
                }
            );
        });

    [Fact]
    public async Task AddServiceBusSubscriptionHealthCheck_UseOptions_EnablePeekModeConnectionString_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddServiceBusSubscriptionHealthCheck(
                "ServiceBusSubscriptionConnectionStringPeekHealthy",
                options =>
                {
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.ConnectionString = _container.ConnectionString;
                    options.EnablePeekMode = true;
                    options.TopicName = ServiceBusContainer.TopicName;
                    options.SubscriptionName = ServiceBusContainer.SubscriptionName;
                }
            );
        });

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusSubscriptionHealthCheck_UseOptions_ModeServiceProvider_SubscriptionNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusSubscriptionHealthCheck(
                    "ServiceBusSubscriptionServiceProviderUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.SubscriptionName = "nonexistent-subscription";
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

    [Fact]
    public async Task AddServiceBusSubscriptionHealthCheck_UseOptions_EnablePeekModeServiceProvider_SubscriptionNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusSubscriptionHealthCheck(
                    "ServiceBusSubscriptionServiceProviderPeekUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EnablePeekMode = true;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.SubscriptionName = "nonexistent-subscription";
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString));
            }
        );

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusSubscriptionHealthCheck_UseOptions_ModeConnectionString_SubscriptionNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddServiceBusSubscriptionHealthCheck(
                "ServiceBusSubscriptionConnectionStringUnhealthy",
                options =>
                {
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.ConnectionString = _container.ConnectionString;
                    options.TopicName = ServiceBusContainer.TopicName;
                    options.SubscriptionName = "nonexistent-subscription";
                }
            );
        });

    [Fact]
    public async Task AddServiceBusSubscriptionHealthCheck_UseOptions_EnablePeekModeConnectionString_SubscriptionNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddServiceBusSubscriptionHealthCheck(
                "ServiceBusSubscriptionConnectionStringPeekUnhealthy",
                options =>
                {
                    options.Mode = ClientCreationMode.ConnectionString;
                    options.ConnectionString = _container.ConnectionString;
                    options.EnablePeekMode = true;
                    options.TopicName = ServiceBusContainer.TopicName;
                    options.SubscriptionName = "nonexistent-subscription";
                }
            );
        });

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusSubscriptionHealthCheck_UseOptions_ModeServiceProvider_TopicNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusSubscriptionHealthCheck(
                    "ServiceBusSubscriptionTopicNotExistsUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.TopicName = "nonexistent-topic";
                        options.SubscriptionName = ServiceBusContainer.SubscriptionName;
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

    [Fact]
    public async Task AddServiceBusSubscriptionHealthCheck_UseOptions_EnablePeekModeServiceProvider_TopicNotExists_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusSubscriptionHealthCheck(
                    "ServiceBusSubscriptionTopicNotExistsPeekUnhealthy",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EnablePeekMode = true;
                        options.TopicName = "nonexistent-topic";
                        options.SubscriptionName = ServiceBusContainer.SubscriptionName;
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString));
            }
        );

    [Fact(Skip = "Unsupported Client. See https://github.com/Azure/azure-service-bus-emulator-installer/issues/17")]
    public async Task AddServiceBusSubscriptionHealthCheck_UseOptions_ModeServiceProvider_Timeout_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusSubscriptionHealthCheck(
                    "ServiceBusSubscriptionServiceProviderDegraded",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.SubscriptionName = ServiceBusContainer.SubscriptionName;
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

    [Fact]
    public async Task AddServiceBusSubscriptionHealthCheck_UseOptions_EnablePeekModeServiceProvider_Timeout_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusSubscriptionHealthCheck(
                    "ServiceBusSubscriptionServiceProviderPeekDegraded",
                    options =>
                    {
                        options.Mode = ClientCreationMode.ServiceProvider;
                        options.EnablePeekMode = true;
                        options.TopicName = ServiceBusContainer.TopicName;
                        options.SubscriptionName = ServiceBusContainer.SubscriptionName;
                        options.Timeout = 0; // Set timeout to 0 to force timeout
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients => _ = clients.AddServiceBusClient(_container.ConnectionString));
            }
        );
}
