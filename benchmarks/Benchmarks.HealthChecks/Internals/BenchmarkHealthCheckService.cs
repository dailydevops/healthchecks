namespace Benchmarks.HealthChecks.Internals;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

internal sealed class BenchmarkHealthCheckService(
    IServiceScopeFactory scopeFactory,
    IOptions<HealthCheckServiceOptions> options
) : HealthCheckService
{
    public override async Task<HealthReport> CheckHealthAsync(
        Func<HealthCheckRegistration, bool>? predicate,
        CancellationToken cancellationToken = default
    )
    {
        var registrations = options.Value.Registrations;
        if (predicate != null)
        {
            registrations = [.. registrations.Where(predicate)];
        }

        var totalTime = Stopwatch.StartNew();

        var tasks = new Task<HealthReportEntry>[registrations.Count];
        var index = 0;

        foreach (var registration in registrations)
        {
            tasks[index++] = Task.Run(() => RunCheckAsync(registration, cancellationToken), cancellationToken);
        }

        _ = await Task.WhenAll(tasks).ConfigureAwait(false);

        index = 0;
        var entries = new Dictionary<string, HealthReportEntry>(StringComparer.OrdinalIgnoreCase);
        foreach (var registration in registrations)
        {
            entries[registration.Name] = await tasks[index++].ConfigureAwait(false);
        }

        return new HealthReport(entries, totalTime.Elapsed);
    }

    private async Task<HealthReportEntry> RunCheckAsync(
        HealthCheckRegistration registration,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var scope = scopeFactory.CreateAsyncScope();
        await using (scope.ConfigureAwait(false))
        {
            var healthCheck = registration.Factory(scope.ServiceProvider);

            var stopwatch = Stopwatch.StartNew();
            var context = new HealthCheckContext { Registration = registration };

            HealthReportEntry entry;
            CancellationTokenSource? timeoutCancellationTokenSource = null;
            try
            {
                HealthCheckResult result;

                var checkCancellationToken = cancellationToken;
                if (registration.Timeout > TimeSpan.Zero)
                {
                    timeoutCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    timeoutCancellationTokenSource.CancelAfter(registration.Timeout);
                    checkCancellationToken = timeoutCancellationTokenSource.Token;
                }

                result = await healthCheck.CheckHealthAsync(context, checkCancellationToken).ConfigureAwait(false);

                entry = new HealthReportEntry(
                    status: result.Status,
                    description: result.Description,
                    duration: stopwatch.Elapsed,
                    exception: result.Exception,
                    data: result.Data,
                    tags: registration.Tags
                );
            }
            catch (Exception ex)
            {
                entry = new HealthReportEntry(
                    status: registration.FailureStatus,
                    description: ex.Message,
                    duration: stopwatch.Elapsed,
                    exception: ex,
                    data: null,
                    tags: registration.Tags
                );
            }
            finally
            {
                timeoutCancellationTokenSource?.Dispose();
            }

            return entry;
        }
    }
}
