namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Synapse;

using System;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Synapse;

[TestGroup($"{nameof(Azure)}.{nameof(Synapse)}")]
public class SynapseWorkspaceAvailableOptionsTests
{
    [Test]
    public void Validate_WhenArgumentsAreValid_ShouldReturnExpected()
    {
        // Arrange
        var options = new SynapseWorkspaceAvailableOptions
        {
            ConnectionString = "Endpoint=https://myworkspace.dev.azuresynapse.net;",
            Mode = SynapseClientCreationMode.ConnectionString,
            Timeout = 100
        };

        // Act
        var result = options.ConnectionString;

        // Assert
        Assert.Equal("Endpoint=https://myworkspace.dev.azuresynapse.net;", result);
    }

    [Test]
    public void Validate_WhenArgumentWorkspaceUriIsValid_ShouldReturnExpected()
    {
        // Arrange
        var options = new SynapseWorkspaceAvailableOptions
        {
            WorkspaceUri = new Uri("https://myworkspace.dev.azuresynapse.net"),
            Mode = SynapseClientCreationMode.DefaultAzureCredentials,
            Timeout = 100
        };

        // Act
        var result = options.WorkspaceUri;

        // Assert
        Assert.Equal(new Uri("https://myworkspace.dev.azuresynapse.net"), result);
    }

    [Test]
    public void Validate_WhenTimeoutIsDefault_ShouldReturn100()
    {
        // Arrange
        var options = new SynapseWorkspaceAvailableOptions();

        // Act
        var result = options.Timeout;

        // Assert
        Assert.Equal(100, result);
    }
}