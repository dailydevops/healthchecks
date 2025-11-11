namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Kusto;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Kusto;

[TestGroup($"{nameof(Azure)}.{nameof(Kusto)}")]
public sealed class KustoAvailableConfigureTests
{
    [Test]
    public void Configure_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var configure = new KustoAvailableConfigure(new ConfigurationBuilder().Build());
        var options = new KustoAvailableOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    public async Task Validate_WhenNameNull_Invalid()
    {
        // Arrange
        var configure = new KustoAvailableConfigure(new ConfigurationBuilder().Build());
        var options = new KustoAvailableOptions();
        const string? name = default;

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenOptionsNull_Invalid()
    {
        // Arrange
        var configure = new KustoAvailableConfigure(new ConfigurationBuilder().Build());
        const KustoAvailableOptions? options = default;

        // Act
        var result = configure.Validate("Test", options!);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenTimeoutNegative_Invalid()
    {
        // Arrange
        var configure = new KustoAvailableConfigure(new ConfigurationBuilder().Build());
        var options = new KustoAvailableOptions { Timeout = -2 };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenConnectionStringAndClusterUriNull_Invalid()
    {
        // Arrange
        var configure = new KustoAvailableConfigure(new ConfigurationBuilder().Build());
        var options = new KustoAvailableOptions();

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenConnectionStringSet_Valid()
    {
        // Arrange
        var configure = new KustoAvailableConfigure(new ConfigurationBuilder().Build());
        var options = new KustoAvailableOptions { ConnectionString = "https://test.kusto.windows.net" };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Validate_WhenClusterUriSet_Valid()
    {
        // Arrange
        var configure = new KustoAvailableConfigure(new ConfigurationBuilder().Build());
        var options = new KustoAvailableOptions { ClusterUri = new Uri("https://test.kusto.windows.net") };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Validate_WhenClusterUriNotAbsolute_Invalid()
    {
        // Arrange
        var configure = new KustoAvailableConfigure(new ConfigurationBuilder().Build());
        var options = new KustoAvailableOptions { ClusterUri = new Uri("/test", UriKind.Relative) };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
    }
}
