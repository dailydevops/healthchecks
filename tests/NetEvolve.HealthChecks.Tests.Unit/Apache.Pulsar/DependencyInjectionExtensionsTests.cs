namespace NetEvolve.HealthChecks.Tests.Unit.Pulsar;

using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Apache.Pulsar;

[TestGroup($"{nameof(Apache)}.{nameof(Pulsar)}")]
public class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddPulsar_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        var builder = default(IHealthChecksBuilder);

        void Act() => builder.AddPulsar("Test");

        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Test]
    public void AddPulsar_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string? name = default;

        void Act() => builder.AddPulsar(name!);

        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
    public void AddPulsar_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var name = string.Empty;

        void Act() => builder.AddPulsar(name);

        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void AddPulsar_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        var tags = default(string[]);

        void Act() => builder.AddPulsar("Test", tags: tags);

        _ = Assert.Throws<ArgumentNullException>("tags", Act);
    }

    [Test]
    public void AddPulsar_WhenArgumentNameIsAlreadyUsed_ThrowArgumentException()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";

        void Act() => builder.AddPulsar(name).AddPulsar(name);

        _ = Assert.Throws<ArgumentException>(nameof(name), Act);
    }

    [Test]
    public async Task AddPulsar_WithValidArguments_RegistersServices()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";

        _ = builder.AddPulsar(name);

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();
        var registrations = options?.Value?.Registrations;

        _ = await Assert.That(registrations).IsNotNull().And.Contains(r => r.Name == name);
    }

    [Test]
    public async Task AddPulsar_WithOptions_ConfiguresOptions()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";
        const int timeout = 200;

        _ = builder.AddPulsar(name, options => options.Timeout = timeout);

        var serviceProvider = services.BuildServiceProvider();
        var optionsSnapshot = serviceProvider.GetService<IOptionsSnapshot<PulsarOptions>>();
        var options = optionsSnapshot?.Get(name);

        using (Assert.Multiple())
        {
            _ = await Assert.That(options).IsNotNull();
            _ = await Assert.That(options!.Timeout).IsEqualTo(timeout);
        }
    }

    [Test]
    public async Task AddPulsar_WithCustomTags_IncludesDefaultAndCustomTags()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name = "Test";
        var customTags = new[] { "custom1", "custom2" };

        _ = builder.AddPulsar(name, tags: customTags);

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();
        var registration = options?.Value?.Registrations.FirstOrDefault(r => r.Name == name);

        using (Assert.Multiple())
        {
            _ = await Assert.That(registration).IsNotNull();
            _ = await Assert
                .That(registration!.Tags)
                .IsNotNull()
                .And.Contains("pulsar")
                .And.Contains("messaging")
                .And.Contains("custom1")
                .And.Contains("custom2");
        }
    }

    [Test]
    public async Task AddPulsar_CalledMultipleTimesWithDifferentNames_RegistersMultipleChecks()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        var builder = services.AddSingleton<IConfiguration>(configuration).AddHealthChecks();
        const string name1 = "Test1";
        const string name2 = "Test2";

        _ = builder.AddPulsar(name1).AddPulsar(name2);

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
