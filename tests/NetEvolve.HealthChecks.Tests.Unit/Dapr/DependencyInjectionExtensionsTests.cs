namespace NetEvolve.HealthChecks.Tests.Unit.Dapr;

using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Dapr;
using Xunit;

[TestGroup(nameof(Dapr))]
public class DependencyInjectionExtensionsTests
{
    [Fact]
    public void AddDapr_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => _ = builder.AddDapr();

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Fact]
    public void AddDapr_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var tags = default(string[]);

        // Act
        void Act() => _ = builder.AddDapr(tags: tags);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("tags", Act);
    }

    [Fact]
    public void AddDapr_Fine_Expected()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        string[] tags = ["sidecar"];
        _ = services
            .AddSingleton<IConfiguration>(configuration)
            .AddHealthChecks()
            .AddDapr(options => { }, tags)
            .AddDapr();

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>()!;

        _ = Assert.Single(options.Value.Registrations);

        var check = options.Value.Registrations.First();

        Assert.Equal(2, check.Tags.Count);
        Assert.Equal("DaprSidecar", check.Name);
        _ = Assert.IsType<DaprHealthCheck>(check.Factory(serviceProvider));
    }
}
