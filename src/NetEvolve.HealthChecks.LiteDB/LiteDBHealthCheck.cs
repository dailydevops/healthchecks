namespace NetEvolve.HealthChecks.LiteDB;

using System.Threading;
using System.Threading.Tasks;
using global::LiteDB;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(LiteDBOptions))]
internal sealed partial class LiteDBHealthCheck
{
    private static async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        LiteDBOptions options,
#pragma warning disable S1172 // Unused method parameters should be removed
        CancellationToken cancellationToken
#pragma warning restore S1172 // Unused method parameters should be removed
    )
    {
        var (isTimelyResponse, collectionExists) = await Task.Run(
                () =>
                {
                    using var db = new LiteDatabase(options.ConnectionString);
                    return db.CollectionExists(options.CollectionName);
                },
                cancellationToken
            )
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!collectionExists)
        {
            return HealthCheckUnhealthy(
                failureStatus,
                name,
                $"Collection '{options.CollectionName}' does not exist in the database."
            );
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
