namespace NetEvolve.HealthChecks.Tests.Integration.Dapr;

using System.Globalization;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Abstractions;

public sealed class DaprContainer : IAsyncInitializer, IAsyncDisposable
{
    private const int DaprHttpPort = 3500;
    private const int DaprGrpcPort = 50001;
    private readonly IContainer _container = new ContainerBuilder()
        .WithImage("daprio/daprd:latest")
        .WithPortBinding(DaprHttpPort, true)
        .WithPortBinding(DaprGrpcPort, true)
        .WithCommand("./daprd", "-app-id", "test-app", "-dapr-http-port", DaprHttpPort.ToString(CultureInfo.InvariantCulture), "-dapr-grpc-port", DaprGrpcPort.ToString(CultureInfo.InvariantCulture))
        .WithLogger(NullLogger.Instance)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(DaprHttpPort).ForPath("/v1.0/healthz")))
        .Build();

    public string HttpEndpoint => $"http://{_container.Hostname}:{_container.GetMappedPublicPort(DaprHttpPort)}";

    public string GrpcEndpoint => $"http://{_container.Hostname}:{_container.GetMappedPublicPort(DaprGrpcPort)}";

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
