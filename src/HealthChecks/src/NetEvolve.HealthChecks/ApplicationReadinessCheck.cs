namespace NetEvolve.HealthChecks;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using NetEvolve.Arguments;
using NetEvolve.HealthChecks.Abstractions;
using System.Threading;
using System.Threading.Tasks;

internal sealed class ApplicationReadinessCheck : HealthCheckBase
{
    private bool _applicationReady;

    public ApplicationReadinessCheck(IHostApplicationLifetime lifetime)
    {
        Argument.ThrowIfNull(lifetime);

        _ = lifetime.ApplicationStarted.Register(OnStarted);
        _ = lifetime.ApplicationStopping.Register(OnStopped);
    }

    protected override ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        CancellationToken cancellationToken
    )
    {
        if (cancellationToken.IsCancellationRequested || !_applicationReady)
        {
            return new ValueTask<HealthCheckResult>(
                HealthCheckResult.Unhealthy($"{name}: Unhealthy")
            );
        }

        return new ValueTask<HealthCheckResult>(HealthCheckResult.Healthy($"{name}: Healthy"));
    }

    private void OnStarted() => _applicationReady = true;

    private void OnStopped() => _applicationReady = false;
}
