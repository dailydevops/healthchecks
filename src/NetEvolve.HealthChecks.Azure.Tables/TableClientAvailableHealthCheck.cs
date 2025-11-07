namespace NetEvolve.HealthChecks.Azure.Tables;

using System.Threading;
using System.Threading.Tasks;
using global::Azure.Data.Tables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(TableServiceAvailableOptions))]
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

        var (isValid, _) = await tableClient
            .QueryAsync(cancellationToken: cancellationToken)
            .GetAsyncEnumerator(cancellationToken)
            .MoveNextAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        (var tableInTime, _) = await tableClient
            .GetTableClient(options.TableName)
            .QueryAsync<TableEntity>(cancellationToken: cancellationToken)
            .GetAsyncEnumerator(cancellationToken)
            .MoveNextAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid && tableInTime, name);
    }
}
