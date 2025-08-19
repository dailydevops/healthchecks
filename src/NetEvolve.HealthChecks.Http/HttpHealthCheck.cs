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
        using var request = new HttpRequestMessage(new HttpMethod(options.HttpMethod), options.Uri);

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
        var isHealthy = options.ExpectedHttpStatusCodes.Contains(statusCode);

        return isHealthy
            ? HealthCheckState(isTimelyResponse, name)
            : HealthCheckUnhealthy(
                failureStatus,
                name,
                $"Unexpected status code {statusCode}. Expected: {string.Join(", ", options.ExpectedHttpStatusCodes)}"
            );
    }
}
