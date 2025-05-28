namespace NetEvolve.HealthChecks.Tests.Integration.HealthChecks;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.HealthChecks.Abstractions;

public class HealthCheckBaseTests : HealthCheckTestBase
{
    [Fact]
    public async Task ExecuteHealthCheckAsync_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
                healthChecks.Add(
                    new HealthCheckRegistration(
                        "NotImplementedException",
                        new TestHealthCheck(),
                        HealthStatus.Unhealthy,
                        null
                    )
                ),
            HealthStatus.Unhealthy
        );

    private sealed class TestHealthCheck : HealthCheckBase
    {
        protected override ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
            string name,
            HealthStatus failureStatus,
            CancellationToken cancellationToken
        ) => throw new NotImplementedException();
    }
}
