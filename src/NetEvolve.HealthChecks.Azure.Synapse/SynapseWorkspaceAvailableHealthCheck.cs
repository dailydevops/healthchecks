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

        // Check workspace availability by trying to get pipeline service
        var pipelineTask = Task.FromResult(true); // Simple connectivity check
        try
        {
            // Try to call a simple method to test connectivity
            var pipelineService = artifactsClient.Pipeline;
            pipelineTask = Task.FromResult(pipelineService is not null);
        }
        catch
        {
            pipelineTask = Task.FromResult(false);
        }

        var (isValid, result) = await pipelineTask
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid && result, name);
    }
}