namespace NetEvolve.HealthChecks.SqlServer.Tests.Unit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

using static NetEvolve.HealthChecks.SqlServer.DependencyInjectionExtensions;

[ExcludeFromCodeCoverage]
[UnitTest]
public class DependencyInjectionExtensionsTests
{
    [Fact]
    public void AddSqlServer_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => _ = builder.AddSqlServer("Test");

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Fact]
    public void AddSqlServer_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var name = default(string);

        // Act
        void Act() => _ = builder.AddSqlServer(name);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Fact]
    public void AddSqlServer_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var name = string.Empty;

        // Act
        void Act() => _ = builder.AddSqlServer(name);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Fact]
    public void AddSqlServer_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var tags = default(string[]);

        // Act
        void Act() => _ = builder.AddSqlServer("Test", tags: tags);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("tags", Act);
    }

    [Fact]
    public void AddSqlServer_WhenArgumentNameIsAlreadyUsed_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        const string name = "Test";

        // Act
        void Act() => _ = builder.AddSqlServer(name).AddSqlServer(name);

        // Assert
        _ = Assert.Throws<ArgumentException>(name, Act);
    }
}
