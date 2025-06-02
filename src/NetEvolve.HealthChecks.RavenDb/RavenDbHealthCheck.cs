namespace NetEvolve.HealthChecks.RavenDb;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.ServerWide.Operations;

internal sealed class RavenDbHealthCheck : ConfigurableHealthCheckBase<RavenDbOptions>
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbHealthCheck"/> class.
    /// </summary>
    /// <param name="optionsMonitor">The <see cref="IOptionsMonitor{TOptions}"/> instance used to access named options.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to resolve dependencies.</param>
    public RavenDbHealthCheck(IOptionsMonitor<RavenDbOptions> optionsMonitor, IServiceProvider serviceProvider)
        : base(optionsMonitor)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        RavenDbOptions options,
        CancellationToken cancellationToken
    )
    {
        var store = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IDocumentStore>()
            : _serviceProvider.GetRequiredKeyedService<IDocumentStore>(options.KeyedService);

        var commandTask = options.CommandAsync.Invoke(store, cancellationToken);

        var (isHealthy, result) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isHealthy && result, name);
    }

    internal static async Task<bool> DefaultCommandAsync(IDocumentStore store, CancellationToken cancellationToken)
    {
        var buildNumber = await store
            .Maintenance.Server.SendAsync(new GetBuildNumberOperation(), cancellationToken)
            .ConfigureAwait(false);

        return buildNumber is not null;
    }
}
