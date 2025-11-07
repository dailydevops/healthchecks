namespace NetEvolve.HealthChecks.Tests.Unit.Azure.EventHubs;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.EventHubs;

[TestGroup($"{nameof(Azure)}.{nameof(EventHubs)}")]
public sealed class EventHubsOptionsTests
{
    [Test]
    public async Task Validate_WhenNameNull_ReturnFailure()
    {
        // Arrange
        var options = new EventHubsOptions();

        // Act
        var result = EventHubsOptions.InternalValidate(null, options);

        // Assert
        _ = await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task Validate_WhenOptionsNull_ReturnFailure()
    {
        // Arrange
        EventHubsOptions options = null!;

        // Act
        var result = EventHubsOptions.InternalValidate("Test", options);

        // Assert
        _ = await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task Validate_WhenTimeoutNegative_ReturnFailure()
    {
        // Arrange
        var options = new EventHubsOptions { Timeout = -2 };

        // Act
        var result = EventHubsOptions.InternalValidate("Test", options);

        // Assert
        _ = await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task Validate_WhenModeNull_ReturnFailure()
    {
        // Arrange
        var options = new EventHubsOptions { Mode = null };

        // Act
        var result = EventHubsOptions.InternalValidate("Test", options);

        // Assert
        _ = await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task Validate_WhenModeDefaultAzureCredentialsAndNamespaceNull_ReturnFailure()
    {
        // Arrange
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedNamespace = null,
        };

        // Act
        var result = EventHubsOptions.InternalValidate("Test", options);

        // Assert
        _ = await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task Validate_WhenModeConnectionStringAndConnectionStringNull_ReturnFailure()
    {
        // Arrange
        var options = new EventHubsOptions { Mode = ClientCreationMode.ConnectionString, ConnectionString = null };

        // Act
        var result = EventHubsOptions.InternalValidate("Test", options);

        // Assert
        _ = await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task Validate_WhenEventHubNameNull_ReturnFailure()
    {
        // Arrange
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = "Test",
            EventHubName = null,
        };

        // Act
        var result = EventHubsOptions.InternalValidate("Test", options);

        // Assert
        _ = await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task Validate_WhenValid_ReturnSuccess()
    {
        // Arrange
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = "Test",
            EventHubName = "Test",
        };

        // Act
        var result = EventHubsOptions.InternalValidate("Test", options);

        // Assert
        _ = await Assert.That(result).IsNull();
    }
}
