namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Kusto;

using System;
using System.Threading.Tasks;
using global::NetEvolve.HealthChecks.Azure.Kusto;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;

[TestGroup($"{nameof(Azure)}.{nameof(Kusto)}")]
public sealed class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddKusto_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        IHealthChecksBuilder? builder = null;

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(
            "builder",
            () =>
            {
                _ = builder.AddKusto(null!);
            }
        );
    }

    [Test]
    public void AddKusto_WhenArgumentNameNull_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(
            "name",
            () =>
            {
                _ = builder.AddKusto(null!);
            }
        );
    }

    [Test]
    public void AddKusto_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>("name", () => builder.AddKusto(""));
    }

    [Test]
    public void AddKusto_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(
            "tags",
            () =>
            {
                _ = builder.AddKusto("Test", null, null!);
            }
        );
    }
}
