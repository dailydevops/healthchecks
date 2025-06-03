namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ServiceBus;

using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;
using NSubstitute;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
public sealed class ServiceBusClientFactoryTests
{
    [Test]
    public async Task GetClient_WhenModeIsConnectionString_ShouldCreateNewClient()
    {
        // Arrange
        const string connectionString =
            "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=testname;SharedAccessKey=testkey";
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = connectionString,
        };
        var serviceProvider = Substitute.For<IServiceProvider>();
        var clientCreation = new ServiceBusClientFactory();

        // Act
        var client = clientCreation.GetClient(
            nameof(GetClient_WhenModeIsDefaultAzureCredentials_ShouldCreateNewClient),
            options,
            serviceProvider
        );

        // Assert
        _ = await Assert.That(client).IsNotNull();
    }

    [Test]
    public async Task GetClient_WhenModeIsDefaultAzureCredentials_ShouldCreateNewClient()
    {
        // Arrange
        const string fullyQualifiedNamespace = "test.servicebus.windows.net";
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedNamespace = fullyQualifiedNamespace,
        };
        var serviceProvider = Substitute.For<IServiceProvider>();
        var clientCreation = new ServiceBusClientFactory();

        // Act
        var client = clientCreation.GetClient(
            nameof(GetClient_WhenModeIsDefaultAzureCredentials_ShouldCreateNewClient),
            options,
            serviceProvider
        );

        // Assert
        _ = await Assert.That(client).IsNotNull();
    }

    [Test]
    public async Task GetClient_CalledTwiceWithSameName_ShouldReturnSameInstance()
    {
        // Arrange
        const string connectionString =
            "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=testname;SharedAccessKey=testkey";
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = connectionString,
        };
        var serviceProvider = Substitute.For<IServiceProvider>();
        var clientCreation = new ServiceBusClientFactory();

        // Act
        var client1 = clientCreation.GetClient(
            nameof(GetClient_CalledTwiceWithSameName_ShouldReturnSameInstance),
            options,
            serviceProvider
        );
        var client2 = clientCreation.GetClient(
            nameof(GetClient_CalledTwiceWithSameName_ShouldReturnSameInstance),
            options,
            serviceProvider
        );

        // Assert
        _ = await Assert.That(client1).IsSameReferenceAs(client2);
    }

    [Test]
    public async Task GetAdministrationClient_WhenModeIsConnectionString_ShouldCreateNewClient()
    {
        // Arrange
        const string connectionString =
            "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=testname;SharedAccessKey=testkey";
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = connectionString,
        };
        var serviceProvider = Substitute.For<IServiceProvider>();
        var clientCreation = new ServiceBusClientFactory();

        // Act
        var client = clientCreation.GetAdministrationClient(
            nameof(GetAdministrationClient_WhenModeIsConnectionString_ShouldCreateNewClient),
            options,
            serviceProvider
        );

        // Assert
        _ = await Assert.That(client).IsNotNull();
    }

    [Test]
    public async Task GetAdministrationClient_WhenModeIsDefaultAzureCredentials_ShouldCreateNewClient()
    {
        // Arrange
        const string fullyQualifiedNamespace = "test.servicebus.windows.net";
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedNamespace = fullyQualifiedNamespace,
        };
        var serviceProvider = Substitute.For<IServiceProvider>();
        var clientCreation = new ServiceBusClientFactory();

        // Act
        var client = clientCreation.GetAdministrationClient(
            nameof(GetAdministrationClient_WhenModeIsDefaultAzureCredentials_ShouldCreateNewClient),
            options,
            serviceProvider
        );

        // Assert
        _ = await Assert.That(client).IsNotNull();
    }

    [Test]
    public async Task GetAdministrationClient_CalledTwiceWithSameName_ShouldReturnSameInstance()
    {
        // Arrange
        const string connectionString =
            "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=testname;SharedAccessKey=testkey";
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = connectionString,
        };
        var serviceProvider = Substitute.For<IServiceProvider>();
        var clientCreation = new ServiceBusClientFactory();

        // Act
        var client1 = clientCreation.GetAdministrationClient(
            nameof(GetAdministrationClient_CalledTwiceWithSameName_ShouldReturnSameInstance),
            options,
            serviceProvider
        );
        var client2 = clientCreation.GetAdministrationClient(
            nameof(GetAdministrationClient_CalledTwiceWithSameName_ShouldReturnSameInstance),
            options,
            serviceProvider
        );

        // Assert
        _ = await Assert.That(client1).IsSameReferenceAs(client2);
    }

    [Test]
    public void GetClient_WhenModeIsInvalid_ShouldThrowUnreachableException()
    {
        // Arrange
        var options = new ServiceBusQueueOptions { Mode = (ClientCreationMode)int.MaxValue };
        var serviceProvider = Substitute.For<IServiceProvider>();
        var clientCreation = new ServiceBusClientFactory();

        // Act
        void Act() =>
            clientCreation.GetClient(
                nameof(GetClient_WhenModeIsInvalid_ShouldThrowUnreachableException),
                options,
                serviceProvider
            );

        // Assert
        _ = Assert.Throws<UnreachableException>(Act);
    }

    [Test]
    public void GetAdministrationClient_WhenModeIsInvalid_ShouldThrowUnreachableException()
    {
        // Arrange
        var options = new ServiceBusQueueOptions { Mode = (ClientCreationMode)int.MaxValue };
        var serviceProvider = Substitute.For<IServiceProvider>();
        var clientCreation = new ServiceBusClientFactory();

        // Act
        void Act() =>
            clientCreation.GetAdministrationClient(
                nameof(GetAdministrationClient_WhenModeIsInvalid_ShouldThrowUnreachableException),
                options,
                serviceProvider
            );

        // Assert
        _ = Assert.Throws<UnreachableException>(Act);
    }

    [Test]
    public void GetClient_WhenServiceProviderIsNull_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var options = new ServiceBusQueueOptions { Mode = ClientCreationMode.ServiceProvider };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var clientCreation = new ServiceBusClientFactory();

        // Act
        void Act() =>
            clientCreation.GetClient(
                nameof(GetClient_WhenServiceProviderIsNull_ShouldThrowInvalidOperationException),
                options,
                serviceProvider
            );

        // Assert
        _ = Assert.Throws<InvalidOperationException>(Act);
    }

    [Test]
    public void GetAdministrationClient_WhenServiceProviderIsNull_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var options = new ServiceBusQueueOptions { Mode = ClientCreationMode.ServiceProvider };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var clientCreation = new ServiceBusClientFactory();

        // Act
        void Act() =>
            clientCreation.GetAdministrationClient(
                nameof(GetAdministrationClient_WhenServiceProviderIsNull_ShouldThrowInvalidOperationException),
                options,
                serviceProvider
            );

        // Assert
        _ = Assert.Throws<InvalidOperationException>(Act);
    }
}
