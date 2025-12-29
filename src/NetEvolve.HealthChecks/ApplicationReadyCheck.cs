namespace NetEvolve.HealthChecks;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

internal sealed class ApplicationReadyCheck : IHealthCheck
{
    private bool _applicationReady;

    public ApplicationReadyCheck(IHostApplicationLifetime lifetime)
    {
        ArgumentNullException.ThrowIfNull(lifetime);

        _ = lifetime.ApplicationStarted.Register(OnStarted);
        _ = lifetime.ApplicationStopping.Register(OnStopped);
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(context);

        if (cancellationToken.IsCancellationRequested || !_applicationReady)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("ApplicationReady: Unhealthy"));
        }

        return Task.FromResult(HealthCheckResult.Healthy("ApplicationReady: Healthy"));
    }

    private void OnStarted() => _applicationReady = true;

    private void OnStopped() => _applicationReady = false;
}
