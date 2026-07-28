namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ServiceBus;

using System;
using System.Threading.Tasks;
using global::Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;
using TUnit.Mocks;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}.Queue")]
public sealed class ServiceBusQueueAdministrationTests
{
    [Test]
    public async Task CheckHealthAsync_WhenAdministrationClient_ShouldCallGetQueueRuntimeProperties()
    {
        // Arrange
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString =
                "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=test",
            QueueName = "test-queue",
            EnablePeekMode = false,
        };

        var optionsMonitor = IOptionsMonitor<ServiceBusQueueOptions>.Mock();
        _ = optionsMonitor.Get("test").Returns(options);

        var serviceProvider = IServiceProvider.Mock();
        var healthCheck = new ServiceBusQueueHealthCheck(serviceProvider, optionsMonitor);

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
    public async Task CheckHealthAsync_WhenQueueDoesNotExist_ShouldReturnUnhealthy()
    {
        // Arrange
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.ServiceProvider,
            QueueName = "non-existing-queue",
            EnablePeekMode = false,
        };

        var optionsMonitor = IOptionsMonitor<ServiceBusQueueOptions>.Mock();
        _ = optionsMonitor.Get("test").Returns(options);

        // Create a mock service provider with a mock administration client
        var mockAdminClient = ServiceBusAdministrationClient.Mock();
        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockAdminClient);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new ServiceBusQueueHealthCheck(serviceProvider, optionsMonitor);

        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, null, null),
        };

        // Setup mock to throw exception to simulate queue not existing
        _ = mockAdminClient
            .GetQueueRuntimePropertiesAsync(Any(), Any())
            .Throws(
                new global::Azure.Messaging.ServiceBus.ServiceBusException(
                    "Queue not found",
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
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.ServiceProvider,
            QueueName = "timeout-queue",
            EnablePeekMode = false,
            Timeout = 1, // Very short timeout to force failure
        };

        var optionsMonitor = IOptionsMonitor<ServiceBusQueueOptions>.Mock();
        _ = optionsMonitor.Get("test").Returns(options);

        // Create a mock service provider with a mock administration client
        var mockAdminClient = ServiceBusAdministrationClient.Mock();
        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockAdminClient);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new ServiceBusQueueHealthCheck(serviceProvider, optionsMonitor);

        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, null, null),
        };

        // Setup mock to delay longer than the timeout
        _ = mockAdminClient
            .GetQueueRuntimePropertiesAsync(Any(), Any())
            .ReturnsAsync(async () =>
            {
                await Task.Delay(100); // Delay longer than the timeout
                return global::Azure.Response.FromValue(
                    global::Azure.Messaging.ServiceBus.ServiceBusModelFactory.QueueRuntimeProperties("timeout-queue"),
                    global::Azure.Response.Mock()
                );
            });

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
    }
}
