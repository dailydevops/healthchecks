namespace NetEvolve.HealthChecks.Tests.Integration.HealthChecks;

using System.Threading.Tasks;

public class ApplicationHealthyCheckTests : HealthCheckTestBase
{
    [Fact]
    public async Task AddApplicationHealthy_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks => _ = healthChecks.AddApplicationHealthy());
}
