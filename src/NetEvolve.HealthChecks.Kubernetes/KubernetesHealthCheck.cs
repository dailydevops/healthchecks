namespace NetEvolve.HealthChecks.Kubernetes;

using System.Threading;
using System.Threading.Tasks;
using global::k8s;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(KubernetesOptions))]
internal sealed partial class KubernetesHealthCheck
{
    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        KubernetesOptions options,
        CancellationToken cancellationToken
    )
    {
        var client = string.IsNullOrWhiteSpace(options.KeyedService)
            ? _serviceProvider.GetRequiredService<IKubernetes>()
            : _serviceProvider.GetRequiredKeyedService<IKubernetes>(options.KeyedService);

        var (isTimelyResponse, version) = await client
            .Version.GetCodeAsync(cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (version is null || string.IsNullOrWhiteSpace(version.GitVersion))
        {
            return HealthCheckUnhealthy(failureStatus, name);
        }

        return HealthCheckState(isTimelyResponse, name);
    }
}
