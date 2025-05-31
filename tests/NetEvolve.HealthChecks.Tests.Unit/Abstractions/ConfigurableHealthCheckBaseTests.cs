namespace NetEvolve.HealthChecks.Tests.Unit.Abstractions;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Abstractions;

[TestGroup(nameof(HealthChecks))]
public class ConfigurableHealthCheckBaseTests
{
    [Test]
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
        var context = new HealthCheckContext { Registration = new HealthCheckRegistration("test", check, null, null) };

        // Act
        var result = await check.CheckHealthAsync(context, default);

        // Assert
        _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
    }

    [Test]
    public async Task HealthCheckState_WhenConditionIsTrue_ReturnsHealthy()
    {
        // Arrange
        const string testName = "TestHealthCheck";

        // Act
        var result = TestHelperConfigurableHealthCheck.TestHealthCheckState(true, testName);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{testName}: Healthy", StringComparison.Ordinal);
        }
    }

    [Test]
    public async Task HealthCheckState_WhenConditionIsFalse_ReturnsDegraded()
    {
        // Arrange
        const string testName = "TestHealthCheck";

        // Act
        var result = TestHelperConfigurableHealthCheck.TestHealthCheckState(false, testName);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Degraded);
            _ = await Assert.That(result.Description).IsEqualTo($"{testName}: Degraded", StringComparison.Ordinal);
        }
    }

    [Test]
    public async Task HealthCheckUnhealthy_WithDefaultMessage_ReturnsUnhealthyWithDefaultMessage()
    {
        // Arrange
        const string testName = "TestHealthCheck";
        const HealthStatus failureStatus = HealthStatus.Unhealthy;

        // Act
        var result = TestHelperConfigurableHealthCheck.TestHealthCheckUnhealthy(failureStatus, testName);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).IsEqualTo($"{testName}: Unhealthy", StringComparison.Ordinal);
        }
    }

    [Test]
    public async Task HealthCheckUnhealthy_WithCustomMessage_ReturnsUnhealthyWithCustomMessage()
    {
        // Arrange
        const string testName = "TestHealthCheck";
        const string customMessage = "Custom unhealthy message";

        // Act
        var result = TestHelperConfigurableHealthCheck.TestHealthCheckUnhealthy(
            HealthStatus.Degraded,
            testName,
            customMessage
        );

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Degraded);
            _ = await Assert
                .That(result.Description)
                .IsEqualTo($"{testName}: {customMessage}", StringComparison.Ordinal);
        }
    }

    [Test]
    public async Task HealthCheckDegraded_ReturnsHealthCheckResultWithDegradedStatus()
    {
        // Arrange
        const string testName = "TestHealthCheck";

        // Act
        var result = TestHelperConfigurableHealthCheck.TestHealthCheckDegraded(testName);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Degraded);
            _ = await Assert.That(result.Description).IsEqualTo($"{testName}: Degraded", StringComparison.Ordinal);
        }
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
