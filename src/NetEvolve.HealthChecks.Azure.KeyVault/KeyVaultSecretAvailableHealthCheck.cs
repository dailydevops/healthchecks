namespace NetEvolve.HealthChecks.Azure.KeyVault;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class KeyVaultSecretAvailableHealthCheck : ConfigurableHealthCheckBase<KeyVaultSecretAvailableOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public KeyVaultSecretAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<KeyVaultSecretAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        KeyVaultSecretAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var secretClient = clientCreation.GetSecretClient(name, options, _serviceProvider);

        var propertiesTask = secretClient
            .GetPropertiesOfSecretsAsync(cancellationToken: cancellationToken)
            .AsPages(pageSizeHint: 1)
            .GetAsyncEnumerator(cancellationToken)
            .MoveNextAsync();

        var (isValid, result) = await propertiesTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid && result, name);
    }
}
