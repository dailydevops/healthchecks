namespace NetEvolve.HealthChecks.Qdrant;

using System.Threading;
using System.Threading.Tasks;
using global::Qdrant.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(QdrantOptions))]
internal sealed partial class QdrantHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        QdrantOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<QdrantClient>()
            : _serviceProvider.GetRequiredKeyedService<QdrantClient>(options.KeyedService);

        var (isTimelyResponse, response) = await client
            .HealthAsync(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(response?.Title))
        {
            return HealthCheckUnhealthy(failureStatus, name);
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
