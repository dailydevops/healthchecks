namespace NetEvolve.HealthChecks.Tests.Unit.Abstractions;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Abstractions;
using Xunit;

[TestGroup(nameof(HealthChecks))]
public class ConfigurableHealthCheckBaseTests
{
    [Fact]
    public async Task InternalAsync_SomethingHappens_ReturnsUnhealthy()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.Configure<TestConfiguration>(options => options.Name = "Hello World");
        _ = services.AddSingleton<IHealthCheck, TestConfigurableHealthCheck>();
        _ = services.AddHealthChecks().AddCheck<TestConfigurableHealthCheck>("test");
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var check = scope.ServiceProvider.GetService<IHealthCheck>();

        Assert.NotNull(check);

        var context = new HealthCheckContext { Registration = new HealthCheckRegistration("test", check, null, null) };

        // Act
        var result = await check.CheckHealthAsync(context, default);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
    }

    [Fact]
    public void HealthCheckState_WhenConditionIsTrue_ReturnsHealthy()
    {
        // Arrange
        var testName = "TestHealthCheck";

        // Act
        var result = TestHelperConfigurableHealthCheck.TestHealthCheckState(true, testName);

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Equal($"{testName}: Healthy", result.Description);
    }

    [Fact]
    public void HealthCheckState_WhenConditionIsFalse_ReturnsDegraded()
    {
        // Arrange
        var testName = "TestHealthCheck";

        // Act
        var result = TestHelperConfigurableHealthCheck.TestHealthCheckState(false, testName);

        // Assert
        Assert.Equal(HealthStatus.Degraded, result.Status);
        Assert.Equal($"{testName}: Degraded", result.Description);
    }

    [Fact]
    public void HealthCheckUnhealthy_WithDefaultMessage_ReturnsUnhealthyWithDefaultMessage()
    {
        // Arrange
        var testName = "TestHealthCheck";
        var failureStatus = HealthStatus.Unhealthy;

        // Act
        var result = TestHelperConfigurableHealthCheck.TestHealthCheckUnhealthy(failureStatus, testName);

        // Assert
        Assert.Equal(failureStatus, result.Status);
        Assert.Equal($"{testName}: Unhealthy", result.Description);
    }

    [Fact]
    public void HealthCheckUnhealthy_WithCustomMessage_ReturnsUnhealthyWithCustomMessage()
    {
        // Arrange
        var testName = "TestHealthCheck";
        var failureStatus = HealthStatus.Degraded;
        var customMessage = "Custom unhealthy message";

        // Act
        var result = TestHelperConfigurableHealthCheck.TestHealthCheckUnhealthy(failureStatus, testName, customMessage);

        // Assert
        Assert.Equal(failureStatus, result.Status);
        Assert.Equal($"{testName}: {customMessage}", result.Description);
    }

    [Fact]
    public void HealthCheckDegraded_ReturnsHealthCheckResultWithDegradedStatus()
    {
        // Arrange
        var testName = "TestHealthCheck";

        // Act
        var result = TestHelperConfigurableHealthCheck.TestHealthCheckDegraded(testName);

        // Assert
        Assert.Equal(HealthStatus.Degraded, result.Status);
        Assert.Equal($"{testName}: Degraded", result.Description);
    }

#pragma warning disable CA1812 // Unused classes
    private sealed class TestConfigurableHealthCheck(IOptionsMonitor<TestConfiguration> optionsMonitor)
        : ConfigurableHealthCheckBase<TestConfiguration>(optionsMonitor)
    {
        protected override ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
            string name,
            HealthStatus failureStatus,
            TestConfiguration options,
            CancellationToken cancellationToken
        ) => throw new NotImplementedException();
    }

    private sealed class TestHelperConfigurableHealthCheck(IOptionsMonitor<TestConfiguration> optionsMonitor)
        : ConfigurableHealthCheckBase<TestConfiguration>(optionsMonitor)
    {
        protected override ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
            string name,
            HealthStatus failureStatus,
            TestConfiguration options,
            CancellationToken cancellationToken
        ) => new(HealthCheckResult.Healthy());

        public static HealthCheckResult TestHealthCheckState(bool condition, string name) =>
            HealthCheckState(condition, name);

        public static HealthCheckResult TestHealthCheckUnhealthy(
            HealthStatus failureStatus,
            string name,
            string message = "Unhealthy"
        ) => HealthCheckUnhealthy(failureStatus, name, message);

        public static HealthCheckResult TestHealthCheckDegraded(string name) => HealthCheckDegraded(name);
    }

    private sealed record TestConfiguration
    {
        public string? Name { get; set; }
    }
#pragma warning restore CA1812 // Unused classes
}
