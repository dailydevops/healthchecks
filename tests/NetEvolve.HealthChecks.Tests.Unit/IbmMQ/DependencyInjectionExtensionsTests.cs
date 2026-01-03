namespace NetEvolve.HealthChecks.Tests.Unit.IbmMQ;

using System;
using Microsoft.Extensions.Configuration;
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
    public void AddIbmMQ_WithDuplicateName_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection().AddSingleton<IConfiguration>(
            new ConfigurationBuilder().AddInMemoryCollection([]).Build()
        );
        var builder = services.AddHealthChecks();
        _ = builder.AddIbmMQ("test");

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() => builder.AddIbmMQ("test"));
    }
}
