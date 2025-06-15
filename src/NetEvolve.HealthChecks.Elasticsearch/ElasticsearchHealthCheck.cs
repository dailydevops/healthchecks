namespace NetEvolve.HealthChecks.Elasticsearch;

using System;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class ElasticsearchHealthCheck : ConfigurableHealthCheckBase<ElasticsearchOptions>
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchHealthCheck"/> class.
    /// </summary>
    /// <param name="optionsMonitor">The <see cref="IOptionsMonitor{TOptions}"/> instance used to access named options.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to resolve dependencies.</param>
    public ElasticsearchHealthCheck(
        IOptionsMonitor<ElasticsearchOptions> optionsMonitor,
        IServiceProvider serviceProvider
    )
        : base(optionsMonitor)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus failureStatus,
        ElasticsearchOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientProvider = _serviceProvider.GetRequiredService<ElasticsearchClientProvider>();
        var client = clientProvider.GetClient(name, options, _serviceProvider);

        var commandTask = options.CommandAsync.Invoke(client, cancellationToken);

        var (isNotTimedOut, isResultValid) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return !isResultValid
            ? HealthCheckUnhealthy(failureStatus, name, "Invalid command result.")
            : HealthCheckState(isNotTimedOut, name);
    }

    internal static async Task<bool> DefaultCommandAsync(
        ElasticsearchClient client,
        CancellationToken cancellationToken
    )
    {
        var response = await client.PingAsync(cancellationToken).ConfigureAwait(false);

        return response.IsValidResponse;
    }
}
