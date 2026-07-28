namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ServiceBus;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;
using TUnit.Mocks;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}.Subscription")]
public sealed class ServiceBusSubscriptionAdministrationTests
{
    [Test]
    public async Task CheckHealthAsync_WhenAdministrationClient_ShouldCallGetSubscriptionRuntimeProperties()
    {
        // Arrange
        var options = new ServiceBusSubscriptionOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString =
                "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=test",
            TopicName = "test-topic",
            SubscriptionName = "test-subscription",
            EnablePeekMode = false,
        };

        var optionsMonitor = IOptionsMonitor<ServiceBusSubscriptionOptions>.Mock();
        _ = optionsMonitor.Get("test").Returns(options);

        var serviceProvider = IServiceProvider.Mock();
        var healthCheck = new ServiceBusSubscriptionHealthCheck(serviceProvider, optionsMonitor);

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
    public async Task CheckHealthAsync_WhenSubscriptionDoesNotExist_ShouldReturnUnhealthy()
    {
        // Arrange
        var options = new ServiceBusSubscriptionOptions
        {
            Mode = ClientCreationMode.ServiceProvider,
            TopicName = "existing-topic",
            SubscriptionName = "non-existing-subscription",
            EnablePeekMode = false,
        };

        var optionsMonitor = IOptionsMonitor<ServiceBusSubscriptionOptions>.Mock();
        _ = optionsMonitor.Get("test").Returns(options);

        // Create a mock service provider with a mock administration client
        var mockAdminClient = ServiceBusAdministrationClient.Mock();
        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockAdminClient);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new ServiceBusSubscriptionHealthCheck(serviceProvider, optionsMonitor);

        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, null, null),
        };

        // Setup mock to throw exception to simulate subscription not existing
        _ = mockAdminClient
            .GetSubscriptionRuntimePropertiesAsync(Any<string>(), Any<string>(), Any<CancellationToken>())
            .Throws(
                new global::Azure.Messaging.ServiceBus.ServiceBusException(
                    "Subscription not found",
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
        var options = new ServiceBusSubscriptionOptions
        {
            Mode = ClientCreationMode.ServiceProvider,
            TopicName = "timeout-topic",
            SubscriptionName = "timeout-subscription",
            EnablePeekMode = false,
            Timeout = 1, // Very short timeout to force failure
        };

        var optionsMonitor = IOptionsMonitor<ServiceBusSubscriptionOptions>.Mock();
        _ = optionsMonitor.Get("test").Returns(options);

        // Create a mock service provider with a mock administration client
        var mockAdminClient = ServiceBusAdministrationClient.Mock();
        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockAdminClient);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new ServiceBusSubscriptionHealthCheck(serviceProvider, optionsMonitor);

        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, null, null),
        };

        // Setup mock to delay longer than the timeout
        _ = mockAdminClient
            .GetSubscriptionRuntimePropertiesAsync(Any<string>(), Any<string>(), Any<CancellationToken>())
            .ReturnsAsync(async () =>
            {
                await Task.Delay(100); // Delay longer than the timeout
                return global::Azure.Response.FromValue(
                    global::Azure.Messaging.ServiceBus.ServiceBusModelFactory.SubscriptionRuntimeProperties(
                        "timeout-topic",
                        "timeout-subscription"
                    ),
                    global::Azure.Response.Mock()
                );
            });

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
    }
}
