namespace NetEvolve.HealthChecks.Azure.Tables;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class TableServiceAvailableHealthCheck
    : ConfigurableHealthCheckBase<TableServiceAvailableOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public TableServiceAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<TableServiceAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        TableServiceAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var tableClient = ClientCreation.GetTableServiceClient(name, options, _serviceProvider);

        var (isValid, result) = await tableClient
            .QueryAsync(cancellationToken: cancellationToken)
            .GetAsyncEnumerator(cancellationToken)
            .MoveNextAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid && result, name);
    }
}
