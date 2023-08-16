namespace NetEvolve.HealthChecks;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class SelfHealthCheck : HealthCheckBase
{
    protected override ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken
    ) => new ValueTask<HealthCheckResult>(HealthCheckResult.Healthy());
}
