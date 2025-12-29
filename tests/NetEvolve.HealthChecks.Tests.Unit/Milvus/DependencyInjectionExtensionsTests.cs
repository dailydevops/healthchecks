namespace NetEvolve.HealthChecks.Tests.Unit.Milvus;

using System;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Milvus;

[TestGroup(nameof(Milvus))]
public sealed class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddMilvus_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => _ = builder!.AddMilvus("Test");

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Test]
    public void AddMilvus_WhenArgumentNameNull_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        const string? name = default;

        // Act
        void Act() => _ = builder.AddMilvus(name!);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void AddMilvus_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var name = string.Empty;

        // Act
        void Act() => _ = builder.AddMilvus(name);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void AddMilvus_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var tags = default(string[]);

        // Act
        void Act() => _ = builder.AddMilvus("Test", tags: tags!);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("tags", Act);
    }
}
