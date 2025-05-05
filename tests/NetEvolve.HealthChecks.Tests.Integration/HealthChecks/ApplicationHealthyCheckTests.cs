namespace NetEvolve.HealthChecks.Tests.Integration.HealthChecks;

using System.Threading.Tasks;
using NetEvolve.Extensions.XUnit;

[TestGroup(nameof(HealthChecks))]
public class ApplicationHealthyCheckTests : HealthCheckTestBase
{
    [Fact]
    public async Task AddApplicationHealthy_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks => _ = healthChecks.AddApplicationHealthy());
}
