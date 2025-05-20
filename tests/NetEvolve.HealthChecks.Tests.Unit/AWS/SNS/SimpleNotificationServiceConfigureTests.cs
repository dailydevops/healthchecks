namespace NetEvolve.HealthChecks.Tests.Unit.AWS.SNS;

using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.AWS.SNS;
using Xunit;

[TestGroup($"{nameof(AWS)}.{nameof(SNS)}")]
public sealed class SimpleNotificationServiceConfigureTests
{
    private static SimpleNotificationServiceConfigure CreateConfigure(IConfiguration? configuration = null) =>
        new(configuration ?? new ConfigurationBuilder().Build());

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenNameIsNullOrWhiteSpace_ReturnsFail(string? name)
    {
        var configure = CreateConfigure();
        var options = new SimpleNotificationServiceOptions { TopicName = "topic", ServiceUrl = "url" };
        Assert.True(configure.Validate(name, options).Failed);
    }

    [Fact]
    public void Validate_WhenOptionsIsNull_ReturnsFail()
    {
        var configure = CreateConfigure();
        Assert.True(configure.Validate("Test", null!).Failed);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_WhenTopicNameIsNullOrWhiteSpace_ReturnsFail(string? topicName)
    {
        var configure = CreateConfigure();
        var options = new SimpleNotificationServiceOptions { ServiceUrl = "url", TopicName = topicName };
        Assert.True(configure.Validate("Test", options).Failed);
    }

    [Fact]
    public void Validate_WhenServiceUrlIsNullOrWhiteSpace_ReturnsFail()
    {
        var configure = CreateConfigure();
        var options = new SimpleNotificationServiceOptions { TopicName = "topic", ServiceUrl = null };
        Assert.True(configure.Validate("Test", options).Failed);
        options.ServiceUrl = "   ";
        Assert.True(configure.Validate("Test", options).Failed);
    }

    [Fact]
    public void Validate_WhenModeIsBasicAuthentication_AndAccessKeyAndSecretKeyAreSet_ReturnsSuccess()
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
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Validate_WhenModeIsNull_DoesNotRequireAccessKeyOrSecretKey()
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
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Configure_WithValidName_BindsConfiguration()
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
        Assert.Equal("topic", options.TopicName);
        Assert.Equal("url", options.ServiceUrl);
        Assert.Equal("access", options.AccessKey);
        Assert.Equal("secret", options.SecretKey);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Configure_WithNullOrWhiteSpaceName_ThrowsArgumentException(string? name)
    {
        var configure = CreateConfigure();
        var options = new SimpleNotificationServiceOptions();
        _ = Assert.Throws<ArgumentException>(() => configure.Configure(name, options));
    }

    [Fact]
    public void Configure_WithNullOrWhiteSpaceName_ThrowsArgumentNullException()
    {
        var configure = CreateConfigure();
        var options = new SimpleNotificationServiceOptions();
        _ = Assert.Throws<ArgumentNullException>(() => configure.Configure(null!, options));
    }

    [Fact]
    public void Configure_Parameterless_UsesDefaultName()
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
        Assert.Equal("topic-default", options.TopicName);
        Assert.Equal("url-default", options.ServiceUrl);
    }
}
