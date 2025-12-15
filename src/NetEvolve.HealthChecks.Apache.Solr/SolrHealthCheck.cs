namespace NetEvolve.HealthChecks.Apache.Solr;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SolrNet;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(SolrOptions))]
internal sealed partial class SolrHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        SolrOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = _serviceProvider.GetRequiredService<ISolrBasicReadOnlyOperations<string>>();

        var (isTimelyResponse, response) = await client
            .PingAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (response.Status != 0)
        {
            return HealthCheckUnhealthy(failureStatus, name, $"Solr ping returned non-zero status: {response.Status}.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
