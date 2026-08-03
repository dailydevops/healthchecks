namespace NetEvolve.HealthChecks.Seq;

using System.Threading;
using System.Threading.Tasks;
using global::Seq.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(SeqOptions))]
internal sealed partial class SeqHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        SeqOptions options,
        CancellationToken cancellationToken
    )
    {
        var clientProvider = _serviceProvider.GetRequiredService<SeqClientProvider>();
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

    internal static async Task<bool> DefaultCommandAsync(SeqConnection client, CancellationToken cancellationToken)
    {
        var root = await client.Client.GetRootAsync(cancellationToken).ConfigureAwait(false);
        return root is not null;
    }
}
