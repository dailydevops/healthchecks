namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ServiceBus;

using System;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.Azure.ServiceBus;
using NSubstitute;
using Xunit;

[TestGroup($"{nameof(Azure)}.{nameof(ServiceBus)}")]
public sealed class ClientCreationTests
{
    private const string TestName = "TestName";

    [Fact]
    public void GetClient_WhenModeIsConnectionString_ShouldCreateNewClient()
    {
        // Arrange
        const string connectionString =
            "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=getclient-connectionstring;SharedAccessKey=testkey";
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = connectionString,
        };
        var serviceProvider = Substitute.For<IServiceProvider>();

        // Act
        var client = ClientCreation.GetClient(TestName, options, serviceProvider);

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void GetClient_WhenModeIsDefaultAzureCredentials_ShouldCreateNewClient()
    {
        // Arrange
        const string fullyQualifiedNamespace = "getclient-default.servicebus.windows.net";
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedNamespace = fullyQualifiedNamespace,
        };
        var serviceProvider = Substitute.For<IServiceProvider>();

        // Act
        var client = ClientCreation.GetClient(TestName, options, serviceProvider);

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void GetClient_CalledTwiceWithSameName_ShouldReturnSameInstance()
    {
        // Arrange
        const string connectionString =
            "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=getclient-samename;SharedAccessKey=testkey";
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = connectionString,
        };
        var serviceProvider = Substitute.For<IServiceProvider>();

        // Act
        var client1 = ClientCreation.GetClient(TestName, options, serviceProvider);
        var client2 = ClientCreation.GetClient(TestName, options, serviceProvider);

        // Assert
        Assert.Same(client1, client2);
    }

    [Fact]
    public void GetAdministrationClient_WhenModeIsConnectionString_ShouldCreateNewClient()
    {
        // Arrange
        const string connectionString =
            "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=getadministrationclient-connectionstring;SharedAccessKey=testkey";
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = connectionString,
        };
        var serviceProvider = Substitute.For<IServiceProvider>();

        // Act
        var client = ClientCreation.GetAdministrationClient(TestName, options, serviceProvider);

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void GetAdministrationClient_WhenModeIsDefaultAzureCredentials_ShouldCreateNewClient()
    {
        // Arrange
        const string fullyQualifiedNamespace = "getadministrationclient-default.servicebus.windows.net";
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedNamespace = fullyQualifiedNamespace,
        };
        var serviceProvider = Substitute.For<IServiceProvider>();

        // Act
        var client = ClientCreation.GetAdministrationClient(TestName, options, serviceProvider);

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void GetAdministrationClient_CalledTwiceWithSameName_ShouldReturnSameInstance()
    {
        // Arrange
        const string connectionString =
            "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=getadministrationclient-samename;SharedAccessKey=testkey";
        var options = new ServiceBusQueueOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = connectionString,
        };
        var serviceProvider = Substitute.For<IServiceProvider>();

        // Act
        var client1 = ClientCreation.GetAdministrationClient(TestName, options, serviceProvider);
        var client2 = ClientCreation.GetAdministrationClient(TestName, options, serviceProvider);

        // Assert
        Assert.Same(client1, client2);
    }
}
