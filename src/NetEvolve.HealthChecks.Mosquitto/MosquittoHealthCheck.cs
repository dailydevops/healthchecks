namespace NetEvolve.HealthChecks.Mosquitto;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MQTTnet;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

/// <summary>
/// Health check implementation for Mosquitto, using the <c>MQTTnet</c> package.
/// </summary>
[ConfigurableHealthCheck(typeof(MosquittoOptions))]
internal sealed partial class MosquittoHealthCheck
{
    /// <inheritdoc />
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus _,
#pragma warning restore S1172 // Unused method parameters should be removed
        MosquittoOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IMqttClient>()
            : _serviceProvider.GetRequiredKeyedService<IMqttClient>(options.KeyedService);

        var checkTask = Task.Run(
            () =>
            {
                if (!client.IsConnected)
                {
                    throw new InvalidOperationException("Client is not connected.");
                }
                return true;
            },
            cancellationToken
        );

        var (isTimelyResponse, _) = await checkTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse, name);
    }
}
