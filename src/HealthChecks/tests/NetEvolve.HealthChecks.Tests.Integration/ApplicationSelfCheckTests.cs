namespace NetEvolve.HealthChecks.Tests.Integration;

using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks;
using NetEvolve.HealthChecks.Tests;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

[IntegrationTest]
[ExcludeFromCodeCoverage]
public class ApplicationSelfCheckTests : HealthCheckTestBase
{
    [Fact]
    public async Task AddApplicationSelfCheck_ShouldReturnHealthy() =>
        await RunAndVerify(healthChecks =>
            {
                _ = healthChecks.AddApplicationSelfCheck();
            })
            .ConfigureAwait(false);
}
