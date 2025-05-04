namespace NetEvolve.HealthChecks.Apache.ActiveMq;

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using global::Apache.NMS;
using global::Apache.NMS.ActiveMQ;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class ActiveMqHealthCheck : ConfigurableHealthCheckBase<ActiveMqOptions>
{
    public ActiveMqHealthCheck(IOptionsMonitor<ActiveMqOptions> optionsMonitor)
        : base(optionsMonitor) { }

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        ActiveMqOptions options,
        CancellationToken cancellationToken
    )
    {
        using var client = await ClientFactory
            .GetConnectionAsync(options, cancellationToken)
            .ConfigureAwait(false);

        var isValid = await client
            .StartAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(client.IsStarted && isValid, name);
    }
}
