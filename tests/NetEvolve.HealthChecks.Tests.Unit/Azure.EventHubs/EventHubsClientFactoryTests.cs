namespace NetEvolve.HealthChecks.Tests.Unit.Azure.EventHubs;

using System;
using System.Diagnostics;
using global::Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.EventHubs;
using TUnit.Mocks;

[TestGroup($"{nameof(Azure)}.{nameof(EventHubs)}")]
public sealed class EventHubsClientFactoryTests
{
    [Test]
    public async Task GetClient_WhenModeIsConnectionString_ShouldCreateNewClient()
    {
        // Arrange
        const string connectionString =
            "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=testkey;EntityPath=eventhub1";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = connectionString,
            EventHubName = "eventhub1",
        };
        var serviceProvider = IServiceProvider.Mock();
        using var clientFactory = new EventHubsClientFactory();

        // Act
        var client = clientFactory.GetClient(
            nameof(GetClient_WhenModeIsConnectionString_ShouldCreateNewClient),
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
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedNamespace = fullyQualifiedNamespace,
            EventHubName = "eventhub1",
        };
        var serviceProvider = IServiceProvider.Mock();
        using var clientFactory = new EventHubsClientFactory();

        // Act
        var client = clientFactory.GetClient(
            nameof(GetClient_WhenModeIsDefaultAzureCredentials_ShouldCreateNewClient),
            options,
            serviceProvider
        );

        // Assert
        _ = await Assert.That(client).IsNotNull();
    }

    [Test]
    public async Task GetClient_WhenModeIsServiceProvider_ShouldReturnRegisteredClient()
    {
        // Arrange
        var mockClient = EventHubProducerClient.Mock();
        // Convert once to the real type and reuse that instance everywhere below - each implicit
        // conversion from the mock wrapper to the real type can mint a distinct proxy instance, so
        // converting again later (e.g. at the assertion) would compare against a different object.
        EventHubProducerClient registeredClient = mockClient;
        var services = new ServiceCollection().AddSingleton(registeredClient);
        await using var serviceProvider = services.BuildServiceProvider();

        var options = new EventHubsOptions { Mode = ClientCreationMode.ServiceProvider, EventHubName = "eventhub1" };
        using var clientFactory = new EventHubsClientFactory();

        // Act
        var client = clientFactory.GetClient(
            nameof(GetClient_WhenModeIsServiceProvider_ShouldReturnRegisteredClient),
            options,
            serviceProvider
        );

        // Assert
        _ = await Assert.That(client).IsSameReferenceAs(registeredClient);
    }

    [Test]
    public async Task GetClient_CalledTwiceWithSameName_ShouldReturnSameInstance()
    {
        // Arrange
        const string connectionString =
            "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=testkey;EntityPath=eventhub1";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = connectionString,
            EventHubName = "eventhub1",
        };
        var serviceProvider = IServiceProvider.Mock();
        using var clientFactory = new EventHubsClientFactory();

        // Act
        var client1 = clientFactory.GetClient(
            nameof(GetClient_CalledTwiceWithSameName_ShouldReturnSameInstance),
            options,
            serviceProvider
        );
        var client2 = clientFactory.GetClient(
            nameof(GetClient_CalledTwiceWithSameName_ShouldReturnSameInstance),
            options,
            serviceProvider
        );

        // Assert
        _ = await Assert.That(client1).IsSameReferenceAs(client2);
    }

    [Test]
    public async Task GetClient_CalledWithDifferentNames_ShouldReturnDifferentInstances()
    {
        // Arrange
        const string connectionString =
            "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=testkey;EntityPath=eventhub1";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = connectionString,
            EventHubName = "eventhub1",
        };
        var serviceProvider = IServiceProvider.Mock();
        using var clientFactory = new EventHubsClientFactory();

        // Act
        var client1 = clientFactory.GetClient("client1", options, serviceProvider);
        var client2 = clientFactory.GetClient("client2", options, serviceProvider);

        // Assert
        _ = await Assert.That(client1).IsNotSameReferenceAs(client2);
    }

    [Test]
    public async Task GetClient_CaseInsensitiveNames_ShouldReturnSameInstance()
    {
        // Arrange
        const string connectionString =
            "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=testkey;EntityPath=eventhub1";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = connectionString,
            EventHubName = "eventhub1",
        };
        var serviceProvider = IServiceProvider.Mock();
        using var clientFactory = new EventHubsClientFactory();

        // Act
        var client1 = clientFactory.GetClient("TestClient", options, serviceProvider);
        var client2 = clientFactory.GetClient("testclient", options, serviceProvider);

        // Assert
        _ = await Assert.That(client1).IsSameReferenceAs(client2);
    }

    [Test]
    public void GetClient_WhenModeIsInvalid_ShouldThrowUnreachableException()
    {
        // Arrange
        var options = new EventHubsOptions { Mode = (ClientCreationMode)int.MaxValue, EventHubName = "eventhub1" };
        var serviceProvider = IServiceProvider.Mock();
        using var clientFactory = new EventHubsClientFactory();

        // Act
        void Act() =>
            clientFactory.GetClient(
                nameof(GetClient_WhenModeIsInvalid_ShouldThrowUnreachableException),
                options,
                serviceProvider
            );

        // Assert
        _ = Assert.Throws<UnreachableException>(Act);
    }

    [Test]
    public void GetClient_WhenModeIsServiceProviderButClientNotRegistered_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var options = new EventHubsOptions { Mode = ClientCreationMode.ServiceProvider, EventHubName = "eventhub1" };
        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        using var clientFactory = new EventHubsClientFactory();

        // Act
        void Act() =>
            clientFactory.GetClient(
                nameof(GetClient_WhenModeIsServiceProviderButClientNotRegistered_ShouldThrowInvalidOperationException),
                options,
                serviceProvider
            );

        // Assert
        _ = Assert.Throws<InvalidOperationException>(Act);
    }
}
