namespace NetEvolve.HealthChecks.Tests.Unit.RabbitMQ;

using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.RabbitMQ;
using Xunit;

[TestGroup(nameof(RabbitMQ))]
public class DependencyInjectionExtensionsTests
{
    [Fact]
    public void AddRabbitMQ_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => builder.AddRabbitMQ("Test");

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Fact]
    public void AddRabbitMQ_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string? name = default;

        // Act
        void Act() => builder.AddRabbitMQ(name!);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Fact]
    public void AddRabbitMQ_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var name = string.Empty;

        // Act
        void Act() => builder.AddRabbitMQ(name);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Fact]
    public void AddRabbitMQ_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var tags = default(string[]);

        // Act
        void Act() => builder.AddRabbitMQ("Test", tags: tags);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("tags", Act);
    }

    [Fact]
    public void AddRabbitMQ_WhenArgumentNameIsAlreadyUsed_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";

        // Act
        void Act() => builder.AddRabbitMQ(name).AddRabbitMQ(name);

        // Assert
        _ = Assert.Throws<ArgumentException>(nameof(name), Act);
    }

    [Fact]
    public void AddRabbitMQ_WithValidArguments_RegistersServices()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";

        // Act
        var result = builder.AddRabbitMQ(name);

        // Assert
        Assert.NotNull(result);
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();
        var registrations = options?.Value?.Registrations;

        Assert.NotNull(registrations);
        Assert.Contains(registrations, r => r.Name == name);
    }

    [Fact]
    public void AddRabbitMQ_WithOptions_ConfiguresOptions()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";
        const int timeout = 200;

        // Act
        var result = builder.AddRabbitMQ(name, options => options.Timeout = timeout);

        // Assert
        Assert.NotNull(result);
        var serviceProvider = services.BuildServiceProvider();
        var optionsSnapshot = serviceProvider.GetService<IOptionsSnapshot<RabbitMQOptions>>();
        var options = optionsSnapshot?.Get(name);

        Assert.NotNull(options);
        Assert.Equal(timeout, options.Timeout);
    }

    [Fact]
    public void AddRabbitMQ_WithCustomTags_IncludesDefaultAndCustomTags()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";
        var customTags = new[] { "custom1", "custom2" };

        // Act
        var result = builder.AddRabbitMQ(name, tags: customTags);

        // Assert
        Assert.NotNull(result);
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();
        var registration = options?.Value?.Registrations.FirstOrDefault(r => r.Name == name);

        Assert.NotNull(registration);
        Assert.Contains("rabbitmq", registration.Tags);
        Assert.Contains("messaging", registration.Tags);
        Assert.Contains("custom1", registration.Tags);
        Assert.Contains("custom2", registration.Tags);
    }

    [Fact]
    public void AddRabbitMQ_CalledMultipleTimesWithDifferentNames_RegistersMultipleChecks()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name1 = "Test1";
        const string name2 = "Test2";

        // Act
        var result = builder.AddRabbitMQ(name1).AddRabbitMQ(name2);

        // Assert
        Assert.NotNull(result);
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();
        var registrations = options?.Value?.Registrations;

        Assert.NotNull(registrations);
        Assert.Contains(registrations, r => r.Name == name1);
        Assert.Contains(registrations, r => r.Name == name2);
    }
}
