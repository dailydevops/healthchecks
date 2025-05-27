namespace NetEvolve.HealthChecks.Tests.Unit.RabbitMQ;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.RabbitMQ;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

[TestGroup(nameof(RabbitMQ))]
public sealed class RabbitMQHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WithKeyedService_UsesKeyedService()
    {
        // Arrange
        var options = new RabbitMQOptions { KeyedService = "test-key", Timeout = 100 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<RabbitMQOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Setup connection mock that returns success
        var mockConnection = Substitute.For<IConnection>();
        _ = mockConnection
            .CreateChannelAsync(Arg.Any<CreateChannelOptions>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IChannel>(Substitute.For<IChannel>()));

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddKeyedSingleton<IConnection>("test-key", mockConnection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new RabbitMQHealthCheck(optionsMonitor, serviceProvider);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Equal("test: Healthy", result.Description);
    }

    [Fact]
    public async Task CheckHealthAsync_WithoutKeyedService_UsesDefaultService()
    {
        // Arrange
        var options = new RabbitMQOptions { KeyedService = null, Timeout = 1000 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<RabbitMQOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Setup connection mock that returns success
        var mockConnection = Substitute.For<IConnection>();
        _ = mockConnection
            .CreateChannelAsync(Arg.Any<CreateChannelOptions>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IChannel>(Substitute.For<IChannel>()));

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton<IConnection>(mockConnection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new RabbitMQHealthCheck(optionsMonitor, serviceProvider);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Equal("test: Healthy", result.Description);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenConnectionFails_ReturnsUnhealthy()
    {
        // Arrange
        var options = new RabbitMQOptions { KeyedService = null, Timeout = 1000 };

        var optionsMonitor = Substitute.For<IOptionsMonitor<RabbitMQOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Setup connection mock that throws an exception
        var mockConnection = Substitute.For<IConnection>();
        _ = mockConnection
            .CreateChannelAsync(Arg.Any<CreateChannelOptions>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Connection failed"));

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton<IConnection>(mockConnection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new RabbitMQHealthCheck(optionsMonitor, serviceProvider);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.Equal(HealthStatus.Unhealthy, result.Status),
            () => Assert.Contains("test: Unexpected error.", result.Description, StringComparison.Ordinal),
            () => Assert.NotNull(result.Exception)
        );
    }

    [Fact]
    public async Task CheckHealthAsync_WhenTimeout_ReturnsDegraded()
    {
        // Arrange
        var options = new RabbitMQOptions
        {
            KeyedService = null,
            Timeout = 1, // Very short timeout to force a timeout
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<RabbitMQOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        // Setup connection mock that delays long enough to cause timeout
        var mockConnection = Substitute.For<IConnection>();
        _ = mockConnection
            .CreateChannelAsync(Arg.Any<CreateChannelOptions>(), Arg.Any<CancellationToken>())
            .Returns(async _ =>
            {
                await Task.Delay(50); // Delay to force timeout
                return Substitute.For<IChannel>();
            });

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton<IConnection>(mockConnection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new RabbitMQHealthCheck(optionsMonitor, serviceProvider);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        Assert.Equal(HealthStatus.Degraded, result.Status);
        Assert.Contains("test: Degraded", result.Description, StringComparison.Ordinal);
    }
}
