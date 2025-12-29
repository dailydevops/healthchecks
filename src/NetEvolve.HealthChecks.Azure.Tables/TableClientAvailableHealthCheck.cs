namespace NetEvolve.HealthChecks.Azure.Tables;

using System.Threading;
using System.Threading.Tasks;
using global::Azure.Data.Tables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(TableClientAvailableOptions))]
internal sealed partial class TableClientAvailableHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        TableClientAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var tableClient = clientCreation.GetTableServiceClient(name, options, _serviceProvider);

        var (isTimelyResponse, result) = await tableClient
            .QueryAsync(x => x.Name == options.TableName, cancellationToken: cancellationToken)
            .GetAsyncEnumerator(cancellationToken)
            .MoveNextAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(
                failureStatus,
                name,
                $"Table '{options.TableName}' does not exist or is not accessible."
            );
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
