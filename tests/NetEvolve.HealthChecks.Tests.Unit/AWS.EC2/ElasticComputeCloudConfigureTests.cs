namespace NetEvolve.HealthChecks.Tests.Unit.AWS.EC2;

using System;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.AWS.EC2;
using NetEvolve.HealthChecks.Tests.Unit.Internals;

[TestGroup($"{nameof(AWS)}.{nameof(EC2)}")]
public sealed class ElasticComputeCloudConfigureTests
{
    private static ElasticComputeCloudConfigure CreateConfigure(IConfiguration? configuration = null) =>
        new(configuration ?? new ConfigurationBuilder().Build());

    [Test]
    [MethodDataSource(typeof(DataSource), nameof(DataSource.NullOrWhitespaces))]
    public async Task Validate_WhenNameIsNullOrWhiteSpace_ReturnsFail(string? name)
    {
        var configure = CreateConfigure();
        var options = new ElasticComputeCloudOptions { KeyName = "key" };
        var result = configure.Validate(name, options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The name cannot be null or whitespace.", StringComparison.Ordinal);
        }
    }

    [Test]
    public async Task Validate_WhenOptionsIsNull_ReturnsFail()
    {
        var configure = CreateConfigure();
        var result = configure.Validate("Test", null!);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The option cannot be null.", StringComparison.Ordinal);
        }
    }

    [Test]
    [MethodDataSource(typeof(DataSource), nameof(DataSource.NullOrWhitespaces))]
    public async Task Validate_WhenKeyNameIsNullOrWhiteSpace_ReturnsFail(string? keyName)
    {
        var configure = CreateConfigure();
        var options = new ElasticComputeCloudOptions { KeyName = keyName };
        var result = configure.Validate("Test", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The key name cannot be null or whitespace.", StringComparison.Ordinal);
        }
    }

    [Test]
    public async Task Validate_WhenModeIsBasicAuthentication_AndAccessKeyAndSecretKeyAreSet_ReturnsSuccess()
    {
        var configure = CreateConfigure();
        var options = new ElasticComputeCloudOptions
        {
            KeyName = "key",
            Mode = CreationMode.BasicAuthentication,
            AccessKey = "access",
            SecretKey = "secret",
        };
        var result = configure.Validate("Test", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsFalse();
            _ = await Assert.That(result.FailureMessage).IsNull();
        }
    }

    [Test]
    public async Task Validate_WhenModeIsNull_DoesNotRequireAccessKeyOrSecretKey()
    {
        var configure = CreateConfigure();
        var options = new ElasticComputeCloudOptions
        {
            KeyName = "key",
            Mode = null,
            AccessKey = null,
            SecretKey = null,
        };
        var result = configure.Validate("Test", options);

        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Configure_WithValidName_BindsConfiguration()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                [
                    new KeyValuePair<string, string?>("HealthChecks:AWSEC2:Test:KeyName", "key"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSEC2:Test:ServiceUrl", "url"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSEC2:Test:AccessKey", "access"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSEC2:Test:SecretKey", "secret"),
                ]
            )
            .Build();
        var configure = CreateConfigure(config);
        var options = new ElasticComputeCloudOptions();
        configure.Configure("Test", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(options).IsNotNull();
            _ = await Assert.That(options.KeyName).IsEqualTo("key");
            _ = await Assert.That(options.ServiceUrl).IsEqualTo("url");
            _ = await Assert.That(options.AccessKey).IsEqualTo("access");
            _ = await Assert.That(options.SecretKey).IsEqualTo("secret");
        }
    }

    [Test]
    [MethodDataSource(typeof(DataSource), nameof(DataSource.NullOrWhitespaces))]
    public void Configure_WithNullOrWhiteSpaceName_ThrowsArgumentException(string? name)
    {
        var configure = CreateConfigure();
        var options = new ElasticComputeCloudOptions();
        _ = Assert.Throws<ArgumentException>(() => configure.Configure(name, options));
    }

    [Test]
    public void Configure_WithNullOrWhiteSpaceName_ThrowsArgumentNullException()
    {
        var configure = CreateConfigure();
        var options = new ElasticComputeCloudOptions();
        _ = Assert.Throws<ArgumentNullException>(() => configure.Configure(null, options));
    }

    [Test]
    public async Task Configure_Parameterless_UsesDefaultName()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                [
                    new KeyValuePair<string, string?>("HealthChecks:AWSEC2:Default:KeyName", "key-default"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSEC2:Default:ServiceUrl", "url-default"),
                ]
            )
            .Build();
        var configure = CreateConfigure(config);
        var options = new ElasticComputeCloudOptions();
        configure.Configure("Default", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(options).IsNotNull();
            _ = await Assert.That(options.KeyName).IsEqualTo("key-default");
            _ = await Assert.That(options.ServiceUrl).IsEqualTo("url-default");
        }
    }

    [Test]
    [MethodDataSource(typeof(DataSource), nameof(DataSource.TimeoutsInvalid))]
    public async Task Validate_WhenTimeoutLessThanInfinite_ReturnsFail(int timeout)
    {
        var configure = CreateConfigure();
        var options = new ElasticComputeCloudOptions { KeyName = "key", Timeout = timeout };
        var result = configure.Validate("Test", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo(
                    "The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.",
                    StringComparison.OrdinalIgnoreCase
                );
        }
    }

    [Test]
    [MethodDataSource(typeof(DataSource), nameof(DataSource.TimeoutsValid))]
    public async Task Validate_WhenTimeoutIsValidValue_ReturnsSuccess(int timeout)
    {
        var configure = CreateConfigure();
        var options = new ElasticComputeCloudOptions { KeyName = "key", Timeout = timeout };
        var result = configure.Validate("Test", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsFalse();
            _ = await Assert.That(result.FailureMessage).IsNull();
        }
    }

    [Test]
    [MethodDataSource(typeof(DataSource), nameof(DataSource.NullOrWhitespaces))]
    public async Task Validate_WhenModeIsBasicAuthentication_AndAccessKeyIsNullOrWhiteSpace_ReturnsFail(
        string? accessKey
    )
    {
        var configure = CreateConfigure();
        var options = new ElasticComputeCloudOptions
        {
            KeyName = "key",
            Mode = CreationMode.BasicAuthentication,
            AccessKey = accessKey,
            SecretKey = "secret",
        };

        var result = configure.Validate("Test", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The access key cannot be null or whitespace.", StringComparison.Ordinal);
        }
    }

    [Test]
    [MethodDataSource(typeof(DataSource), nameof(DataSource.NullOrWhitespaces))]
    public async Task Validate_WhenModeIsBasicAuthentication_AndSecretKeyIsNullOrWhiteSpace_ReturnsFail(
        string? secretKey
    )
    {
        var configure = CreateConfigure();
        var options = new ElasticComputeCloudOptions
        {
            KeyName = "key",
            Mode = CreationMode.BasicAuthentication,
            AccessKey = "access",
            SecretKey = secretKey,
        };

        var result = configure.Validate("Test", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .IsEqualTo("The secret key cannot be null or whitespace.", StringComparison.Ordinal);
        }
    }

    [Test]
    public async Task Configure_BindsAllConfigurationProperties()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                [
                    new KeyValuePair<string, string?>("HealthChecks:AWSEC2:Test:KeyName", "key"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSEC2:Test:ServiceUrl", "url"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSEC2:Test:AccessKey", "access"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSEC2:Test:SecretKey", "secret"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSEC2:Test:Timeout", "10000"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSEC2:Test:Mode", "BasicAuthentication"),
                ]
            )
            .Build();
        var configure = CreateConfigure(config);
        var options = new ElasticComputeCloudOptions();
        configure.Configure("Test", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(options).IsNotNull();
            _ = await Assert.That(options.KeyName).IsEqualTo("key");
            _ = await Assert.That(options.ServiceUrl).IsEqualTo("url");
            _ = await Assert.That(options.AccessKey).IsEqualTo("access");
            _ = await Assert.That(options.SecretKey).IsEqualTo("secret");
            _ = await Assert.That(options.Timeout).IsEqualTo(10000);
            _ = await Assert.That(options.Mode).IsEqualTo(CreationMode.BasicAuthentication);
        }
    }
}
