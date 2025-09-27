namespace NetEvolve.HealthChecks.Tests.Unit.Azure.IotHub;

using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.IotHub;

[TestGroup($"{nameof(Azure)}.{nameof(IotHub)}")]
public sealed class IotHubClientFactoryTests
{
    [Test]
    public void CreateServiceClient_WhenInvalidMode_ThrowUnreachableException()
    {
        // Arrange
        var options = new IotHubAvailabilityOptions { Mode = (ClientCreationMode)13 };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var factory = new IotHubClientFactory();

        // Act & Assert
        _ = Assert.Throws<UnreachableException>(() => factory.CreateServiceClient(options, serviceProvider));
    }

    [Test]
    public void CreateServiceClient_WhenModeServiceProviderAndNoServiceRegistered_ThrowInvalidOperationException()
    {
        // Arrange
        var options = new IotHubAvailabilityOptions { Mode = ClientCreationMode.ServiceProvider };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var factory = new IotHubClientFactory();

        // Act & Assert
        _ = Assert.Throws<InvalidOperationException>(() => factory.CreateServiceClient(options, serviceProvider));
    }

    [Test]
    public void CreateServiceClient_WhenModeConnectionStringAndConnectionStringNull_ThrowArgumentException()
    {
        // Arrange
        var options = new IotHubAvailabilityOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = null,
        };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var factory = new IotHubClientFactory();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() => factory.CreateServiceClient(options, serviceProvider));
    }

    [Test]
    public void CreateServiceClient_WhenModeDefaultAzureCredentialsAndHostnameNull_ThrowArgumentException()
    {
        // Arrange
        var options = new IotHubAvailabilityOptions
        {
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedHostname = null,
        };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var factory = new IotHubClientFactory();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() => factory.CreateServiceClient(options, serviceProvider));
    }

    [Test]
    public void CreateServiceClient_WhenModeConnectionStringAndValidConnectionString_ReturnsServiceClient()
    {
        // Arrange
        var options = new IotHubAvailabilityOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString =
                "HostName=test.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=dGVzdGtleQ==",
        };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var factory = new IotHubClientFactory();

        // Act
        var client = factory.CreateServiceClient(options, serviceProvider);

        // Assert
        Assert.That(client, Is.Not.Null);
    }

    [Test]
    public void CreateServiceClient_WhenModeDefaultAzureCredentialsAndValidHostname_ReturnsServiceClient()
    {
        // Arrange
        var options = new IotHubAvailabilityOptions
        {
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedHostname = "test.azure-devices.net",
        };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var factory = new IotHubClientFactory();

        // Act
        var client = factory.CreateServiceClient(options, serviceProvider);

        // Assert
        Assert.That(client, Is.Not.Null);
    }
}
