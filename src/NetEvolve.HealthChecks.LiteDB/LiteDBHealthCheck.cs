namespace NetEvolve.HealthChecks.LiteDB;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using global::LiteDB;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(LiteDBOptions))]
internal sealed partial class LiteDBHealthCheck
{
    private static ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        LiteDBOptions options,
#pragma warning disable S1172 // Unused method parameters should be removed
        CancellationToken cancellationToken
#pragma warning restore S1172 // Unused method parameters should be removed
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
