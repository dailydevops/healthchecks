namespace NetEvolve.HealthChecks.QuestDB;

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(QuestDBOptions))]
internal sealed partial class QuestDBHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        QuestDBOptions options,
        CancellationToken cancellationToken
    )
    {
        var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();

        var httpClient = httpClientFactory.CreateClient(name);
        using var request = new HttpRequestMessage(HttpMethod.Get, options.StatusUri);

        var (isTimelyResponse, response) = await httpClient
            .SendAsync(request, cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        using (response)
        {
            return response.IsSuccessStatusCode
                ? HealthCheckState(isTimelyResponse, name)
                : HealthCheckUnhealthy(
                    failureStatus,
                    name,
                    $"Unexpected status code {response.StatusCode}. Expected: between 200 and 300"
                );
        }
    }
}
