namespace NetEvolve.HealthChecks.Http;

using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(HttpOptions), true)]
internal sealed partial class HttpHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        HttpOptions options,
        CancellationToken cancellationToken
    )
    {
        var httpClient = _serviceProvider.GetRequiredService<HttpClient>();
        var httpMethod = new HttpMethod(options.HttpMethod);

        using var request = new HttpRequestMessage(httpMethod, options.Uri);

        // Add headers
        foreach (var header in options.Headers)
        {
            _ = request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // Add content if provided
        if (!string.IsNullOrEmpty(options.Content))
        {
            request.Content = new StringContent(options.Content, Encoding.UTF8, options.ContentType);
        }

        var (isTimelyResponse, response) = await httpClient
            .SendAsync(request, cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        var statusCode = (int)response.StatusCode;

        return options.ExpectedHttpStatusCodes.Contains(statusCode)
            ? HealthCheckState(isTimelyResponse, name)
            : HealthCheckUnhealthy(
                failureStatus,
                name,
                $"Unexpected status code {statusCode}. Expected: {string.Join(", ", options.ExpectedHttpStatusCodes)}"
            );
    }
}
