namespace NetEvolve.HealthChecks.Tests.Unit.GCP.PubSub;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.GCP.PubSub;

[TestGroup($"GCP.{nameof(PubSub)}")]
public sealed class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddPubSub_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        IHealthChecksBuilder builder = null!;
        const string name = "Test";

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(nameof(builder), () => builder.AddPubSub(name));
    }

    [Test]
    public void AddPubSub_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        const string name = null!;

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(nameof(name), () => builder.AddPubSub(name));
    }

    [Test]
    public void AddPubSub_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        const string name = "";

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(nameof(name), () => builder.AddPubSub(name));
    }

    [Test]
    public void AddPubSub_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        const string name = "Test";

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>("tags", () => builder.AddPubSub(name, options: null, tags: null!));
    }

    [Test]
    public async Task AddPubSub_WhenParameterCorrect_RegistrationAdded()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection([]).Build());
        var builder = services.AddHealthChecks();
        const string name = "Test";

        // Act
        _ = builder.AddPubSub(name);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options =
            serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<HealthCheckServiceOptions>>();
        var registration = options.Value.Registrations.FirstOrDefault(r => r.Name == name);

        _ = await Assert.That(registration).IsNotNull();
    }

    [Test]
    public async Task AddPubSub_WhenParameterCorrectWithOptions_RegistrationAdded()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection([]).Build());
        var builder = services.AddHealthChecks();
        const string name = "Test";

        // Act
        _ = builder.AddPubSub(
            name,
            options =>
            {
                options.Timeout = 10000;
                options.KeyedService = "pubsub";
            }
        );

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options =
            serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<HealthCheckServiceOptions>>();
        var registration = options.Value.Registrations.FirstOrDefault(r => r.Name == name);

        _ = await Assert.That(registration).IsNotNull();
    }

    [Test]
    public void AddPubSub_WhenDuplicateName_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection([]).Build());
        var builder = services.AddHealthChecks();
        const string name = "Test";

        _ = builder.AddPubSub(name);

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(nameof(name), () => builder.AddPubSub(name));
    }

    [Test]
    public async Task AddPubSub_WhenMultipleChecksWithDifferentNames_AllRegistrationsAdded()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection([]).Build());
        var builder = services.AddHealthChecks();

        // Act
        _ = builder.AddPubSub("Test1");
        _ = builder.AddPubSub("Test2");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options =
            serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<HealthCheckServiceOptions>>();

        _ = await Assert.That(options.Value.Registrations.Any(r => r.Name == "Test1")).IsTrue();
        _ = await Assert.That(options.Value.Registrations.Any(r => r.Name == "Test2")).IsTrue();
    }

    [Test]
    public async Task AddPubSub_WhenCustomTags_TagsApplied()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection([]).Build());
        var builder = services.AddHealthChecks();
        const string name = "Test";
        const string customTag = "custom-tag";

        // Act
        _ = builder.AddPubSub(name, tags: customTag);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options =
            serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<HealthCheckServiceOptions>>();
        var registration = options.Value.Registrations.First(r => r.Name == name);

        _ = await Assert.That(registration.Tags.Contains(customTag)).IsTrue();
        _ = await Assert.That(registration.Tags.Contains("pubsub")).IsTrue();
        _ = await Assert.That(registration.Tags.Contains("gcp")).IsTrue();
    }
}
