namespace NetEvolve.HealthChecks.Keycloak;

using System;
using System.Threading.Tasks;
using global::Keycloak.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class KeycloakHealthCheck : ConfigurableHealthCheckBase<KeycloakOptions>
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakHealthCheck"/> class.
    /// </summary>
    /// <param name="optionsMonitor">The <see cref="IOptionsMonitor{TOptions}"/> instance used to access named options.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to resolve dependencies.</param>
    public KeycloakHealthCheck(IOptionsMonitor<KeycloakOptions> optionsMonitor, IServiceProvider serviceProvider)
        : base(optionsMonitor)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        KeycloakOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientProvider = _serviceProvider.GetRequiredService<KeycloakClientProvider>();
        var client = clientProvider.GetClient(options, _serviceProvider);

        var commandTask = options.CommandAsync.Invoke(client, cancellationToken);

        var (isHealthy, result) = await commandTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isHealthy && result, name);
    }

    internal static async Task<bool> DefaultCommandAsync(KeycloakClient client, CancellationToken cancellationToken)
    {
        var serverInfo = await client.GetServerInfoAsync("master", cancellationToken).ConfigureAwait(false);
        return serverInfo is not null;
    }
}
