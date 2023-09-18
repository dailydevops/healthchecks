namespace NetEvolve.HealthChecks.Tests.Integration;

using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks;
using NetEvolve.HealthChecks.Tests;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

[IntegrationTest]
[ExcludeFromCodeCoverage]
[SetCulture]
public class ApplicationHealthyCheckTests : HealthCheckTestBase
{
    [Fact]
    public async Task AddApplicationHealthy_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
        {
            _ = healthChecks.AddApplicationHealthy();
        });
}
