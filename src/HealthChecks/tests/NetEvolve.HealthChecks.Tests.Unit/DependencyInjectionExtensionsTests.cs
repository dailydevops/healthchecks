namespace NetEvolve.Extensions.Tasks.Tests.Unit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Xunit;

[UnitTest]
[ExcludeFromCodeCoverage]
public class DependencyInjectionExtensionsTests
{
    [Fact]
    public void AddApplicationReadinessCheck_ParamBuilderNull_ArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "builder",
            () => DependencyInjectionExtensions.AddApplicationReadinessCheck(null!, tags: null!)
        );

    [Fact]
    public void AddApplicationReadinessCheck_ParamTagsNull_ArgumentNullException()
    {
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        _ = Assert.Throws<ArgumentNullException>(
            "tags",
            () => DependencyInjectionExtensions.AddApplicationReadinessCheck(builder, tags: null!)
        );
    }

    [Fact]
    public void AddApplicationReadinessCheck_Fine_Expected()
    {
        var services = new ServiceCollection();
        _ = services
            .AddSingleton<IHostApplicationLifetime, TestHostApplicationLifeTime>()
            .AddHealthChecks()
            .AddApplicationReadinessCheck("self", "healthy")
            .AddApplicationReadinessCheck();

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>()!;

        _ = Assert.Single(options.Value.Registrations);

        var check = options.Value.Registrations.First();

        Assert.Equal(3, check.Tags.Count);
        Assert.Equal("ApplicationReadiness", check.Name);
        _ = Assert.IsType<ApplicationReadinessCheck>(check.Factory(serviceProvider));
    }

    [Fact]
    public void AddSelfCheck_ParamBuilderNull_ArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "builder",
            () => DependencyInjectionExtensions.AddApplicationSelfCheck(null!, tags: null!)
        );

    [Fact]
    public void AddSelfCheck_ParamTagsNull_ArgumentNullException()
    {
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        _ = Assert.Throws<ArgumentNullException>(
            "tags",
            () => DependencyInjectionExtensions.AddApplicationSelfCheck(builder, tags: null!)
        );
    }

    [Fact]
    public void AddSelfCheck_Fine_Expected()
    {
        var services = new ServiceCollection();
        _ = services
            .AddHealthChecks()
            .AddApplicationSelfCheck("self", "healthy")
            .AddApplicationSelfCheck();

        var serviceProvider = services.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>()!;

        _ = Assert.Single(options.Value.Registrations);

        var check = options.Value.Registrations.First();

        Assert.Equal(2, check.Tags.Count);
        Assert.Equal("ApplicationSelf", check.Name);
        _ = Assert.IsType<ApplicationSelfCheck>(check.Factory(serviceProvider));
    }

    [ExcludeFromCodeCoverage]
    [SuppressMessage(
        "Performance",
        "CA1812:Avoid uninstantiated internal classes",
        Justification = "False positive"
    )]
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
