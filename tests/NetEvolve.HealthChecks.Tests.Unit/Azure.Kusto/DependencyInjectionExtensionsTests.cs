namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Kusto;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Kusto;

[TestGroup($"{nameof(Azure)}.{nameof(Kusto)}")]
public sealed class DependencyInjectionExtensionsTests
{
    [Test]
    public async Task AddKusto_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        _ = await Assert
            .That(() => DependencyInjectionExtensions.AddKusto(null!, "Test"))
            .Throws<ArgumentNullException>()
            .And.HasParameterName("builder");
    }

    [Test]
    public async Task AddKusto_WhenArgumentNameNull_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act & Assert
        _ = await Assert
            .That(() => builder.AddKusto(null!))
            .Throws<ArgumentException>()
            .And.HasParameterName("name");
    }

    [Test]
    public async Task AddKusto_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act & Assert
        _ = await Assert
            .That(() => builder.AddKusto(string.Empty))
            .Throws<ArgumentException>()
            .And.HasParameterName("name");
    }

    [Test]
    public async Task AddKusto_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act & Assert
        _ = await Assert
            .That(() => builder.AddKusto("Test", null, null!))
            .Throws<ArgumentNullException>()
            .And.HasParameterName("tags");
    }
}