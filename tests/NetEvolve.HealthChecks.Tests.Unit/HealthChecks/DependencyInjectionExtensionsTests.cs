namespace NetEvolve.HealthChecks.Tests.Unit.HealthChecks;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks;

[TestGroup(nameof(HealthChecks))]
public class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddApplicationReadinessCheck_ParamBuilderNull_ArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "builder",
            () => DependencyInjectionExtensions.AddApplicationReady(null!, tags: null!)
        );

    [Test]
    public void AddApplicationReadinessCheck_ParamTagsNull_ArgumentNullException()
    {
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        _ = Assert.Throws<ArgumentNullException>("tags", () => builder.AddApplicationReady(tags: null!));
    }

    [Test]
    public async Task AddApplicationReadinessCheck_Fine_Expected()
    {
        var services = new ServiceCollection();
        _ = services
            .AddSingleton<IHostApplicationLifetime, TestHostApplicationLifeTime>()
            .AddHealthChecks()
            .AddApplicationReady("self", "healthy")
            .AddApplicationReady();

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>()!;

        var check = options.Value.Registrations.First();

        using (Assert.Multiple())
        {
            _ = await Assert.That(check).IsNotNull();
            _ = await Assert.That(check!.Tags.Count).IsEqualTo(3);
            _ = await Assert.That(check!.Name).IsEqualTo("ApplicationReady");
            _ = await Assert.That(check!.Factory(serviceProvider)).IsNotNull().And.IsTypeOf<ApplicationReadyCheck>();
        }
    }

    [Test]
    public void AddApplicationHealthy_ParamBuilderNull_ArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "builder",
            () => DependencyInjectionExtensions.AddApplicationHealthy(null!, tags: null!)
        );

    [Test]
    public void AddApplicationHealthy_ParamTagsNull_ArgumentNullException()
    {
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        _ = Assert.Throws<ArgumentNullException>("tags", () => builder.AddApplicationHealthy(tags: null!));
    }

    [Test]
    public async Task AddApplicationHealthy_Fine_Expected()
    {
        var services = new ServiceCollection();
        _ = services.AddHealthChecks().AddApplicationHealthy("self", "healthy").AddApplicationHealthy();

        var serviceProvider = services.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>()!;

        var check = options.Value.Registrations.First();

        using (Assert.Multiple())
        {
            _ = await Assert.That(check).IsNotNull();
            _ = await Assert.That(check.Tags.Count).IsEqualTo(2);
            _ = await Assert.That(check.Name).IsEqualTo("ApplicationHealthy");
            _ = await Assert.That(check.Factory(serviceProvider)).IsTypeOf<ApplicationHealthyCheck>();
        }
    }

    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "False positive")]
    private sealed class TestHostApplicationLifeTime : IHostApplicationLifetime, IDisposable
    {
        private readonly CancellationTokenSource _sourceStarted = new();
        private readonly CancellationTokenSource _sourceStopping = new();
        private readonly CancellationTokenSource _sourceStopped = new();

        public CancellationToken ApplicationStarted => _sourceStarted.Token;

        public CancellationToken ApplicationStopped => _sourceStopped.Token;

        public CancellationToken ApplicationStopping => _sourceStopping.Token;

        public void StopApplication()
        {
            ExecuteHandlers(_sourceStopping);

            ExecuteHandlers(_sourceStopped);
        }

        private static void ExecuteHandlers(CancellationTokenSource source)
        {
            if (source.IsCancellationRequested)
            {
                return;
            }

            source.Cancel(throwOnFirstException: false);
        }

        public void Dispose()
        {
            _sourceStarted.Dispose();
            _sourceStopping.Dispose();
            _sourceStopped.Dispose();
        }
    }
}
