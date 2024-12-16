namespace NetEvolve.HealthChecks.Tests.Integration.HealthChecks;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using NetEvolve.Extensions.XUnit;

[SetCulture("en-US")]
public class ApplicationHealthyCheckTests : HealthCheckTestBase
{
    [Fact]
    public async Task AddApplicationHealthy_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddApplicationHealthy();
        });
}
