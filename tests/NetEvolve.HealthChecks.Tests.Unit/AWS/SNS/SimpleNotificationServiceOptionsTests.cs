namespace NetEvolve.HealthChecks.Tests.Unit.AWS.SNS;

using Amazon;
using Amazon.Runtime;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.AWS.SNS;
using Xunit;

[TestGroup($"{nameof(AWS)}.{nameof(SNS)}")]
public sealed class SimpleNotificationServiceOptionsTests
{
    [Fact]
    public void GetCredentials_WhenModeIsNull_ReturnsNull()
    {
        var options = new SimpleNotificationServiceOptions { Mode = null };
        Assert.Null(options.GetCredentials());
    }

    [Fact]
    public void GetCredentials_WhenModeIsBasicAuthentication_ReturnsBasicAWSCredentials()
    {
        var options = new SimpleNotificationServiceOptions
        {
            Mode = CreationMode.BasicAuthentication,
            AccessKey = "access-key",
            SecretKey = "secret-key",
        };

        var credentials = options.GetCredentials();

        Assert.NotNull(credentials);
        _ = Assert.IsType<BasicAWSCredentials>(credentials);
    }
}
