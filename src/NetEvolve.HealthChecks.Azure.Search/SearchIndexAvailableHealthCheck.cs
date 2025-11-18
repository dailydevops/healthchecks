namespace NetEvolve.HealthChecks.Azure.Search;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(SearchIndexAvailableOptions))]
internal sealed partial class SearchIndexAvailableHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable RCS1163 // Unused parameter
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
#pragma warning restore RCS1163 // Unused parameter
        SearchIndexAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(options.IndexName))
        {
            return HealthCheckResult.Unhealthy(
                $"Health check '{name}' failed: IndexName is required."
            );
        }

        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var searchClient = clientCreation.GetSearchClient(name, options, _serviceProvider);

        var (isValid, result) = await searchClient
            .GetDocumentCountAsync(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid && result >= 0, name);
    }
}
