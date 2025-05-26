namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ServiceBus;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using global::Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;
using NSubstitute;
using Xunit;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
public sealed class ServiceBusSubscriptionAdministrationTests
{
    [Fact]
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

        var optionsMonitor = Substitute.For<IOptionsMonitor<ServiceBusSubscriptionOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        var serviceProvider = Substitute.For<IServiceProvider>();
        var healthCheck = new ServiceBusSubscriptionHealthCheck(optionsMonitor, serviceProvider);

        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, null, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
    }

    [Fact]
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

        var optionsMonitor = Substitute.For<IOptionsMonitor<ServiceBusSubscriptionOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Create a mock service provider with a mock administration client
        var mockAdminClient = Substitute.For<ServiceBusAdministrationClient>();
        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockAdminClient);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new ServiceBusSubscriptionHealthCheck(optionsMonitor, serviceProvider);

        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, null, null),
        };

        // Setup mock to throw exception to simulate subscription not existing
        _ = mockAdminClient
            .GetSubscriptionRuntimePropertiesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns<global::Azure.Response<SubscriptionRuntimeProperties>>(_ =>
            {
                throw new global::Azure.Messaging.ServiceBus.ServiceBusException(
                    "Subscription not found",
                    global::Azure.Messaging.ServiceBus.ServiceBusFailureReason.MessagingEntityNotFound
                );
            });

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
    }

    [Fact]
    [SuppressMessage(
        "Substitute creation",
        "NS2001:Could not find accessible constructor.",
        Justification = "Reviewed"
    )]
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

        var optionsMonitor = Substitute.For<IOptionsMonitor<ServiceBusSubscriptionOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Create a mock service provider with a mock administration client
        var mockAdminClient = Substitute.For<ServiceBusAdministrationClient>();
        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockAdminClient);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new ServiceBusSubscriptionHealthCheck(optionsMonitor, serviceProvider);

        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, null, null),
        };

        // Setup mock to delay longer than the timeout
        _ = mockAdminClient
            .GetSubscriptionRuntimePropertiesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(async _ =>
            {
                await Task.Delay(100); // Delay longer than the timeout
                return global::Azure.Response.FromValue(
                    Substitute.For<SubscriptionRuntimeProperties>(),
                    Substitute.For<global::Azure.Response>()
                );
            });

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
    }
}
