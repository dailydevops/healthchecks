namespace NetEvolve.HealthChecks.Tests.Integration.Kubernetes;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using k8s;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.K3s;
using TUnit.Core.Interfaces;

[SuppressMessage("Naming", "S101:Types should be named in PascalCase", Justification = "K3s is the product name.")]
public sealed class K3sDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly K3sContainer _container = new K3sBuilder(
        /*dockerimage*/"rancher/k3s:v1.35.0-rc1-k3s1"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    private KubernetesClientConfiguration? _configuration;

    public IKubernetes CreateClient() => new Kubernetes(_configuration!);

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);
        var kubeconfig = await _container.GetKubeconfigAsync().ConfigureAwait(false);
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(kubeconfig));
        _configuration = await KubernetesClientConfiguration
            .BuildConfigFromConfigFileAsync(stream)
            .ConfigureAwait(false);
    }
}
