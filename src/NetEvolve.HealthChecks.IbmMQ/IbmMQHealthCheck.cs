namespace NetEvolve.HealthChecks.IbmMQ;

using System.Threading;
using System.Threading.Tasks;
using IBM.WMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

/// <summary>
/// Health check implementation for IBM MQ, using the <c>IBMMQDotnetClient</c> package.
/// </summary>
[ConfigurableHealthCheck(typeof(IbmMQOptions))]
internal sealed partial class IbmMQHealthCheck
{
    /// <inheritdoc />
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        IbmMQOptions options,
        CancellationToken cancellationToken
    )
    {
        var queueManager = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<MQQueueManager>()
            : _serviceProvider.GetRequiredKeyedService<MQQueueManager>(options.KeyedService);

        var (isTimelyResponse, isConnected) = await Task.Run(
                () =>
                {
                    if (!queueManager.IsConnected)
                    {
                        queueManager.Connect();
                    }

                    return queueManager.IsConnected;
                },
                cancellationToken
            )
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!isConnected)
        {
            return HealthCheckUnhealthy(failureStatus, name, "IBM MQ Queue Manager is not connected.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
