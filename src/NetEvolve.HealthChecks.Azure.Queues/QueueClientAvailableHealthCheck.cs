namespace NetEvolve.HealthChecks.Azure.Queues;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(QueueClientAvailableOptions))]
internal sealed partial class QueueClientAvailableHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        QueueClientAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var queueClient = clientCreation.GetQueueServiceClient(name, options, _serviceProvider);

        var (isTimelyResponse, result) = await queueClient
            .GetQueuesAsync(cancellationToken: cancellationToken)
            .AsPages(pageSizeHint: 1)
            .GetAsyncEnumerator(cancellationToken)
            .MoveNextAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Failed to list queues.");
        }

        var queue = queueClient.GetQueueClient(options.QueueName);

        var queueExists = await queue.ExistsAsync(cancellationToken).ConfigureAwait(false);
        if (!queueExists)
        {
            return HealthCheckUnhealthy(failureStatus, name, $"Queue `{options.QueueName}` does not exist.");
        }

        (var queueInTime, _) = await queue
            .GetPropertiesAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse && queueInTime, name);
    }
}
