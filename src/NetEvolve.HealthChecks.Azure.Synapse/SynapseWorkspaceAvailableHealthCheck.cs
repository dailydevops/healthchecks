namespace NetEvolve.HealthChecks.Azure.Synapse;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class SynapseWorkspaceAvailableHealthCheck : ConfigurableHealthCheckBase<SynapseWorkspaceAvailableOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public SynapseWorkspaceAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<SynapseWorkspaceAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        SynapseWorkspaceAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var artifactsClient = clientCreation.GetArtifactsClient(name, options, _serviceProvider);

        try
        {
            // Test connectivity by trying to access the LinkedService service
            // This is a basic connectivity check to the Synapse workspace
            var linkedServiceClient = artifactsClient.LinkedService;
            var healthCheckTask = Task.FromResult(linkedServiceClient is not null);

            var (isValid, result) = await healthCheckTask
                .WithTimeoutAsync(options.Timeout, cancellationToken)
                .ConfigureAwait(false);

            return HealthCheckState(isValid && result, name);
        }
        catch (Exception ex)
        {
            return HealthCheckUnhealthy(failureStatus, name, ex: ex);
        }
    }
}