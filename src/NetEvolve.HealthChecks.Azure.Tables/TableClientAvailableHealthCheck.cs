namespace NetEvolve.HealthChecks.Azure.Tables;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::Azure.Data.Tables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class TableClientAvailableHealthCheck : ConfigurableHealthCheckBase<TableClientAvailableOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public TableClientAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<TableClientAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
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
