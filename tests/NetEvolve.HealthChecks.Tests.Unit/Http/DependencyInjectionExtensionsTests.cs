namespace NetEvolve.HealthChecks.Tests.Unit.Http;

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Http;

[TestGroup(nameof(Http))]
public sealed class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddHttp_WhenArgumentBuilderNull_ThrowArgumentNullException()
    {
        // Arrange
        IHealthChecksBuilder builder = null!;
        const string name = "Test";

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(nameof(builder), () => builder.AddHttp(name));
    }

    [Test]
    public void AddHttp_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        const string name = null!;

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(nameof(name), () => builder.AddHttp(name));
    }

    [Test]
    public void AddHttp_WhenArgumentTagsNull_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        const string name = "Test";

        // Act & Assert
        Assert.Throws<ArgumentNullException>("tags", () => builder.AddHttp(name, options: null, tags: null!));
    }

    [Test]
    public async Task AddHttp_WhenParameterCorrect_ServiceAdded()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection([]).Build());
        var builder = services.AddHealthChecks();
        const string name = "Test";

        // Act
        _ = builder.AddHttp(name);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckService = serviceProvider.GetRequiredService<HealthCheckService>();

        _ = await Assert.That(healthCheckService).IsNotNull();
    }

    [Test]
    public async Task AddHttp_WhenParameterCorrectWithOptions_ServiceAdded()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection([]).Build());
        var builder = services.AddHealthChecks();
        const string name = "Test";

        // Act
        _ = builder.AddHttp(
            name,
            options =>
            {
                options.Uri = "https://example.com";
                options.Timeout = 3000;
            }
        );

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckService = serviceProvider.GetRequiredService<HealthCheckService>();

        _ = await Assert.That(healthCheckService).IsNotNull();
    }
}
