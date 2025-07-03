namespace NetEvolve.HealthChecks.Tests.Unit.LiteDB;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.LiteDB;

[TestGroup(nameof(LiteDB))]
public sealed class LiteDBConfigureTests
{
    [Test]
    public void Configure_WhenNameIsNull_ThrowsArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new LiteDBConfigure(configuration);
        var options = new LiteDBOptions();

        // Act
        void Act() => configure.Configure(null, options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public void Configure_WhenNameIsWhiteSpace_ThrowsArgumentException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new LiteDBConfigure(configuration);
        var options = new LiteDBOptions();

        // Act
        void Act() => configure.Configure(" ", options);

        // Assert
        _ = Assert.Throws<ArgumentException>("name", Act);
    }

    [Test]
    public async Task Configure_WithName_BindsFromConfiguration()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["HealthChecks:LiteDB:test:ConnectionString"] = "filename=test.db",
            ["HealthChecks:LiteDB:test:CollectionName"] = "TestCollection",
            ["HealthChecks:LiteDB:test:Timeout"] = "1000",
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();

        var configure = new LiteDBConfigure(configuration);
        var options = new LiteDBOptions();

        // Act
        configure.Configure("test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.ConnectionString).IsEqualTo("filename=test.db");
            _ = await Assert.That(options.CollectionName).IsEqualTo("TestCollection");
            _ = await Assert.That(options.Timeout).IsEqualTo(1000);
        }
    }

    [Test]
    public async Task Validate_WhenNameIsNull_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new LiteDBConfigure(configuration);
        var options = new LiteDBOptions
        {
            ConnectionString = "filename=test.db",
            CollectionName = "TestCollection",
            Timeout = 1000,
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
    public async Task Validate_WhenNameIsWhitespace_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new LiteDBConfigure(configuration);
        var options = new LiteDBOptions
        {
            ConnectionString = "filename=test.db",
            CollectionName = "TestCollection",
            Timeout = 1000,
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
    public async Task Validate_WhenOptionsIsNull_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new LiteDBConfigure(configuration);

        // Act
        var result = configure.Validate("test", null!);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The options cannot be null.");
        }
    }

    [Test]
    public async Task Validate_WhenTimeoutIsNegative_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new LiteDBConfigure(configuration);
        var options = new LiteDBOptions
        {
            ConnectionString = "filename=test.db",
            CollectionName = "TestCollection",
            Timeout = -2, // -1 is allowed for infinite timeout
        };

        // Act
        var result = configure.Validate("test", options);

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
    public async Task Validate_WhenConnectionStringIsNull_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new LiteDBConfigure(configuration);
        var options = new LiteDBOptions
        {
            ConnectionString = null!,
            CollectionName = "TestCollection",
            Timeout = 1000,
        };

        // Act
        var result = configure.Validate("test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The connection string cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenConnectionStringIsWhitespace_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new LiteDBConfigure(configuration);
        var options = new LiteDBOptions
        {
            ConnectionString = " ",
            CollectionName = "TestCollection",
            Timeout = 1000,
        };

        // Act
        var result = configure.Validate("test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The connection string cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenCollectionNameIsNull_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new LiteDBConfigure(configuration);
        var options = new LiteDBOptions
        {
            ConnectionString = "filename=test.db",
            CollectionName = null!,
            Timeout = 1000,
        };

        // Act
        var result = configure.Validate("test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The collection name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenCollectionNameIsWhitespace_ReturnsFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new LiteDBConfigure(configuration);
        var options = new LiteDBOptions
        {
            ConnectionString = "filename=test.db",
            CollectionName = " ",
            Timeout = 1000,
        };

        // Act
        var result = configure.Validate("test", options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).IsEqualTo("The collection name cannot be null or whitespace.");
        }
    }

    [Test]
    public async Task Validate_WhenAllOptionsValid_ReturnsSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new LiteDBConfigure(configuration);
        var options = new LiteDBOptions
        {
            ConnectionString = "filename=test.db",
            CollectionName = "TestCollection",
            Timeout = 1000,
        };

        // Act
        var result = configure.Validate("test", options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Validate_WhenInfiniteTimeout_ReturnsSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new LiteDBConfigure(configuration);
        var options = new LiteDBOptions
        {
            ConnectionString = "filename=test.db",
            CollectionName = "TestCollection",
            Timeout = -1, // Infinite timeout
        };

        // Act
        var result = configure.Validate("test", options);

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }
}
