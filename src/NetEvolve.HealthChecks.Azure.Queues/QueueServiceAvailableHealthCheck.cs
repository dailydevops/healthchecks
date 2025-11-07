namespace NetEvolve.HealthChecks.Azure.Queues;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(QueueServiceAvailableOptions))]
internal sealed partial class QueueServiceAvailableHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        QueueServiceAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var queueClient = clientCreation.GetQueueServiceClient(name, options, _serviceProvider);

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
