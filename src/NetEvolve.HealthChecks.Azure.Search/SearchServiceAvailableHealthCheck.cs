namespace NetEvolve.HealthChecks.Azure.Search;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class SearchServiceAvailableHealthCheck : ConfigurableHealthCheckBase<SearchServiceAvailableOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public SearchServiceAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<SearchServiceAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        SearchServiceAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var searchIndexClient = clientCreation.GetSearchIndexClient(name, options, _serviceProvider);

        var (isValid, _) = await searchIndexClient
            .GetServiceStatisticsAsync(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }
}
