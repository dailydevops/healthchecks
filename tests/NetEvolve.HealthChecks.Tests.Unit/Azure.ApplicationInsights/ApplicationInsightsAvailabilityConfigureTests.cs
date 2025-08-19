namespace NetEvolve.HealthChecks.Tests.Unit.Azure.ApplicationInsights;

using System;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.ApplicationInsights;

[TestGroup($"{nameof(Azure)}.{nameof(ApplicationInsights)}")]
public sealed class ApplicationInsightsAvailabilityConfigureTests
{
    [Test]
    public void Configure_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new ApplicationInsightsAvailabilityOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    public async Task Validate_WhenNameNull_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );

        // Act
        var result = configure.Validate(null, new ApplicationInsightsAvailabilityOptions());

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenNameWhitespace_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );

        // Act
        var result = configure.Validate("\t", new ApplicationInsightsAvailabilityOptions());

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenOptionsNull_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );

        // Act
        var result = configure.Validate("name", null!);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The option cannot be null.");
        }
    }

    [Test]
    public async Task Validate_WhenInvalidTimeout_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new ApplicationInsightsAvailabilityOptions { Timeout = -2 };

        // Act
        var result = configure.Validate("name", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo(
                    "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout."
                );
        }
    }

    [Test]
    public async Task Validate_WhenConnectionStringModeWithNullConnectionString_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new ApplicationInsightsAvailabilityOptions
        {
            Mode = ApplicationInsightsClientCreationMode.ConnectionString,
            ConnectionString = null,
        };

        // Act
        var result = configure.Validate("name", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The connection string cannot be null or whitespace when using `ConnectionString` mode.");
        }
    }

    [Test]
    public async Task Validate_WhenConnectionStringModeWithWhitespaceConnectionString_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new ApplicationInsightsAvailabilityOptions
        {
            Mode = ApplicationInsightsClientCreationMode.ConnectionString,
            ConnectionString = "\t",
        };

        // Act
        var result = configure.Validate("name", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The connection string cannot be null or whitespace when using `ConnectionString` mode.");
        }
    }

    [Test]
    public async Task Validate_WhenConnectionStringModeWithValidConnectionString_ReturnsSuccess()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new ApplicationInsightsAvailabilityOptions
        {
            Mode = ApplicationInsightsClientCreationMode.ConnectionString,
            ConnectionString =
                "InstrumentationKey=12345678-1234-1234-1234-123456789abc;IngestionEndpoint=https://westus-0.in.applicationinsights.azure.com/",
        };

        // Act
        var result = configure.Validate("name", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsNull();
        }
    }

    [Test]
    public async Task Validate_WhenInstrumentationKeyModeWithNullInstrumentationKey_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new ApplicationInsightsAvailabilityOptions
        {
            Mode = ApplicationInsightsClientCreationMode.InstrumentationKey,
            InstrumentationKey = null,
        };

        // Act
        var result = configure.Validate("name", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo(
                    "The instrumentation key cannot be null or whitespace when using `InstrumentationKey` mode."
                );
        }
    }

    [Test]
    public async Task Validate_WhenInstrumentationKeyModeWithWhitespaceInstrumentationKey_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new ApplicationInsightsAvailabilityOptions
        {
            Mode = ApplicationInsightsClientCreationMode.InstrumentationKey,
            InstrumentationKey = "\t",
        };

        // Act
        var result = configure.Validate("name", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo(
                    "The instrumentation key cannot be null or whitespace when using `InstrumentationKey` mode."
                );
        }
    }

    [Test]
    public async Task Validate_WhenInstrumentationKeyModeWithValidInstrumentationKey_ReturnsSuccess()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new ApplicationInsightsAvailabilityOptions
        {
            Mode = ApplicationInsightsClientCreationMode.InstrumentationKey,
            InstrumentationKey = "12345678-1234-1234-1234-123456789abc",
        };

        // Act
        var result = configure.Validate("name", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsNull();
        }
    }

    [Test]
    public async Task Validate_WhenServiceProviderModeWithoutTelemetryClient_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new ApplicationInsightsAvailabilityOptions
        {
            Mode = ApplicationInsightsClientCreationMode.ServiceProvider,
        };

        // Act
        var result = configure.Validate("name", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo(
                    "No service of type `TelemetryClient` registered. Please register Application Insights using AddApplicationInsightsTelemetry()."
                );
        }
    }

    [Test]
    public async Task Validate_WhenServiceProviderModeWithTelemetryClient_ReturnsSuccess()
    {
        // Arrange
        var services = new ServiceCollection().AddSingleton<TelemetryClient>();
        var configure = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new ApplicationInsightsAvailabilityOptions
        {
            Mode = ApplicationInsightsClientCreationMode.ServiceProvider,
        };

        // Act
        var result = configure.Validate("name", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsNull();
        }
    }

    [Test]
    public async Task Validate_WhenUnsupportedMode_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new ApplicationInsightsAvailabilityConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new ApplicationInsightsAvailabilityOptions { Mode = null };

        // Act
        var result = configure.Validate("name", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The mode `` is not supported.");
        }
    }
}
