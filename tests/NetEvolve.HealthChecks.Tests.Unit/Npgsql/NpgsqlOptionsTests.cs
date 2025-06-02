namespace NetEvolve.HealthChecks.Tests.Unit.Npgsql;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Npgsql;

[TestGroup(nameof(Npgsql))]
public sealed class NpgsqlOptionsTests
{
    [Test]
    public async Task Constructor_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var options = new NpgsqlOptions();

        // Assert

        using (Assert.Multiple())
        {
            _ = await Assert.That(options.ConnectionString).IsNull();
            _ = await Assert.That(options.Timeout).IsEqualTo(100);
            _ = await Assert.That(options.Command).IsEqualTo(NpgsqlHealthCheck.DefaultCommand);
        }
    }

    [Test]
    public async Task ConnectionString_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new NpgsqlOptions();
        const string testValue = "test-connection-string";

        // Act
        options.ConnectionString = testValue;

        // Assert
        _ = await Assert.That(options.ConnectionString).IsEqualTo(testValue);
    }

    [Test]
    public async Task Timeout_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new NpgsqlOptions();
        const int testValue = 5000;

        // Act
        options.Timeout = testValue;

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(testValue);
    }

    [Test]
    public async Task Command_SetAndGet_WorksCorrectly()
    {
        // Arrange
        var options = new NpgsqlOptions();
        const string testValue = "Test";

        // Act
        options.Command = testValue;

        // Assert
        _ = await Assert.That(options.Command).IsEqualTo(testValue);
    }

    [Test]
    public async Task Instance_CanBeShallowCopied()
    {
        var options = new NpgsqlOptions();
        var options2 = options with { };

        using (Assert.Multiple())
        {
            _ = await Assert.That(options2).IsNotNull();
            _ = await Assert.That(options2).IsNotSameReferenceAs(options);
            _ = await Assert.That(options2.ConnectionString).IsEqualTo(options.ConnectionString);
            _ = await Assert.That(options2.Timeout).IsEqualTo(options.Timeout);
            _ = await Assert.That(options2.Command).IsEqualTo(options.Command);
        }
    }
}
