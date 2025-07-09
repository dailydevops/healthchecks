namespace NetEvolve.HealthChecks.Tests.Unit.LiteDB;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.LiteDB;

[TestGroup(nameof(LiteDB))]
public sealed class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddLiteDB_WhenBuilderIsNull_ThrowsArgumentNullException()
    {
        // Act
        void Act() => DependencyInjectionExtensions.AddLiteDB(null!, "test");

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Test]
    public void AddLiteDB_WhenNameIsNull_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        void Act() => builder.AddLiteDB(null!);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void AddLiteDB_WhenNameIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        void Act() => builder.AddLiteDB(string.Empty);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void AddLiteDB_WhenTagsIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        void Act() => builder.AddLiteDB("test", tags: null!);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("tags", Act);
    }

    [Test]
    public async Task AddLiteDB_WhenValidParameters_RegistersHealthCheck()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();

        // Act
        var result = builder.AddLiteDB(
            "litedb-test",
            options =>
            {
                options.ConnectionString = "filename=:memory:";
                options.CollectionName = "test";
                options.Timeout = 1000;
            },
            "tag1",
            "tag2"
        );

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();
        var registration = options?.Value.Registrations.FirstOrDefault();

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsNotNull();
            _ = await Assert.That(registration).IsNotNull();
            _ = await Assert.That(registration!.Name).IsEqualTo("litedb-test");
            _ = await Assert.That(registration!.FailureStatus).IsEqualTo(HealthStatus.Unhealthy);

            var tags = registration!.Tags.ToArray();
            _ = await Assert.That(tags.Length).IsEqualTo(4);
            _ = await Assert.That(tags).Contains("litedb");
            _ = await Assert.That(tags).Contains("database");
            _ = await Assert.That(tags).Contains("tag1");
            _ = await Assert.That(tags).Contains("tag2");
        }
    }

    [Test]
    public async Task AddLiteDB_WhenAddedMultipleTimes_RegistersMultipleHealthChecks()
    {
        // Arrange
        var configration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection().AddSingleton(configration);
        var builder = services.AddHealthChecks();

        // Act
        _ = builder.AddLiteDB(
            "litedb-test1",
            options =>
            {
                options.ConnectionString = "filename=:memory:";
                options.CollectionName = "test1";
                options.Timeout = 1000;
            }
        );

        _ = builder.AddLiteDB(
            "litedb-test2",
            options =>
            {
                options.ConnectionString = "filename=:memory:";
                options.CollectionName = "test2";
                options.Timeout = 1000;
            }
        );

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();
        var registrations = options
            ?.Value.Registrations.Where(r => r.Name.StartsWith("litedb-test", StringComparison.Ordinal))
            .ToArray();

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(registrations).IsNotNull();
            _ = await Assert.That(registrations!.Length).IsEqualTo(2);
            _ = await Assert.That(registrations!.Any(r => r.Name == "litedb-test1")).IsTrue();
            _ = await Assert.That(registrations!.Any(r => r.Name == "litedb-test2")).IsTrue();
        }
    }

    [Test]
    public async Task AddLiteDB_WhenNameIsAlreadyInUse_ThrowsArgumentException()
    {
        // Arrange
        IConfiguration configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection().AddSingleton(configuration);
        var builder = services.AddHealthChecks();

        _ = builder.AddLiteDB("duplicate");

        // Act
        void Act() => builder.AddLiteDB("duplicate", options => { });

        // Assert
        var exception = Assert.Throws<ArgumentException>("name", Act);
        _ = await Assert
            .That(exception.Message)
            .StartsWith("Name `duplicate` already in use.", StringComparison.OrdinalIgnoreCase);
    }

    [Test]
    public async Task AddLiteDB_RegistersOnlyOnce_SharedServices()
    {
        // Arrange
        IConfiguration configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection().AddSingleton(configuration);
        var builder = services.AddHealthChecks();

        // Act
        _ = builder.AddLiteDB("litedb-test1");
        _ = builder.AddLiteDB("litedb-test2");

        var serviceProvider = services.BuildServiceProvider();

        // We should only have one instance of LiteDBHealthCheck registered
        var healthChecks = serviceProvider.GetServices<LiteDBHealthCheck>().ToArray();

        // Assert
        _ = await Assert.That(healthChecks.Length).IsEqualTo(1);
    }
}
