namespace NetEvolve.HealthChecks.Tests.Integration.Seq.Container;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Seq;

public sealed class SeqContainerAccess : IAsyncInitializer, IAsyncDisposable
{
    private readonly SeqContainer _container = new SeqBuilder(
        /*dockerimage*/"datalust/seq:2025.2.16202"
    )
        .WithLogger(NullLogger.Instance)
        .WithAcceptLicenseAgreement(true)
        .Build();

    public Uri ServerUrl => new Uri(_container.GetEndpoint());

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
