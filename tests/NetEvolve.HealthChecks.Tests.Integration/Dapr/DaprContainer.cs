namespace NetEvolve.HealthChecks.Tests.Integration.Dapr;

using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Abstractions;

public sealed class DaprContainer : IAsyncInitializer, IAsyncDisposable
{
    private const int DaprHttpPort = 3500;
    private const int DaprGrpcPort = 50001;
    private readonly IContainer _container = new ContainerBuilder(
        /*dockerimage*/"daprio/daprd:1.16.5"
    )
        .WithPortBinding(DaprHttpPort, true)
        .WithPortBinding(DaprGrpcPort, true)
        .WithEntrypoint("./daprd")
        .WithCommand(
            "-app-id",
            "test-app",
            "-dapr-http-port",
            DaprHttpPort.ToString(CultureInfo.InvariantCulture),
            "-dapr-grpc-port",
            DaprGrpcPort.ToString(CultureInfo.InvariantCulture),
            "--log-level",
            "info"
        )
        .WithLogger(NullLogger.Instance)
        .WithWaitStrategy(
            Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(r =>
                    r.ForPath("/v1.0/healthz").ForPort(DaprHttpPort).ForStatusCode(HttpStatusCode.NoContent)
                )
        )
        .Build();

    public string HttpEndpoint => $"http://{_container.Hostname}:{_container.GetMappedPublicPort(DaprHttpPort)}";

    public string GrpcEndpoint => $"http://{_container.Hostname}:{_container.GetMappedPublicPort(DaprGrpcPort)}";

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
