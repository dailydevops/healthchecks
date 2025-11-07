namespace NetEvolve.HealthChecks.Keycloak;

using System.Threading.Tasks;
using global::Keycloak.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(KeycloakOptions))]
internal sealed partial class KeycloakHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        KeycloakOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientProvider = _serviceProvider.GetRequiredService<KeycloakClientProvider>();
        var client = clientProvider.GetClient(name, options, _serviceProvider);

        var commandTask = options.CommandAsync.Invoke(client, cancellationToken);

        var (isTimelyResponse, resultIsValid) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (!resultIsValid)
        {
            return HealthCheckUnhealthy(failureStatus, name, "The command did not return a valid result.");
        }

        return HealthCheckState(isTimelyResponse, name);
    }

    internal static async Task<bool> DefaultCommandAsync(KeycloakClient client, CancellationToken cancellationToken)
    {
        var serverInfo = await client.GetServerInfoAsync("master", cancellationToken).ConfigureAwait(false);
        return serverInfo is not null;
    }
}
