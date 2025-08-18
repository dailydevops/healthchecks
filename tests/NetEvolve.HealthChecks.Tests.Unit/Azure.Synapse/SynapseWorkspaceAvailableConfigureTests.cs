namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Synapse;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Synapse;

[TestGroup($"{nameof(Azure)}.{nameof(Synapse)}")]
public class SynapseWorkspaceAvailableConfigureTests
{
    [Test]
    public void Validate_WhenArgumentsAreValidForConnectionString_ShouldReturnSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new SynapseWorkspaceAvailableConfigure(configuration, serviceProvider);
        var options = new SynapseWorkspaceAvailableOptions
        {
            ConnectionString = "Endpoint=https://myworkspace.dev.azuresynapse.net;",
            Mode = SynapseClientCreationMode.ConnectionString,
            Timeout = 100
        };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Test]
    public void Validate_WhenArgumentsAreValidForDefaultAzureCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new SynapseWorkspaceAvailableConfigure(configuration, serviceProvider);
        var options = new SynapseWorkspaceAvailableOptions
        {
            WorkspaceUri = new Uri("https://myworkspace.dev.azuresynapse.net"),
            Mode = SynapseClientCreationMode.DefaultAzureCredentials,
            Timeout = 100
        };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Test]
    public void Validate_WhenConnectionStringIsInvalid_ShouldReturnFailure()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new SynapseWorkspaceAvailableConfigure(configuration, serviceProvider);
        var options = new SynapseWorkspaceAvailableOptions
        {
            ConnectionString = "InvalidConnectionString",
            Mode = SynapseClientCreationMode.ConnectionString,
            Timeout = 100
        };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("Endpoint", result.FailureMessage);
    }

    [Test]
    public void Validate_WhenWorkspaceUriIsNull_ShouldReturnFailure()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var configure = new SynapseWorkspaceAvailableConfigure(configuration, serviceProvider);
        var options = new SynapseWorkspaceAvailableOptions
        {
            WorkspaceUri = null,
            Mode = SynapseClientCreationMode.DefaultAzureCredentials,
            Timeout = 100
        };

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("workspace uri cannot be null", result.FailureMessage);
    }
}