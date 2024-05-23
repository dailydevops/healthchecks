namespace NetEvolve.HealthChecks.Abstractions.Tests.Unit;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Xunit;

public class ConfigurableHealthCheckBaseTests
{
    [Fact]
    public async Task InternalAsync_SomethingHappens_ReturnsUnhealthy()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.Configure<TestConfiguration>(options =>
        {
            options.Name = "Hello World";
        });
        _ = services.AddSingleton<IHealthCheck, TestConfigurableHealthCheck>();
        _ = services.AddHealthChecks().AddCheck<TestConfigurableHealthCheck>("test");
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var check = scope.ServiceProvider.GetService<IHealthCheck>();

        Assert.NotNull(check);

        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", check, null, null)
        };

        // Act
        var result = await check.CheckHealthAsync(context, default);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
    }

#pragma warning disable CA1812 // Unused classes
    private sealed class TestConfigurableHealthCheck(
        IOptionsMonitor<TestConfiguration> optionsMonitor
    ) : ConfigurableHealthCheckBase<TestConfiguration>(optionsMonitor)
    {
        protected override ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
            string name,
            HealthStatus failureStatus,
            TestConfiguration options,
            CancellationToken cancellationToken
        ) => throw new NotImplementedException();
    }

    private sealed class TestConfiguration
    {
        public string? Name { get; set; }
    }
#pragma warning restore CA1812 // Unused classes
}
