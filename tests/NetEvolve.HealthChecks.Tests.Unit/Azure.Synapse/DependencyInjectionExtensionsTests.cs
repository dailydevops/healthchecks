namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Synapse;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Synapse;

[TestGroup($"{nameof(Azure)}.{nameof(Synapse)}")]
public class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddSynapseWorkspaceAvailability_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => builder.AddSynapseWorkspaceAvailability("Test");

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Test]
    public void AddSynapseWorkspaceAvailability_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        void Act() => builder.AddSynapseWorkspaceAvailability(null!);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void AddSynapseWorkspaceAvailability_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        void Act() => builder.AddSynapseWorkspaceAvailability(string.Empty);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void AddSynapseWorkspaceAvailability_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        void Act() => builder.AddSynapseWorkspaceAvailability("Test", tags: null!);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("tags", Act);
    }
}