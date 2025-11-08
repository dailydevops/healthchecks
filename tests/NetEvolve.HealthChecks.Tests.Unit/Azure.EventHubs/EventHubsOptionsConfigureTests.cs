namespace NetEvolve.HealthChecks.Tests.Unit.Azure.EventHubs;

using System;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.EventHubs;

[TestGroup($"{nameof(Azure)}.{nameof(EventHubs)}")]
public sealed class EventHubsOptionsConfigureTests
{
    [Test]
    public void Configure_WhenArgumentNameNull_ThrowsArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        var options = new EventHubsOptions();

        // Act
        void Act() => configure.Configure(null, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void Configure_WhenArgumentNameEmpty_ThrowsArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        var options = new EventHubsOptions();

        // Act
        void Act() => configure.Configure(string.Empty, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void Configure_WhenArgumentNameWhitespace_ThrowsArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        var options = new EventHubsOptions();

        // Act
        void Act() => configure.Configure(" ", options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public async Task Configure_WhenArgumentNameValid_BindsFromConfiguration()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["HealthChecks:AzureEventHubs:Test:Mode"] = "ConnectionString",
            ["HealthChecks:AzureEventHubs:Test:ConnectionString"] = "TestConnectionString",
            ["HealthChecks:AzureEventHubs:Test:EventHubName"] = "TestEventHub",
            ["HealthChecks:AzureEventHubs:Test:Timeout"] = "200",
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        var options = new EventHubsOptions();

        // Act
        configure.Configure("Test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.Mode).IsEqualTo(ClientCreationMode.ConnectionString);
            _ = await Assert.That(options.ConnectionString).IsEqualTo("TestConnectionString");
            _ = await Assert.That(options.EventHubName).IsEqualTo("TestEventHub");
            _ = await Assert.That(options.Timeout).IsEqualTo(200);
        }
    }

    [Test]
    public async Task Validate_WhenArgumentNameNull_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = "Test",
            EventHubName = "Test",
        };

        // Act
        var result = configure.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenArgumentNameEmpty_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = "Test",
            EventHubName = "Test",
        };

        // Act
        var result = configure.Validate(string.Empty, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenArgumentNameWhitespace_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = "Test",
            EventHubName = "Test",
        };

        // Act
        var result = configure.Validate(" ", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenArgumentOptionsNull_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        const string name = "Test";
        var options = default(EventHubsOptions);

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The option cannot be null.");
        }
    }

    [Test]
    public async Task Validate_WhenArgumentTimeoutLessThanInfinite_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        const string name = "Test";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = "Test",
            EventHubName = "Test",
            Timeout = -2,
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo(
                    "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout."
                );
        }
    }

    [Test]
    public async Task Validate_WhenArgumentTimeoutValid_ReturnsSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        const string name = "Test";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = "Test",
            EventHubName = "Test",
            Timeout = 100,
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Validate_WhenArgumentTimeoutInfinite_ReturnsSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        const string name = "Test";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = "Test",
            EventHubName = "Test",
            Timeout = Timeout.Infinite,
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Validate_WhenModeNull_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        const string name = "Test";
        var options = new EventHubsOptions { Mode = null, EventHubName = "Test" };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The client creation mode cannot be null.");
        }
    }

    [Test]
    public async Task Validate_WhenModeDefaultAzureCredentialsAndFullyQualifiedNamespaceNull_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        const string name = "Test";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedNamespace = null,
            EventHubName = "Test",
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo(
                    "The fully qualified namespace cannot be null or whitespace when using DefaultAzureCredentials."
                );
        }
    }

    [Test]
    public async Task Validate_WhenModeDefaultAzureCredentialsAndFullyQualifiedNamespaceEmpty_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        const string name = "Test";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedNamespace = string.Empty,
            EventHubName = "Test",
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo(
                    "The fully qualified namespace cannot be null or whitespace when using DefaultAzureCredentials."
                );
        }
    }

    [Test]
    public async Task Validate_WhenModeDefaultAzureCredentialsAndFullyQualifiedNamespaceWhitespace_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        const string name = "Test";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedNamespace = " ",
            EventHubName = "Test",
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo(
                    "The fully qualified namespace cannot be null or whitespace when using DefaultAzureCredentials."
                );
        }
    }

    [Test]
    public async Task Validate_WhenModeDefaultAzureCredentialsAndFullyQualifiedNamespaceValid_ReturnsSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        const string name = "Test";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.DefaultAzureCredentials,
            FullyQualifiedNamespace = "test.servicebus.windows.net",
            EventHubName = "Test",
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Validate_WhenModeConnectionStringAndConnectionStringNull_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        const string name = "Test";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = null,
            EventHubName = "Test",
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The connection string cannot be null or whitespace when using ConnectionString.");
        }
    }

    [Test]
    public async Task Validate_WhenModeConnectionStringAndConnectionStringEmpty_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        const string name = "Test";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = string.Empty,
            EventHubName = "Test",
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The connection string cannot be null or whitespace when using ConnectionString.");
        }
    }

    [Test]
    public async Task Validate_WhenModeConnectionStringAndConnectionStringWhitespace_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        const string name = "Test";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = " ",
            EventHubName = "Test",
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The connection string cannot be null or whitespace when using ConnectionString.");
        }
    }

    [Test]
    public async Task Validate_WhenModeConnectionStringAndConnectionStringValid_ReturnsSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["HealthChecks:AzureEventHubs:Test:Mode"] = "ConnectionString",
                    ["HealthChecks:AzureEventHubs:Test:ConnectionString"] = "Test",
                    ["HealthChecks:AzureEventHubs:Test:EventHubName"] = "Test",
                }
            )
            .Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        var options = new EventHubsOptions();

        configure.Configure("Test", options);

        // Act
        var result = configure.Validate("Test", options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Validate_WhenEventHubNameNull_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        const string name = "Test";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = "Test",
            EventHubName = null,
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The event hub name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenEventHubNameEmpty_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        const string name = "Test";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = "Test",
            EventHubName = string.Empty,
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The event hub name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenEventHubNameWhitespace_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        const string name = "Test";
        var options = new EventHubsOptions
        {
            Mode = ClientCreationMode.ConnectionString,
            ConnectionString = "Test",
            EventHubName = " ",
        };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The event hub name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenModeServiceProviderAndEventHubNameValid_ReturnsSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        const string name = "Test";
        var options = new EventHubsOptions { Mode = ClientCreationMode.ServiceProvider, EventHubName = "Test" };

        // Act
        var result = configure.Validate(name, options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public void Configure_WithDefaultName_BindsFromConfiguration()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new EventHubsOptionsConfigure(configuration);
        var options = new EventHubsOptions();

        // Act
        void Act() => configure.Configure(options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }
}
