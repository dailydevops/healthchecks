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
#pragma warning disable RCS1163 // Unused parameter
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus failureStatus,
#pragma warning restore S1172 // Unused method parameters should be removed
#pragma warning restore RCS1163 // Unused parameter
        TableServiceAvailableOptions options,
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

        return HealthCheckState(isValid, name);
    }
}
