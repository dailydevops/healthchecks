namespace NetEvolve.HealthChecks;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

internal sealed partial class ApplicationReadyCheck : IHealthCheck
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

        var failureStatus = context.Registration.FailureStatus;

        try
        {
            if (cancellationToken.IsCancellationRequested || !_applicationReady)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("ApplicationReady: Unhealthy"));
            }

            return Task.FromResult(HealthCheckResult.Healthy("ApplicationReady: Healthy"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                new HealthCheckResult(failureStatus, description: "ApplicationReady: Unexpected error.", exception: ex)
            );
        }
    }

    private void OnStarted() => _applicationReady = true;

    private void OnStopped() => _applicationReady = false;
}
