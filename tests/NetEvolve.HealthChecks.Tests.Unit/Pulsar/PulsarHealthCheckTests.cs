namespace NetEvolve.HealthChecks.Tests.Unit.Pulsar;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Pulsar;

[TestGroup(nameof(Pulsar))]
public sealed class PulsarHealthCheckTests
{
    [Test]
    public void CheckHealthAsync_WhenClientNotRegistered_ThrowsException()
    {
        var options = new PulsarOptions { KeyedService = null, Timeout = 1000 };

        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        _ = services.AddSingleton<IConfiguration>(configuration);
        _ = services.Configure<PulsarOptions>(
            "test",
            opt =>
            {
                opt.Timeout = options.Timeout;
            }
        );

        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<PulsarOptions>>();

        var healthCheck = new PulsarHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, HealthStatus.Unhealthy, null),
        };

        _ = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await healthCheck.CheckHealthAsync(context, default)
        );
    }
}
