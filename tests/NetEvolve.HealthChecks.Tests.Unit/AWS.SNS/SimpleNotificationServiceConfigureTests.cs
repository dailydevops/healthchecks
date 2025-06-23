namespace NetEvolve.HealthChecks.Tests.Unit.AWS.SNS;

using System;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.AWS.SNS;
using NetEvolve.HealthChecks.Tests.Unit.Internals;

[TestGroup($"{nameof(AWS)}.{nameof(SNS)}")]
public sealed class SimpleNotificationServiceConfigureTests
{
    private static SimpleNotificationServiceConfigure CreateConfigure(IConfiguration? configuration = null) =>
        new(configuration ?? new ConfigurationBuilder().Build());

    [Test]
    [MethodDataSource(typeof(DataSource), nameof(DataSource.NullOrWhitespaces))]
    public async Task Validate_WhenNameIsNullOrWhiteSpace_ReturnsFail(string? name)
    {
        var configure = CreateConfigure();
        var options = new SimpleNotificationServiceOptions { TopicName = "topic", ServiceUrl = "url" };
        var result = configure.Validate(name, options);

        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenOptionsIsNull_ReturnsFail()
    {
        var configure = CreateConfigure();
        var result = configure.Validate("Test", null!);

        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    [MethodDataSource(typeof(DataSource), nameof(DataSource.NullOrWhitespaces))]
    public async Task Validate_WhenTopicNameIsNullOrWhiteSpace_ReturnsFail(string? topicName)
    {
        var configure = CreateConfigure();
        var options = new SimpleNotificationServiceOptions { ServiceUrl = "url", TopicName = topicName };
        var result = configure.Validate("Test", options);

        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    [MethodDataSource(typeof(DataSource), nameof(DataSource.NullOrWhitespaces))]
    public async Task Validate_WhenServiceUrlIsNullOrWhiteSpace_ReturnsFail(string? service)
    {
        var configure = CreateConfigure();
        var options = new SimpleNotificationServiceOptions { TopicName = "topic", ServiceUrl = service };
        var result = configure.Validate("Test", options);

        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenModeIsBasicAuthentication_AndAccessKeyAndSecretKeyAreSet_ReturnsSuccess()
    {
        var configure = CreateConfigure();
        var options = new SimpleNotificationServiceOptions
        {
            TopicName = "topic",
            ServiceUrl = "url",
            Mode = CreationMode.BasicAuthentication,
            AccessKey = "access",
            SecretKey = "secret",
        };
        var result = configure.Validate("Test", options);

        _ = await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Validate_WhenModeIsNull_DoesNotRequireAccessKeyOrSecretKey()
    {
        var configure = CreateConfigure();
        var options = new SimpleNotificationServiceOptions
        {
            TopicName = "topic",
            ServiceUrl = "url",
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
                    new KeyValuePair<string, string?>("HealthChecks:AWSSNS:Test:TopicName", "topic"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSSNS:Test:ServiceUrl", "url"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSSNS:Test:AccessKey", "access"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSSNS:Test:SecretKey", "secret"),
                ]
            )
            .Build();
        var configure = CreateConfigure(config);
        var options = new SimpleNotificationServiceOptions();
        configure.Configure("Test", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(options).IsNotNull();
            _ = await Assert.That(options.TopicName).IsEqualTo("topic");
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
        var options = new SimpleNotificationServiceOptions();
        _ = Assert.Throws<ArgumentException>(() => configure.Configure(name, options));
    }

    [Test]
    public void Configure_WithNullOrWhiteSpaceName_ThrowsArgumentNullException()
    {
        var configure = CreateConfigure();
        var options = new SimpleNotificationServiceOptions();
        _ = Assert.Throws<ArgumentNullException>(() => configure.Configure(null, options));
    }

    [Test]
    public async Task Configure_Parameterless_UsesDefaultName()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                [
                    new KeyValuePair<string, string?>("HealthChecks:AWSSNS:Default:TopicName", "topic-default"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSSNS:Default:ServiceUrl", "url-default"),
                ]
            )
            .Build();
        var configure = CreateConfigure(config);
        var options = new SimpleNotificationServiceOptions();
        configure.Configure("Default", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(options).IsNotNull();
            _ = await Assert.That(options.TopicName).IsEqualTo("topic-default");
            _ = await Assert.That(options.ServiceUrl).IsEqualTo("url-default");
        }
    }

    [Test]
    [MethodDataSource(typeof(DataSource), nameof(DataSource.TimeoutsInvalid))]
    public async Task Validate_WhenTimeoutLessThanInfinite_ReturnsFail(int timeout)
    {
        var configure = CreateConfigure();
        var options = new SimpleNotificationServiceOptions
        {
            TopicName = "topic",
            ServiceUrl = "url",
            Timeout = timeout,
        };
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
        var options = new SimpleNotificationServiceOptions
        {
            TopicName = "topic",
            ServiceUrl = "url",
            Timeout = timeout,
        };
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
        var options = new SimpleNotificationServiceOptions
        {
            TopicName = "topic",
            ServiceUrl = "url",
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
        var options = new SimpleNotificationServiceOptions
        {
            TopicName = "topic",
            ServiceUrl = "url",
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
                    new KeyValuePair<string, string?>("HealthChecks:AWSSNS:Test:TopicName", "topic"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSSNS:Test:ServiceUrl", "url"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSSNS:Test:AccessKey", "access"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSSNS:Test:SecretKey", "secret"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSSNS:Test:Timeout", "500"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSSNS:Test:Subscription", "sub123"),
                    new KeyValuePair<string, string?>("HealthChecks:AWSSNS:Test:Mode", "BasicAuthentication"),
                ]
            )
            .Build();
        var configure = CreateConfigure(config);
        var options = new SimpleNotificationServiceOptions();
        configure.Configure("Test", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(options).IsNotNull();
            _ = await Assert.That(options.TopicName).IsEqualTo("topic");
            _ = await Assert.That(options.ServiceUrl).IsEqualTo("url");
            _ = await Assert.That(options.AccessKey).IsEqualTo("access");
            _ = await Assert.That(options.SecretKey).IsEqualTo("secret");
            _ = await Assert.That(options.Timeout).IsEqualTo(500);
            _ = await Assert.That(options.Subscription).IsEqualTo("sub123");
            _ = await Assert.That(options.Mode).IsEqualTo(CreationMode.BasicAuthentication);
        }
    }
}
