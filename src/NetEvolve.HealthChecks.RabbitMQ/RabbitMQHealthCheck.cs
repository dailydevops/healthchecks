namespace NetEvolve.HealthChecks.RabbitMQ;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Health check implementation for RabbitMQ, using the <c>RabbitMQ.Client</c> package.
/// </summary>
internal sealed class RabbitMQHealthCheck : ConfigurableHealthCheckBase<RabbitMQOptions>
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQHealthCheck"/> class.
    /// </summary>
    /// <param name="optionsMonitor">The <see cref="IOptionsMonitor{TOptions}"/> instance used to access named options.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to resolve dependencies.</param>
    public RabbitMQHealthCheck(IOptionsMonitor<RabbitMQOptions> optionsMonitor, IServiceProvider serviceProvider)
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    /// <inheritdoc />
    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        RabbitMQOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IConnection>()
            : _serviceProvider.GetRequiredKeyedService<IConnection>(options.KeyedService);

        var (isValid, _) = await client
            .CreateChannelAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }
}
