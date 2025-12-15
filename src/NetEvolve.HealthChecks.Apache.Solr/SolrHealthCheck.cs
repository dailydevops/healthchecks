namespace NetEvolve.HealthChecks.Apache.Solr;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
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
        var httpClient = _serviceProvider.GetRequiredService<HttpClient>();

        var baseUrlString = options.BaseUrl.ToString().TrimEnd('/');
        var pingUrl = new Uri($"{baseUrlString}/solr/{options.Core}/admin/ping");

        try
        {
            var (isTimelyResponse, response) = await httpClient
                .GetAsync(pingUrl, cancellationToken)
                .WithTimeoutAsync(options.Timeout, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return HealthCheckUnhealthy(
                    failureStatus,
                    name,
                    $"Solr returned status code {response.StatusCode}."
                );
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!content.Contains("\"status\":\"OK\"", StringComparison.OrdinalIgnoreCase))
            {
                return HealthCheckUnhealthy(failureStatus, name, "Solr ping response did not indicate OK status.");
            }

            return HealthCheckState(isTimelyResponse, name);
        }
        catch (HttpRequestException ex)
        {
            return HealthCheckUnhealthy(failureStatus, name, $"HTTP request failed: {ex.Message}");
        }
    }
}
