namespace NetEvolve.HealthChecks.Tests.Unit.AWS.CloudWatch;

using System;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.AWS.CloudWatch;
using NetEvolve.HealthChecks.Tests.Unit.Internals;

[TestGroup($"{nameof(AWS)}.{nameof(CloudWatch)}")]
public sealed class CloudWatchConfigureTests
{
    private static CloudWatchConfigure CreateConfigure(IConfiguration? configuration = null) =>
        new(configuration ?? new ConfigurationBuilder().Build());

    [Test]
    [MethodDataSource(typeof(DataSource), nameof(DataSource.NullOrWhitespaces))]
    public async Task Validate_WhenNameIsNullOrWhiteSpace_ReturnsFail(string? name)
    {
        var configure = CreateConfigure();
        var options = new CloudWatchOptions { AlarmName = "alarm", ServiceUrl = "url" };
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
    public async Task Validate_WhenAlarmNameIsNullOrWhiteSpace_ReturnsFail(string? alarmName)
    {
        var configure = CreateConfigure();
        var options = new CloudWatchOptions { ServiceUrl = "url", AlarmName = alarmName };
        var result = configure.Validate("Test", options);

        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    [MethodDataSource(typeof(DataSource), nameof(DataSource.NullOrWhitespaces))]
    public async Task Validate_WhenServiceUrlIsNullOrWhiteSpace_ReturnsFail(string? service)
    {
        var configure = CreateConfigure();
        var options = new CloudWatchOptions { AlarmName = "alarm", ServiceUrl = service };
        var result = configure.Validate("Test", options);

        _ = await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_WhenModeIsBasicAuthentication_AndAccessKeyAndSecretKeyAreSet_ReturnsSuccess()
    {
        var configure = CreateConfigure();
        var options = new CloudWatchOptions
        {
            AlarmName = "alarm",
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
        var options = new CloudWatchOptions
        {
            AlarmName = "alarm",
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
            .AddInMemoryCollection([
                new KeyValuePair<string, string?>("HealthChecks:AWSCloudWatch:Test:AlarmName", "alarm"),
                new KeyValuePair<string, string?>("HealthChecks:AWSCloudWatch:Test:ServiceUrl", "url"),
                new KeyValuePair<string, string?>("HealthChecks:AWSCloudWatch:Test:AccessKey", "access"),
                new KeyValuePair<string, string?>("HealthChecks:AWSCloudWatch:Test:SecretKey", "secret"),
            ])
            .Build();
        var configure = CreateConfigure(config);
        var options = new CloudWatchOptions();
        configure.Configure("Test", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(options).IsNotNull();
            _ = await Assert.That(options.AlarmName).IsEqualTo("alarm");
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
        var options = new CloudWatchOptions();
        _ = Assert.Throws<ArgumentException>(() => configure.Configure(name, options));
    }

    [Test]
    public void Configure_WithNullOrWhiteSpaceName_ThrowsArgumentNullException()
    {
        var configure = CreateConfigure();
        var options = new CloudWatchOptions();
        _ = Assert.Throws<ArgumentNullException>(() => configure.Configure(null, options));
    }

    [Test]
    public async Task Configure_Parameterless_UsesDefaultName()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection([
                new KeyValuePair<string, string?>("HealthChecks:AWSCloudWatch:Default:AlarmName", "alarm-default"),
                new KeyValuePair<string, string?>("HealthChecks:AWSCloudWatch:Default:ServiceUrl", "url-default"),
            ])
            .Build();
        var configure = CreateConfigure(config);
        var options = new CloudWatchOptions();
        configure.Configure("Default", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(options).IsNotNull();
            _ = await Assert.That(options.AlarmName).IsEqualTo("alarm-default");
            _ = await Assert.That(options.ServiceUrl).IsEqualTo("url-default");
        }
    }

    [Test]
    [MethodDataSource(typeof(DataSource), nameof(DataSource.TimeoutsInvalid))]
    public async Task Validate_WhenTimeoutLessThanInfinite_ReturnsFail(int timeout)
    {
        var configure = CreateConfigure();
        var options = new CloudWatchOptions
        {
            AlarmName = "alarm",
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
        var options = new CloudWatchOptions
        {
            AlarmName = "alarm",
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
        var options = new CloudWatchOptions
        {
            AlarmName = "alarm",
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
        var options = new CloudWatchOptions
        {
            AlarmName = "alarm",
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
            .AddInMemoryCollection([
                new KeyValuePair<string, string?>("HealthChecks:AWSCloudWatch:Test:AlarmName", "alarm"),
                new KeyValuePair<string, string?>("HealthChecks:AWSCloudWatch:Test:ServiceUrl", "url"),
                new KeyValuePair<string, string?>("HealthChecks:AWSCloudWatch:Test:AccessKey", "access"),
                new KeyValuePair<string, string?>("HealthChecks:AWSCloudWatch:Test:SecretKey", "secret"),
                new KeyValuePair<string, string?>("HealthChecks:AWSCloudWatch:Test:Timeout", "10000"),
                new KeyValuePair<string, string?>("HealthChecks:AWSCloudWatch:Test:Mode", "BasicAuthentication"),
            ])
            .Build();
        var configure = CreateConfigure(config);
        var options = new CloudWatchOptions();
        configure.Configure("Test", options);

        using (Assert.Multiple())
        {
            _ = await Assert.That(options).IsNotNull();
            _ = await Assert.That(options.AlarmName).IsEqualTo("alarm");
            _ = await Assert.That(options.ServiceUrl).IsEqualTo("url");
            _ = await Assert.That(options.AccessKey).IsEqualTo("access");
            _ = await Assert.That(options.SecretKey).IsEqualTo("secret");
            _ = await Assert.That(options.Timeout).IsEqualTo(10000);
            _ = await Assert.That(options.Mode).IsEqualTo(CreationMode.BasicAuthentication);
        }
    }
}
