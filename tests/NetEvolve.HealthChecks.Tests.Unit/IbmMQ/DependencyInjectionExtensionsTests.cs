namespace NetEvolve.HealthChecks.Tests.Unit.IbmMQ;

using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.IbmMQ;

[TestGroup(nameof(IbmMQ))]
public sealed class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddIbmMQ_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        var builder = default(IHealthChecksBuilder);

        // Act
        void Act() => builder!.AddIbmMQ("Test");

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Test]
    public void AddIbmMQ_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string? name = default;

        // Act
        void Act() => builder.AddIbmMQ(name!);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
    public void AddIbmMQ_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var name = string.Empty;

        // Act
        void Act() => builder.AddIbmMQ(name);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void AddIbmMQ_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var tags = default(string[]);

        // Act
        void Act() => builder.AddIbmMQ("Test", tags: tags!);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("tags", Act);
    }

    [Test]
    public void AddIbmMQ_WhenArgumentNameIsAlreadyUsed_ThrowArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";

        // Act
        void Act() => builder.AddIbmMQ(name).AddIbmMQ(name);

        // Assert
        _ = Assert.Throws<ArgumentException>(nameof(name), Act);
    }

    [Test]
    public async Task AddIbmMQ_WithValidArguments_RegistersServices()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";

        // Act
        _ = builder.AddIbmMQ(name);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();
        var registrations = options?.Value?.Registrations;

        _ = await Assert.That(registrations).IsNotNull().And.Contains(r => r.Name == name);
    }

    [Test]
    public async Task AddIbmMQ_WithOptions_ConfiguresOptions()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";
        const int timeout = 200;

        // Act
        _ = builder.AddIbmMQ(name, options => options.Timeout = timeout);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var optionsSnapshot = serviceProvider.GetService<IOptionsSnapshot<IbmMQOptions>>();
        var options = optionsSnapshot?.Get(name);

        using (Assert.Multiple())
        {
            _ = await Assert.That(options).IsNotNull();
            _ = await Assert.That(options!.Timeout).IsEqualTo(timeout);
        }
    }

    [Test]
    public async Task AddIbmMQ_WithCustomTags_IncludesDefaultAndCustomTags()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";
        var customTags = new[] { "custom1", "custom2" };

        // Act
        _ = builder.AddIbmMQ(name, tags: customTags);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();
        var registration = options?.Value?.Registrations.FirstOrDefault(r => r.Name == name);

        using (Assert.Multiple())
        {
            _ = await Assert.That(registration).IsNotNull();
            _ = await Assert
                .That(registration!.Tags)
                .IsNotNull()
                .And.Contains("ibmmq")
                .And.Contains("messaging")
                .And.Contains("custom1")
                .And.Contains("custom2");
        }
    }

    [Test]
    public async Task AddIbmMQ_CalledMultipleTimesWithDifferentNames_RegistersMultipleChecks()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name1 = "Test1";
        const string name2 = "Test2";

        // Act
        _ = builder.AddIbmMQ(name1).AddIbmMQ(name2);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();
        var registrations = options?.Value?.Registrations;

        _ = await Assert
            .That(registrations)
            .IsNotNull()
            .And.Contains(r => r.Name == name1)
            .And.Contains(r => r.Name == name2);
    }
}
