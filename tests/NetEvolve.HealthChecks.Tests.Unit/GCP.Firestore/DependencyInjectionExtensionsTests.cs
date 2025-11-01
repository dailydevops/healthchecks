namespace NetEvolve.HealthChecks.Tests.Unit.GCP.Firestore;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.GCP.Firestore;

[TestGroup($"GCP.{nameof(Firestore)}")]
public sealed class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddFirestore_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        IHealthChecksBuilder builder = null!;
        const string name = "Test";

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(nameof(builder), () => builder.AddFirestore(name));
    }

    [Test]
    public void AddFirestore_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        const string name = null!;

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(nameof(name), () => builder.AddFirestore(name));
    }

    [Test]
    public void AddFirestore_WhenArgumentNameEmpty_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        const string name = "";

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(nameof(name), () => builder.AddFirestore(name));
    }

    [Test]
    public void AddFirestore_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        const string name = "Test";

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>("tags", () => builder.AddFirestore(name, options: null, tags: null!));
    }

    [Test]
    public async Task AddFirestore_WhenParameterCorrect_RegistrationAdded()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection([]).Build());
        var builder = services.AddHealthChecks();
        const string name = "Test";

        // Act
        _ = builder.AddFirestore(name);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options =
            serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<HealthCheckServiceOptions>>();
        var registration = options.Value.Registrations.FirstOrDefault(r => r.Name == name);

        _ = await Assert.That(registration).IsNotNull();
    }

    [Test]
    public async Task AddFirestore_WhenParameterCorrectWithOptions_RegistrationAdded()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection([]).Build());
        var builder = services.AddHealthChecks();
        const string name = "Test";

        // Act
        _ = builder.AddFirestore(
            name,
            options =>
            {
                options.Timeout = 10000;
                options.KeyedService = "firestore";
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
    public void AddFirestore_WhenDuplicateName_ThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection([]).Build());
        var builder = services.AddHealthChecks();
        const string name = "Test";

        _ = builder.AddFirestore(name);

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(nameof(name), () => builder.AddFirestore(name));
    }

    [Test]
    public async Task AddFirestore_WhenMultipleChecksWithDifferentNames_AllRegistrationsAdded()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection([]).Build());
        var builder = services.AddHealthChecks();

        // Act
        _ = builder.AddFirestore("Test1");
        _ = builder.AddFirestore("Test2");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options =
            serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<HealthCheckServiceOptions>>();

        _ = await Assert.That(options.Value.Registrations.Any(r => r.Name == "Test1")).IsTrue();
        _ = await Assert.That(options.Value.Registrations.Any(r => r.Name == "Test2")).IsTrue();
    }

    [Test]
    public async Task AddFirestore_WhenCustomTags_TagsApplied()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection([]).Build());
        var builder = services.AddHealthChecks();
        const string name = "Test";
        const string customTag = "custom-tag";

        // Act
        _ = builder.AddFirestore(name, tags: customTag);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options =
            serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<HealthCheckServiceOptions>>();
        var registration = options.Value.Registrations.First(r => r.Name == name);

        _ = await Assert.That(registration.Tags.Contains(customTag)).IsTrue();
        _ = await Assert.That(registration.Tags.Contains("firestore")).IsTrue();
        _ = await Assert.That(registration.Tags.Contains("gcp")).IsTrue();
    }
}
