namespace NetEvolve.HealthChecks.RavenDb;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using Raven.Client.Documents;
using Raven.Client.ServerWide.Operations;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(RavenDbOptions))]
internal sealed partial class RavenDbHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
#pragma warning disable S1172 // Unused method parameters should be removed
        HealthStatus _,
#pragma warning restore S1172 // Unused method parameters should be removed
        RavenDbOptions options,
        CancellationToken cancellationToken
    )
    {
        var store = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IDocumentStore>()
            : _serviceProvider.GetRequiredKeyedService<IDocumentStore>(options.KeyedService);

        var commandTask = options.CommandAsync.Invoke(store, cancellationToken);

        var (isTimelyResponse, result) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isTimelyResponse && result, name);
    }

    internal static async Task<bool> DefaultCommandAsync(IDocumentStore store, CancellationToken cancellationToken)
    {
        var buildNumber = await store
            .Maintenance.Server.SendAsync(new GetBuildNumberOperation(), cancellationToken)
            .ConfigureAwait(false);

        return buildNumber is not null;
    }
}
