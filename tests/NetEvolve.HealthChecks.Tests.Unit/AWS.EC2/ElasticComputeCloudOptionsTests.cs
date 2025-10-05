namespace NetEvolve.HealthChecks.Tests.Unit.AWS.EC2;

using Amazon.Runtime;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.AWS.EC2;

[TestGroup($"{nameof(AWS)}.{nameof(EC2)}")]
public sealed class ElasticComputeCloudOptionsTests
{
    [Test]
    public async Task GetCredentials_WhenModeIsNull_ReturnsNull()
    {
        var options = new ElasticComputeCloudOptions { Mode = null };
        _ = await Assert.That(options.GetCredentials()).IsNull();
    }

    [Test]
    public async Task GetCredentials_WhenModeIsBasicAuthentication_ReturnsBasicAWSCredentials()
    {
        var options = new ElasticComputeCloudOptions
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
        var options1 = new ElasticComputeCloudOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
