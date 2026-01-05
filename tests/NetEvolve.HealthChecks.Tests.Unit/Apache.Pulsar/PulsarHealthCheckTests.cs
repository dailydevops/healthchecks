namespace NetEvolve.HealthChecks.Tests.Unit.Apache.Pulsar;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Apache.Pulsar;

[TestGroup($"{nameof(Apache)}.{nameof(Pulsar)}")]
public sealed class PulsarHealthCheckTests
{
    private const string TestName = $"{nameof(Apache)}.{nameof(Pulsar)}";

    [Test]
    public void CheckHealthAsync_WhenClientNotRegistered_ThrowsException()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .Configure<PulsarOptions>(TestName, opt => opt.Timeout = 1000);

        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<PulsarOptions>>();

        var healthCheck = new PulsarHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(TestName, healthCheck, HealthStatus.Unhealthy, null),
        };

        _ = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await healthCheck.CheckHealthAsync(context, default)
        );
    }
}
