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
[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}.Topic")]
public sealed class ServiceBusTopicAdministrationTests
{
    [Fact]
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

        var optionsMonitor = Substitute.For<IOptionsMonitor<ServiceBusTopicOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        var serviceProvider = Substitute.For<IServiceProvider>();
        var healthCheck = new ServiceBusTopicHealthCheck(optionsMonitor, serviceProvider);

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
    public async Task CheckHealthAsync_WhenTopicDoesNotExist_ShouldReturnUnhealthy()
    {
        // Arrange
        var options = new ServiceBusTopicOptions
        {
            Mode = ClientCreationMode.ServiceProvider,
            TopicName = "non-existing-topic",
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<ServiceBusTopicOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Create a mock service provider with a mock administration client
        var mockAdminClient = Substitute.For<ServiceBusAdministrationClient>();
        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockAdminClient);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new ServiceBusTopicHealthCheck(optionsMonitor, serviceProvider);

        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, null, null),
        };

        // Setup mock to throw exception to simulate topic not existing
        _ = mockAdminClient
            .GetTopicRuntimePropertiesAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns<global::Azure.Response<TopicRuntimeProperties>>(_ =>
            {
                throw new global::Azure.Messaging.ServiceBus.ServiceBusException(
                    "Topic not found",
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
        var options = new ServiceBusTopicOptions
        {
            Mode = ClientCreationMode.ServiceProvider,
            TopicName = "timeout-topic",
            Timeout = 1, // Very short timeout to force failure
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<ServiceBusTopicOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Create a mock service provider with a mock administration client
        var mockAdminClient = Substitute.For<ServiceBusAdministrationClient>();
        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(mockAdminClient);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new ServiceBusTopicHealthCheck(optionsMonitor, serviceProvider);

        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, null, null),
        };

        // Setup mock to delay longer than the timeout
        _ = mockAdminClient
            .GetTopicRuntimePropertiesAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(async _ =>
            {
                await Task.Delay(100); // Delay longer than the timeout
                return global::Azure.Response.FromValue(
                    Substitute.For<TopicRuntimeProperties>(),
                    Substitute.For<global::Azure.Response>()
                );
            });

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
    }
}
