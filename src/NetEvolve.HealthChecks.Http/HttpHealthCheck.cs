namespace NetEvolve.HealthChecks.Http;

using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class HttpHealthCheck : ConfigurableHealthCheckBase<HttpOptions>
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpHealthCheck"/> class.
    /// </summary>
    /// <param name="optionsMonitor">The <see cref="IOptionsMonitor{TOptions}"/> instance used to access named options.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to resolve dependencies.</param>
    public HttpHealthCheck(IOptionsMonitor<HttpOptions> optionsMonitor, IServiceProvider serviceProvider)
        : base(optionsMonitor)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        HttpOptions options,
        CancellationToken cancellationToken
    )
    {
        var httpClient = _serviceProvider.GetRequiredService<HttpClient>();

        try
        {
            using var request = new HttpRequestMessage(new System.Net.Http.HttpMethod(options.HttpMethod), options.Uri);

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

            var (isNotTimedOut, response) = await httpClient
                .SendAsync(request, cancellationToken)
                .WithTimeoutAsync(options.Timeout, cancellationToken)
                .ConfigureAwait(false);

            if (!isNotTimedOut)
            {
                return HealthCheckDegraded(name);
            }

            if (response is null)
            {
                return HealthCheckUnhealthy(failureStatus, name, "No response received.");
            }

            var statusCode = (int)response.StatusCode;
            var isHealthy = options.ExpectedHttpStatusCodes.Contains(statusCode);

            return isHealthy
                ? HealthCheckResult.Healthy($"{name}: Healthy")
                : HealthCheckUnhealthy(
                    failureStatus,
                    name,
                    $"Unexpected status code {statusCode}. Expected: {string.Join(", ", options.ExpectedHttpStatusCodes)}"
                );
        }
        catch (HttpRequestException ex)
        {
            return HealthCheckUnhealthy(failureStatus, name, $"HTTP request failed: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            return HealthCheckDegraded(name);
        }
        catch (Exception ex)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Unexpected error.", ex);
        }
    }
}
