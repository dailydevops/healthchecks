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
        var httpClient = _serviceProvider.GetRequiredService<HttpClient>();

        var httpMethod = new HttpMethod("GET");
        using var request = new HttpRequestMessage(httpMethod, options.StatusUri);

        var (isTimelyResponse, response) = await httpClient
            .SendAsync(request, cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        var statusCode = (int)response.StatusCode;

        return statusCode is >= 200 and < 300
            ? HealthCheckState(isTimelyResponse, name)
            : HealthCheckUnhealthy(
                failureStatus,
                name,
                $"Unexpected status code {statusCode}. Expected: between 200 and 300"
            );
    }
}
