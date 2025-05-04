namespace NetEvolve.HealthChecks.Tests.Integration.Azure.ServiceBus;

using Microsoft.Extensions.Azure;
using NetEvolve.HealthChecks.Azure.ServiceBus;

public class ServiceBusQueueHealthCheckTests
    : HealthCheckTestBase,
        IClassFixture<ServiceBusContainer>
{
    private readonly ServiceBusContainer _container;

    public ServiceBusQueueHealthCheckTests(ServiceBusContainer container) => _container = container;

    [Fact]
    public async Task AddServiceBusQueueHealthCheck_UseOptions_ModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusQueueHealthCheck(
                    "ServiceBusQueueServiceProviderHealthy",
                    options => options.QueueName = ServiceBusContainer.QueueName
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
    public async Task AddServiceBusQueueHealthCheck_UseOptions_EnablePeekModeServiceProvider_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddServiceBusQueueHealthCheck(
                    "ServiceBusQueueServiceProviderHealthy",
                    options =>
                    {
                        options.EnablePeekMode = true;
                        options.QueueName = ServiceBusContainer.QueueName;
                    }
                );
            },
            serviceBuilder: services =>
            {
                services.AddAzureClients(clients =>
                    _ = clients.AddServiceBusClient(_container.ConnectionString)
                );
            }
        );
}
