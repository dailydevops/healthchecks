namespace NetEvolve.HealthChecks.Tests.Unit.JanusGraph;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.JanusGraph;

[TestGroup(nameof(JanusGraph))]
public sealed class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddJanusGraph_WhenBuilderIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => _ = builder!.AddJanusGraph("Test");

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Test]
    public void AddJanusGraph_WhenNameIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        void Act() => _ = builder.AddJanusGraph(null!);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void AddJanusGraph_WhenNameIsEmpty_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        void Act() => _ = builder.AddJanusGraph(string.Empty);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void AddJanusGraph_WhenTagsIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        void Act() => _ = builder.AddJanusGraph("Test", null, null!);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("tags", Act);
    }

    [Test]
    public void AddJanusGraph_WhenNameIsAlreadyUsed_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();

        // Act
        void Act() => _ = builder.AddJanusGraph("Test").AddJanusGraph("Test");

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }
}
