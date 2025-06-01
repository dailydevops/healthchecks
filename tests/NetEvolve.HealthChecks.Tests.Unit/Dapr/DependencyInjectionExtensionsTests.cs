namespace NetEvolve.HealthChecks.Tests.Unit.Dapr;

using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Dapr;

[TestGroup(nameof(Dapr))]
public class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddDapr_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => builder.AddDapr();

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Test]
    public void AddDapr_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var tags = default(string[]);

        // Act
        void Act() => builder.AddDapr(tags: tags);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("tags", Act);
    }

    [Test]
    public async Task AddDapr_Fine_Expected()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        string[] tags = ["sidecar"];
        _ = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks().AddDapr(_ => { }, tags).AddDapr();

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>()!;

        var check = options.Value.Registrations.First();

        using (Assert.Multiple())
        {
            _ = await Assert.That(check).IsNotNull();
            _ = await Assert.That(check.Tags.Count).IsEqualTo(2);
            _ = await Assert.That(check.Name).IsEqualTo("DaprSidecar");
            _ = await Assert.That(check.Factory(serviceProvider)).IsTypeOf<DaprHealthCheck>();
        }
    }
}
