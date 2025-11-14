namespace NetEvolve.HealthChecks.Tests.Integration.HealthChecks;

using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;

[TestGroup(nameof(HealthChecks))]
[TestGroup("Z03TestGroup")]
public class ApplicationHealthyCheckTests : HealthCheckTestBase
{
    [Test]
    public async Task AddApplicationHealthy_Healthy() =>
        await RunAndVerify(healthChecks => healthChecks.AddApplicationHealthy(), HealthStatus.Healthy);
}
