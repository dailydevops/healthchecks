namespace NetEvolve.HealthChecks.GCP.BigQuery;

using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(BigQueryOptions))]
internal sealed partial class BigQueryHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        BigQueryOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<BigQueryClient>()
            : _serviceProvider.GetRequiredKeyedService<BigQueryClient>(options.KeyedService);

        // Use a simple operation to check if the BigQuery service is accessible
        // List datasets is a lightweight operation that validates the client is configured and can communicate with BigQuery
        var (isTimelyResponse, _) = await client
            .ListDatasetsAsync()
            .ReadPageAsync(1, cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }
}
