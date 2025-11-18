namespace NetEvolve.HealthChecks.Azure.Search;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(SearchServiceAvailableOptions))]
internal sealed partial class SearchServiceAvailableHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable RCS1163 // Unused parameter
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
#pragma warning restore RCS1163 // Unused parameter
        SearchServiceAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var searchIndexClient = clientCreation.GetSearchIndexClient(
            name,
            options,
            _serviceProvider
        );

        var (isValid, _) = await searchIndexClient
            .GetIndexNamesAsync(cancellationToken)
            .GetAsyncEnumerator(cancellationToken)
            .MoveNextAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }
}
