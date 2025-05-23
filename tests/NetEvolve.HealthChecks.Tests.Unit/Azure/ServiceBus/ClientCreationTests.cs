namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ServiceBus;

using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;
using NSubstitute;
using Xunit;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
public sealed class ClientCreationTests
{
    [Fact]
    public void GetClient_WhenModeIsConnectionString_ShouldCreateNewClient()
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

        // Act
        var client = ClientCreation.GetClient(
            nameof(GetClient_WhenModeIsDefaultAzureCredentials_ShouldCreateNewClient),
            options,
            serviceProvider
        );

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void GetClient_WhenModeIsDefaultAzureCredentials_ShouldCreateNewClient()
    {
        // Arrange
        const string fullyQualifiedNamespace = "test.servicebus.windows.net";
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedNamespace = fullyQualifiedNamespace,
        };
        var serviceProvider = Substitute.For<IServiceProvider>();

        // Act
        var client = ClientCreation.GetClient(
            nameof(GetClient_WhenModeIsDefaultAzureCredentials_ShouldCreateNewClient),
            options,
            serviceProvider
        );

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void GetClient_CalledTwiceWithSameName_ShouldReturnSameInstance()
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

        // Act
        var client1 = ClientCreation.GetClient(
            nameof(GetClient_CalledTwiceWithSameName_ShouldReturnSameInstance),
            options,
            serviceProvider
        );
        var client2 = ClientCreation.GetClient(
            nameof(GetClient_CalledTwiceWithSameName_ShouldReturnSameInstance),
            options,
            serviceProvider
        );

        // Assert
        Assert.Same(client1, client2);
    }

    [Fact]
    public void GetAdministrationClient_WhenModeIsConnectionString_ShouldCreateNewClient()
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

        // Act
        var client = ClientCreation.GetAdministrationClient(
            nameof(GetAdministrationClient_WhenModeIsConnectionString_ShouldCreateNewClient),
            options,
            serviceProvider
        );

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void GetAdministrationClient_WhenModeIsDefaultAzureCredentials_ShouldCreateNewClient()
    {
        // Arrange
        const string fullyQualifiedNamespace = "test.servicebus.windows.net";
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedNamespace = fullyQualifiedNamespace,
        };
        var serviceProvider = Substitute.For<IServiceProvider>();

        // Act
        var client = ClientCreation.GetAdministrationClient(
            nameof(GetAdministrationClient_WhenModeIsDefaultAzureCredentials_ShouldCreateNewClient),
            options,
            serviceProvider
        );

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void GetAdministrationClient_CalledTwiceWithSameName_ShouldReturnSameInstance()
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

        // Act
        var client1 = ClientCreation.GetAdministrationClient(
            nameof(GetAdministrationClient_CalledTwiceWithSameName_ShouldReturnSameInstance),
            options,
            serviceProvider
        );
        var client2 = ClientCreation.GetAdministrationClient(
            nameof(GetAdministrationClient_CalledTwiceWithSameName_ShouldReturnSameInstance),
            options,
            serviceProvider
        );

        // Assert
        Assert.Same(client1, client2);
    }

    [Fact]
    public void GetClient_WhenModeIsInvalid_ShouldThrowUnreachableException()
    {
        // Arrange
        var options = new ServiceBusQueueOptions { Mode = (ClientCreationMode)int.MaxValue };
        var serviceProvider = Substitute.For<IServiceProvider>();

        // Act
        void Act() =>
            ClientCreation.GetClient(
                nameof(GetClient_WhenModeIsInvalid_ShouldThrowUnreachableException),
                options,
                serviceProvider
            );

        // Assert
        _ = Assert.Throws<UnreachableException>(Act);
    }

    [Fact]
    public void GetAdministrationClient_WhenModeIsInvalid_ShouldThrowUnreachableException()
    {
        // Arrange
        var options = new ServiceBusQueueOptions { Mode = (ClientCreationMode)int.MaxValue };
        var serviceProvider = Substitute.For<IServiceProvider>();

        // Act
        void Act() =>
            ClientCreation.GetAdministrationClient(
                nameof(GetAdministrationClient_WhenModeIsInvalid_ShouldThrowUnreachableException),
                options,
                serviceProvider
            );

        // Assert
        _ = Assert.Throws<UnreachableException>(Act);
    }

    [Fact]
    public void GetClient_WhenServiceProviderIsNull_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var options = new ServiceBusQueueOptions { Mode = ClientCreationMode.ServiceProvider };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        void Act() =>
            ClientCreation.GetClient(
                nameof(GetClient_WhenServiceProviderIsNull_ShouldThrowInvalidOperationException),
                options,
                serviceProvider
            );

        // Assert
        _ = Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void GetAdministrationClient_WhenServiceProviderIsNull_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var options = new ServiceBusQueueOptions { Mode = ClientCreationMode.ServiceProvider };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        void Act() =>
            ClientCreation.GetAdministrationClient(
                nameof(GetAdministrationClient_WhenServiceProviderIsNull_ShouldThrowInvalidOperationException),
                options,
                serviceProvider
            );

        // Assert
        _ = Assert.Throws<InvalidOperationException>(Act);
    }
}
