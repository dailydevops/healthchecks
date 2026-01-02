namespace NetEvolve.HealthChecks.Tests.Unit.IbmMQ;

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.IbmMQ;

[TestGroup(nameof(IbmMQ))]
public sealed class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddIbmMQ_WithNullBuilder_ThrowsArgumentNullException()
    {
        // Arrange
        IHealthChecksBuilder builder = null!;

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => builder.AddIbmMQ("test"));
    }

    [Test]
    public void AddIbmMQ_WithNullName_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() => builder.AddIbmMQ(null!));
    }

    [Test]
    public void AddIbmMQ_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() => builder.AddIbmMQ(string.Empty));
    }

    [Test]
    public void AddIbmMQ_WithNullTags_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => builder.AddIbmMQ("test", null, null!));
    }

    [Test]
    public async Task AddIbmMQ_WithValidName_RegistersHealthCheck()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        _ = builder.AddIbmMQ("test");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var registrations = serviceProvider.GetService<IEnumerable<HealthCheckRegistration>>();
        _ = await Assert.That(registrations).IsNotNull();

        var registration = registrations!.FirstOrDefault(x => x.Name == "test");
        _ = await Assert.That(registration).IsNotNull();
    }

    [Test]
    public async Task AddIbmMQ_WithOptions_RegistersHealthCheck()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        _ = builder.AddIbmMQ("test", options => options.Timeout = 500);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var registrations = serviceProvider.GetService<IEnumerable<HealthCheckRegistration>>();
        _ = await Assert.That(registrations).IsNotNull();

        var registration = registrations!.FirstOrDefault(x => x.Name == "test");
        _ = await Assert.That(registration).IsNotNull();
    }

    [Test]
    public async Task AddIbmMQ_WithTags_RegistersHealthCheckWithTags()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        _ = builder.AddIbmMQ("test", null, "custom-tag");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var registrations = serviceProvider.GetService<IEnumerable<HealthCheckRegistration>>();
        _ = await Assert.That(registrations).IsNotNull();

        var registration = registrations!.FirstOrDefault(x => x.Name == "test");
        using (Assert.Multiple())
        {
            _ = await Assert.That(registration).IsNotNull();
            _ = await Assert.That(registration!.Tags).Contains("ibmmq");
            _ = await Assert.That(registration!.Tags).Contains("messaging");
            _ = await Assert.That(registration!.Tags).Contains("custom-tag");
        }
    }

    [Test]
    public void AddIbmMQ_WithDuplicateName_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        _ = builder.AddIbmMQ("test");

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() => builder.AddIbmMQ("test"));
    }
}
