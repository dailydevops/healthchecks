namespace NetEvolve.HealthChecks.Azure.Tables;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(TableServiceAvailableOptions))]
internal sealed partial class TableServiceAvailableHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        TableServiceAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var tableClient = clientCreation.GetTableServiceClient(name, options, _serviceProvider);

        var (isTimelyResponse, result) = await tableClient
            .QueryAsync(cancellationToken: cancellationToken)
            .GetAsyncEnumerator(cancellationToken)
            .MoveNextAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, $"The Table Service '{name}' is not available.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
