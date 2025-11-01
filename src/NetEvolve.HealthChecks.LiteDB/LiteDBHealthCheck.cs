namespace NetEvolve.HealthChecks.LiteDB;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using global::LiteDB;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class LiteDBHealthCheck(IOptionsMonitor<LiteDBOptions> optionsMonitor)
    : ConfigurableHealthCheckBase<LiteDBOptions>(optionsMonitor)
{
    protected override ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        LiteDBOptions options,
        CancellationToken cancellationToken
    )
    {
        using var db = new LiteDatabase(options.ConnectionString);

        var sw = Stopwatch.StartNew();
        var collectionExists = db.CollectionExists(options.CollectionName);
        sw.Stop();
        var isTimelyResponse = options.Timeout >= sw.Elapsed.TotalMilliseconds;

        if (!collectionExists)
        {
            return ValueTask.FromResult(
                HealthCheckUnhealthy(
                    failureStatus,
                    name,
                    $"Collection '{options.CollectionName}' does not exist in the database."
                )
            );
        }

        return ValueTask.FromResult(HealthCheckState(isTimelyResponse, name));
    }
}
