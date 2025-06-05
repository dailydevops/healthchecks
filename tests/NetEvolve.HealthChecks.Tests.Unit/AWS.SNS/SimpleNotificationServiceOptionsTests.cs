namespace NetEvolve.HealthChecks.Tests.Unit.AWS.SNS;

using Amazon.Runtime;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.AWS.SNS;

[TestGroup($"{nameof(AWS)}.{nameof(SNS)}")]
public sealed class SimpleNotificationServiceOptionsTests
{
    [Test]
    public async Task GetCredentials_WhenModeIsNull_ReturnsNull()
    {
        var options = new SimpleNotificationServiceOptions { Mode = null };
        _ = await Assert.That(options.GetCredentials()).IsNull();
    }

    [Test]
    public async Task GetCredentials_WhenModeIsBasicAuthentication_ReturnsBasicAWSCredentials()
    {
        var options = new SimpleNotificationServiceOptions
        {
            Mode = CreationMode.BasicAuthentication,
            AccessKey = "access-key",
            SecretKey = "secret-key",
        };

        var credentials = options.GetCredentials();

        _ = await Assert.That(credentials).IsNotNull().And.IsTypeOf<BasicAWSCredentials>();
    }

    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new SimpleNotificationServiceOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
