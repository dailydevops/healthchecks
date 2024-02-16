namespace NetEvolve.HealthChecks.Tests.Integration;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks;
using NetEvolve.HealthChecks.Tests;
using Xunit;

[IntegrationTest]
[ExcludeFromCodeCoverage]
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
