namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ServiceBus;

using System.Threading.Tasks;
using global::Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;
using TUnit.Mocks;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}.Topic")]
public sealed class ServiceBusTopicAdministrationTests
{
    [Test]
    public async Task CheckHealthAsync_WhenAdministrationClient_ShouldCallGetTopicRuntimeProperties()
    {
        // Arrange
        var options = new ServiceBusTopicOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString =
                "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=test",
            TopicName = "test-topic",
        };

        var optionsMonitor = IOptionsMonitor<ServiceBusTopicOptions>.Mock();
        _ = optionsMonitor.Get("test").Returns(options);

        var serviceProvider = IServiceProvider.Mock();
        var healthCheck = new ServiceBusTopicHealthCheck(serviceProvider, optionsMonitor);

        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, null, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
    }

    [Test]
    public async Task CheckHealthAsync_WhenTopicDoesNotExist_ShouldReturnUnhealthy()
    {
        // Arrange
        var options = new ServiceBusTopicOptions
        {
            Mode = ClientCreationMode.ServiceProvider,
            TopicName = "non-existing-topic",
        };

        var optionsMonitor = IOptionsMonitor<ServiceBusTopicOptions>.Mock();
        _ = optionsMonitor.Get("test").Returns(options);

        // Create a mock service provider with a mock administration client
        var mockAdminClient = ServiceBusAdministrationClient.Mock();
        ServiceBusAdministrationClient registeredAdminClient = mockAdminClient;
        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(registeredAdminClient);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new ServiceBusTopicHealthCheck(serviceProvider, optionsMonitor);

        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, null, null),
        };

        // Setup mock to throw exception to simulate topic not existing
        _ = mockAdminClient
            .GetTopicRuntimePropertiesAsync(Any(), Any())
            .Throws(
                new global::Azure.Messaging.ServiceBus.ServiceBusException(
                    "Topic not found",
                    global::Azure.Messaging.ServiceBus.ServiceBusFailureReason.MessagingEntityNotFound
                )
            );

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
    }

    [Test]
    public async Task CheckHealthAsync_WhenTimeout_ShouldReturnUnhealthy()
    {
        // Arrange
        var options = new ServiceBusTopicOptions
        {
            Mode = ClientCreationMode.ServiceProvider,
            TopicName = "timeout-topic",
            Timeout = 1, // Very short timeout to force failure
        };

        var optionsMonitor = IOptionsMonitor<ServiceBusTopicOptions>.Mock();
        _ = optionsMonitor.Get("test").Returns(options);

        // Create a mock service provider with a mock administration client
        var mockAdminClient = ServiceBusAdministrationClient.Mock();
        ServiceBusAdministrationClient registeredAdminClient = mockAdminClient;
        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(registeredAdminClient);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new ServiceBusTopicHealthCheck(serviceProvider, optionsMonitor);

        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, null, null),
        };

        // Setup mock to delay longer than the timeout
        _ = mockAdminClient
            .GetTopicRuntimePropertiesAsync(Any(), Any())
            .ReturnsAsync(async () =>
            {
                await Task.Delay(100); // Delay longer than the timeout
                return global::Azure.Response.FromValue(
                    global::Azure.Messaging.ServiceBus.ServiceBusModelFactory.TopicRuntimeProperties("timeout-topic"),
                    global::Azure.Response.Mock()
                );
            });

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
    }
}
