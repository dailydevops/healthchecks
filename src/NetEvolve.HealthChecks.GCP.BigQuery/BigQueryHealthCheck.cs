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
        HealthStatus failureStatus,
        BigQueryOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<BigQueryClient>()
            : _serviceProvider.GetRequiredKeyedService<BigQueryClient>(options.KeyedService);

        // Use a simple operation to check if the BigQuery service is accessible
        // List datasets is a lightweight operation that validates the client is configured and can communicate with BigQuery
        var checkTask = Task.Run(
            async () =>
            {
                var datasets = await client
                    .ListDatasetsAsync()
                    .ReadPageAsync(1, cancellationToken)
                    .ConfigureAwait(false);
                return datasets?.GetEnumerator().MoveNext() ?? false;
            },
            cancellationToken
        );

        var (isTimelyResponse, result) = await checkTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, "BigQuery health check failed to retrieve datasets.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
