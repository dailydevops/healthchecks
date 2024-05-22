namespace NetEvolve.HealthChecks.Azure.Queues;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class QueueClientAvailableHealthCheck
    : ConfigurableHealthCheckBase<QueueClientAvailableOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public QueueClientAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<QueueClientAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        QueueClientAvailableOptions options,
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

        var container = queueClient.GetQueueClient(options.QueueName);

        var containerExists = await container.ExistsAsync(cancellationToken).ConfigureAwait(false);
        if (!containerExists)
        {
            return HealthCheckResult.Unhealthy(
                $"{name}: Container `{options.QueueName}` does not exist."
            );
        }

        (var containerInTime, _) = await container
            .GetPropertiesAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return isValid && result && containerInTime
            ? HealthCheckResult.Healthy($"{name}: Healthy")
            : HealthCheckResult.Degraded($"{name}: Degraded");
    }
}
