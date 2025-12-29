namespace NetEvolve.HealthChecks.RavenDb;

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using Raven.Client.Documents;
using Raven.Client.ServerWide.Operations;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(RavenDbOptions))]
internal sealed partial class RavenDbHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
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

        var (isTimelyResponse, result) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!result)
        {
            return HealthCheckUnhealthy(failureStatus, name, $"RavenDB health check '{name}' failed.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<bool> DefaultCommandAsync(IDocumentStore store, CancellationToken cancellationToken)
    {
        var buildNumber = await store
            .Maintenance.Server.SendAsync(new GetBuildNumberOperation(), cancellationToken)
            .ConfigureAwait(false);

        return buildNumber is not null;
    }
}
