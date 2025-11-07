namespace NetEvolve.HealthChecks.Tests.Unit.MySql.Devart;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.MySql.Devart;

[TestGroup($"{nameof(MySql)}.{nameof(Devart)}")]
public sealed class MySqlDevartOptionsTests
{
    [Test]
    public async Task ConnectionString_WhenSet_ShouldReturnValue()
    {
        // Arrange
        var options = new MySqlDevartOptions { ConnectionString = "Server=localhost;" };

        // Act
        var result = options.ConnectionString;

        // Assert
        _ = await Assert.That(result).IsEqualTo("Server=localhost;");
    }

    [Test]
    public async Task Timeout_WhenNotSet_ShouldReturnDefault()
    {
        // Arrange
        var options = new MySqlDevartOptions();

        // Act
        var result = options.Timeout;

        // Assert
        _ = await Assert.That(result).IsEqualTo(100);
    }

    [Test]
    public async Task Timeout_WhenSet_ShouldReturnValue()
    {
        // Arrange
        var options = new MySqlDevartOptions { Timeout = 1000 };

        // Act
        var result = options.Timeout;

        // Assert
        _ = await Assert.That(result).IsEqualTo(1000);
    }

    [Test]
    public async Task Command_WhenNotSet_ShouldReturnDefault()
    {
        // Arrange
        var options = new MySqlDevartOptions();

        // Act
        var result = options.Command;

        // Assert
        _ = await Assert.That(result).IsEqualTo(MySqlDevartHealthCheck.DefaultCommand);
    }

    [Test]
    public async Task Command_WhenSet_ShouldReturnValue()
    {
        // Arrange
        var options = new MySqlDevartOptions { Command = "SELECT 2;" };

        // Act
        var result = options.Command;

        // Assert
        _ = await Assert.That(result).IsEqualTo("SELECT 2;");
    }
}
