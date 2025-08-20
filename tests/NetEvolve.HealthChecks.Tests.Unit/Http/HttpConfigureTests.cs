namespace NetEvolve.HealthChecks.Tests.Unit.Http;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Http;

[TestGroup(nameof(Http))]
public sealed class HttpConfigureTests
{
    [Test]
    public void Configure_WithNullName_ThrowsArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new HttpConfigure(configuration);
        var options = new HttpOptions();
        const string? name = null;

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void Configure_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new HttpConfigure(configuration);
        var options = new HttpOptions();
        var name = string.Empty;

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void Configure_WithWhitespaceName_ThrowsArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new HttpConfigure(configuration);
        var options = new HttpOptions();
        const string name = "  ";

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public async Task Configure_WithValidName_BindsConfiguration()
    {
        // Arrange
        const string expectedUri = "https://example.com";
        const string expectedMethod = "POST";
        const int expectedTimeout = 10000;
        const string expectedContentType = "application/xml";
        const bool expectedAllowAutoRedirect = false;

        var configValues = new List<KeyValuePair<string, string?>>
        {
            new("HealthChecks:Http:TestService:Uri", expectedUri),
            new("HealthChecks:Http:TestService:HttpMethod", expectedMethod),
            new("HealthChecks:Http:TestService:Timeout", expectedTimeout.ToString(CultureInfo.InvariantCulture)),
            new("HealthChecks:Http:TestService:ContentType", expectedContentType),
            new(
                "HealthChecks:Http:TestService:AllowAutoRedirect",
                expectedAllowAutoRedirect.ToString(CultureInfo.InvariantCulture)
            ),
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();

        var configure = new HttpConfigure(configuration);
        var options = new HttpOptions();
        const string name = "TestService";

        // Act
        configure.Configure(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.Uri).IsEqualTo(expectedUri);
            _ = await Assert.That(options.HttpMethod).IsEqualTo(expectedMethod);
            _ = await Assert.That(options.Timeout).IsEqualTo(expectedTimeout);
            _ = await Assert.That(options.ContentType).IsEqualTo(expectedContentType);
            _ = await Assert.That(options.AllowAutoRedirect).IsEqualTo(expectedAllowAutoRedirect);
        }
    }

    [Test]
    public void Configure_WithDefaultName_ThrowsArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new HttpConfigure(configuration);
        var options = new HttpOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }
}
