namespace NetEvolve.HealthChecks.Apache.RocketMQ;

using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using Org.Apache.Rocketmq;
using SourceGenerator.Attributes;

/// <summary>
/// Health check implementation for Apache RocketMQ, using the <c>RocketMQ.Client</c> package.
/// </summary>
[ConfigurableHealthCheck(typeof(RocketMQOptions))]
internal sealed partial class RocketMQHealthCheck
{
    private readonly ConcurrentDictionary<string, ClientConfig> _clientConfigs = new(
        StringComparer.OrdinalIgnoreCase
    );

    /// <inheritdoc />
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        RocketMQOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientConfig = _clientConfigs.GetOrAdd(name, _ => BuildClientConfig(options));

        var (isTimelyResponse, sendReceipt) = await BuildAndSendAsync(
                clientConfig,
                options.Topic!,
                cancellationToken
            )
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (sendReceipt is null)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Failed to send health check message to RocketMQ.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    private static ClientConfig BuildClientConfig(RocketMQOptions options)
    {
        var builder = new ClientConfig.Builder().SetEndpoints(options.Endpoint!);

        if (!options.EnableSsl)
        {
            _ = builder.EnableSsl(false);
        }

        if (options.AccessKey is not null && options.AccessSecret is not null)
        {
            _ = builder.SetCredentialsProvider(
                new StaticSessionCredentialsProvider(options.AccessKey, options.AccessSecret)
            );
        }

        return builder.Build();
    }

    private static async Task<ISendReceipt?> BuildAndSendAsync(
        ClientConfig clientConfig,
        string topic,
        CancellationToken cancellationToken
    )
    {
        var producer = await new Producer.Builder()
            .SetClientConfig(clientConfig)
            .SetTopics(topic)
            .Build()
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        await using (producer.ConfigureAwait(false))
        {
            var message = new Message.Builder()
                .SetTopic(topic)
                .SetBody(Encoding.UTF8.GetBytes("healthcheck"))
                .Build();

            return await producer.Send(message).WaitAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
