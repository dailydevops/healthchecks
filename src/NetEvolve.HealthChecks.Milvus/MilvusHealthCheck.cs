namespace NetEvolve.HealthChecks.Milvus;

using System.Threading;
using System.Threading.Tasks;
using global::Milvus.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(MilvusOptions))]
internal sealed partial class MilvusHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        MilvusOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<MilvusClient>()
            : _serviceProvider.GetRequiredKeyedService<MilvusClient>(options.KeyedService);

        var (isTimelyResponse, response) = await client
            .HealthAsync(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsHealthy)
        {
            return HealthCheckUnhealthy(failureStatus, name, response.ErrorMsg);
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
