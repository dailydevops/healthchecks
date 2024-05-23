namespace NetEvolve.HealthChecks.Azure.Queues;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class QueueServiceAvailableHealthCheck
    : ConfigurableHealthCheckBase<QueueServiceAvailableOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public QueueServiceAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<QueueServiceAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        QueueServiceAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var queueClient = ClientCreation.GetQueueServiceClient(name, options, _serviceProvider);

        var queueTask = queueClient
            .GetQueuesAsync(cancellationToken: cancellationToken)
            .AsPages(pageSizeHint: 1)
            .GetAsyncEnumerator(cancellationToken)
            .MoveNextAsync();

        var (isValid, result) = await queueTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid && result, name);
    }
}
