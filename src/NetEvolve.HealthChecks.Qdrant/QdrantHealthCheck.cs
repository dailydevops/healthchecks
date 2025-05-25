namespace NetEvolve.HealthChecks.Qdrant;

using System.Threading;
using System.Threading.Tasks;
using global::Qdrant.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class QdrantHealthCheck : ConfigurableHealthCheckBase<QdrantOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public QdrantHealthCheck(IOptionsMonitor<QdrantOptions> optionsMonitor, IServiceProvider serviceProvider)
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        QdrantOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<QdrantClient>()
            : _serviceProvider.GetRequiredKeyedService<QdrantClient>(options.KeyedService);

        var (isValid, response) = await client
            .HealthAsync(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(response?.Title))
        {
            return HealthCheckUnhealthy(failureStatus, name);
        }

        return HealthCheckState(isValid, name);
    }
}
