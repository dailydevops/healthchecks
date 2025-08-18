namespace NetEvolve.HealthChecks.Tests.Unit.Http;

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Http;

[TestGroup(nameof(Http))]
public sealed class HttpConfigureTests
{
    [Test]
    public async Task Configure_WhenArgumentNameNull_ConfigurationNotCalled()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection([]).Build());

        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var configure = new HttpConfigure(configuration);
        var options = new HttpOptions();

        // Act
        configure.Configure(name: null, options);

        // Assert
        _ = await Assert.That(options.Uri).IsEqualTo(default(string));
    }

    [Test]
    public async Task Configure_WhenArgumentNameEmpty_ConfigurationNotCalled()
    {
        // Arrange
        var services = new ServiceCollection();
        _ = services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection([]).Build());

        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var configure = new HttpConfigure(configuration);
        var options = new HttpOptions();

        // Act
        configure.Configure(name: string.Empty, options);

        // Assert
        _ = await Assert.That(options.Uri).IsEqualTo(default(string));
    }

    [Test]
    public async Task Configure_WhenConfigurationExists_ConfigurationCalled()
    {
        // Arrange
        const string key = "Test";
        const string uri = "https://example.com";

        var services = new ServiceCollection();
        _ = services.AddSingleton<IConfiguration>(
            new ConfigurationBuilder().AddInMemoryCollection([$"HealthChecks:Http:{key}:Uri"] = [uri]).Build()
        );

        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var configure = new HttpConfigure(configuration);
        var options = new HttpOptions();

        // Act
        configure.Configure(key, options);

        // Assert
        _ = await Assert.That(options.Uri).IsEqualTo(uri);
    }
}
