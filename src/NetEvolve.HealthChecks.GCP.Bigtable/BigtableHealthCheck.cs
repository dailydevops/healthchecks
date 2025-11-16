namespace NetEvolve.HealthChecks.GCP.Bigtable;

using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Bigtable.Admin.V2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(BigtableOptions))]
internal sealed partial class BigtableHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
        BigtableOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<BigtableInstanceAdminClient>()
            : _serviceProvider.GetRequiredKeyedService<BigtableInstanceAdminClient>(options.KeyedService);

        var (isTimelyResponse, _) = await client
            .ListInstancesAsync(new ListInstancesRequest { Parent = options.ProjectName }, cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }
}
