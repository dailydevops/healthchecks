namespace NetEvolve.HealthChecks.Azure.Files;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class FileServiceAvailableHealthCheck : ConfigurableHealthCheckBase<FileServiceAvailableOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public FileServiceAvailableHealthCheck(
        IServiceProvider serviceProvider,
        IOptionsMonitor<FileServiceAvailableOptions> optionsMonitor
    )
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        FileServiceAvailableOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientCreation = _serviceProvider.GetRequiredService<ClientCreation>();
        var shareClient = clientCreation.GetShareServiceClient(name, options, _serviceProvider);

        var (isValid, _) = await shareClient
            .GetPropertiesAsync(cancellationToken: cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        return HealthCheckState(isValid, name);
    }
}