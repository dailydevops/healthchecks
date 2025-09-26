namespace NetEvolve.HealthChecks.Azure.DigitalTwin;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class DigitalTwinServiceAvailableHealthCheck
    : ConfigurableHealthCheckBase<DigitalTwinServiceAvailableOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public DigitalTwinServiceAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<DigitalTwinServiceAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        DigitalTwinServiceAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var digitalTwinsClient = clientCreation.GetDigitalTwinsClient(name, options, _serviceProvider);

        var (isValid, _) = await digitalTwinsClient
            .QueryAsync<object>("SELECT * FROM digitaltwins", cancellationToken: cancellationToken)
            .GetAsyncEnumerator(cancellationToken)
            .MoveNextAsync()
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }
}
