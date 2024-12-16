namespace NetEvolve.HealthChecks.Tests.Integration.HealthChecks;

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class ApplicationReadyCheckTests : HealthCheckTestBase
{
    [Fact]
    public async Task AddApplicationReady_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks => _ = healthChecks.AddApplicationReady());

    [Fact]
    public async Task AddApplicationReady_WithCustomName_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => _ = healthChecks.AddApplicationReady(),
            serverConfiguration: server =>
            {
                var lifetime = server.Services.GetRequiredService<IHostApplicationLifetime>();
                lifetime.StopApplication();
            }
        );
}
