namespace NetEvolve.HealthChecks.Azure.KeyVault;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class KeyVaultHealthCheck : ConfigurableHealthCheckBase<KeyVaultOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public KeyVaultHealthCheck(IServiceProvider serviceProvider, IOptionsMonitor<KeyVaultOptions> optionsMonitor)
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        KeyVaultOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = KeyVaultClientFactory.CreateSecretClient(name, options, _serviceProvider);

        try
        {
            // Try to list secrets to verify connectivity and permissions
            var secretsEnumerator = client
                .GetPropertiesOfSecretsAsync(cancellationToken)
                .GetAsyncEnumerator(cancellationToken);

            var (isValid, _) = await secretsEnumerator
                .MoveNextAsync()
                .AsTask()
                .WithTimeoutAsync(options.Timeout, cancellationToken)
                .ConfigureAwait(false);

            await secretsEnumerator.DisposeAsync().ConfigureAwait(false);

            return HealthCheckState(isValid, name);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"{name}: {ex.Message}", ex);
        }
    }
}
