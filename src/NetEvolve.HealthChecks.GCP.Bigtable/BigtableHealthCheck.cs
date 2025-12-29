namespace NetEvolve.HealthChecks.GCP.Bigtable;

using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(BigtableOptions))]
internal sealed partial class BigtableHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        BigtableOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<BigtableTableAdminClient>()
            : _serviceProvider.GetRequiredKeyedService<BigtableTableAdminClient>(options.KeyedService);

        var listTablesTask = Task.Run(
            async () =>
            {
                var result = client.ListTablesAsync(options.ProjectName);
                var pages = await result.ReadPageAsync(1, cancellationToken).ConfigureAwait(false);
                return pages?.GetEnumerator().MoveNext() ?? false;
            },
            cancellationToken
        );

        var (isTimelyResponse, result) = await listTablesTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(
                failureStatus,
                name,
                $"Bigtable health check failed for project '{options.ProjectName}'."
            );
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
