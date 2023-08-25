namespace NetEvolve.HealthChecks.Tests.Integration;

using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks;
using NetEvolve.HealthChecks.Tests;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

[IntegrationTest]
[ExcludeFromCodeCoverage]
public class ApplicationReadinessCheckTests : HealthCheckTestBase
{
    [Fact]
    public async Task AddApplicationReadinessCheck_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
            {
                _ = healthChecks.AddApplicationReadinessCheck();
            })
            .ConfigureAwait(false);
}
