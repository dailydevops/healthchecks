namespace NetEvolve.HealthChecks;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using NetEvolve.HealthChecks.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

internal sealed class ApplicationReadinessCheck : HealthCheckBase
{
    private readonly IHostApplicationLifetime _lifetime;

    private bool _applicationReady;

    public ApplicationReadinessCheck(IHostApplicationLifetime lifetime)
    {
        ArgumentNullException.ThrowIfNull(lifetime);

        _lifetime = lifetime;

        _ = _lifetime.ApplicationStarted.Register(OnStarted);
        _ = _lifetime.ApplicationStopping.Register(OnStopped);
    }

    protected override ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthCheckContext context,
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
