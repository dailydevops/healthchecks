namespace NetEvolve.HealthChecks.Tests.Integration.HealthChecks;

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using NetEvolve.Extensions.TUnit;

[TestGroup(nameof(HealthChecks))]
public class ApplicationReadyCheckTests : HealthCheckTestBase
{
    [Test]
    public async Task AddApplicationReady_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks => healthChecks.AddApplicationReady(), HealthStatus.Healthy);

    [Test]
    public async Task AddApplicationReady_WithCustomName_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddApplicationReady(),
            HealthStatus.Unhealthy,
            serverConfiguration: server =>
            {
                var lifetime = server.Services.GetRequiredService<IHostApplicationLifetime>();
                lifetime.StopApplication();
            }
        );
}
