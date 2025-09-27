namespace NetEvolve.HealthChecks.Tests.Unit.Azure.IotHub;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.IotHub;

[TestGroup($"{nameof(Azure)}.{nameof(IotHub)}")]
public class IotHubAvailabilityOptionsTests
{
    [Test]
    public void Validate_WhenNameNull_ExpectValidationError()
    {
        // Arrange
        const string? name = null;
        var options = new IotHubAvailabilityOptions();

        // Act
        var result = IotHubOptionsBase.InternalValidate(name, options);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.FailureMessage, Is.EqualTo("The name cannot be null or whitespace."));
    }

    [Test]
    public void Validate_WhenNameEmpty_ExpectValidationError()
    {
        // Arrange
        var name = string.Empty;
        var options = new IotHubAvailabilityOptions();

        // Act
        var result = IotHubOptionsBase.InternalValidate(name, options);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.FailureMessage, Is.EqualTo("The name cannot be null or whitespace."));
    }

    [Test]
    public void Validate_WhenOptionsNull_ExpectValidationError()
    {
        // Arrange
        const string name = "Test";
        const IotHubAvailabilityOptions? options = null;

        // Act
        var result = IotHubOptionsBase.InternalValidate(name, options);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.FailureMessage, Is.EqualTo("The option cannot be null."));
    }

    [Test]
    public void Validate_WhenTimeoutTooLow_ExpectValidationError()
    {
        // Arrange
        const string name = "Test";
        var options = new IotHubAvailabilityOptions { Timeout = -2 };

        // Act
        var result = IotHubOptionsBase.InternalValidate(name, options);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(
            result.FailureMessage,
            Is.EqualTo("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.")
        );
    }

    [Test]
    public void Validate_WhenModeNull_ExpectValidationError()
    {
        // Arrange
        const string name = "Test";
        var options = new IotHubAvailabilityOptions { Mode = null };

        // Act
        var result = IotHubOptionsBase.InternalValidate(name, options);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.FailureMessage, Is.EqualTo("The client creation mode cannot be null."));
    }

    [Test]
    public void Validate_WhenModeDefaultAzureCredentialsAndHostnameNull_ExpectValidationError()
    {
        // Arrange
        const string name = "Test";
        var options = new IotHubAvailabilityOptions
        {
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedHostname = null,
        };

        // Act
        var result = IotHubOptionsBase.InternalValidate(name, options);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(
            result.FailureMessage,
            Is.EqualTo("The fully qualified hostname cannot be null or whitespace when using DefaultAzureCredentials.")
        );
    }

    [Test]
    public void Validate_WhenModeConnectionStringAndConnectionStringNull_ExpectValidationError()
    {
        // Arrange
        const string name = "Test";
        var options = new IotHubAvailabilityOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = null,
        };

        // Act
        var result = IotHubOptionsBase.InternalValidate(name, options);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(
            result.FailureMessage,
            Is.EqualTo("The connection string cannot be null or whitespace when using ConnectionString.")
        );
    }

    [Test]
    public void Validate_WhenValidConnectionStringOptions_ExpectSuccess()
    {
        // Arrange
        const string name = "Test";
        var options = new IotHubAvailabilityOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = "HostName=test.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=test",
        };

        // Act
        var result = IotHubOptionsBase.InternalValidate(name, options);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Validate_WhenValidDefaultAzureCredentialsOptions_ExpectSuccess()
    {
        // Arrange
        const string name = "Test";
        var options = new IotHubAvailabilityOptions
        {
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedHostname = "test.azure-devices.net",
        };

        // Act
        var result = IotHubOptionsBase.InternalValidate(name, options);

        // Assert
        Assert.That(result, Is.Null);
    }
}
