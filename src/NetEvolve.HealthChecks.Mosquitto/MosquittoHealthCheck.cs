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
        HealthStatus failureStatus,
        MosquittoOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IMqttClient>()
            : _serviceProvider.GetRequiredKeyedService<IMqttClient>(options.KeyedService);

        var checkTask = Task.Run(() => client.IsConnected, cancellationToken);

        var (isTimelyResponse, result) = await checkTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Mosquitto health check failed.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
