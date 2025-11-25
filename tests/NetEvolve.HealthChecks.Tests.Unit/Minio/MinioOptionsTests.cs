namespace NetEvolve.HealthChecks.Tests.Unit.Minio;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Minio;

[TestGroup(nameof(Minio))]
public class MinioOptionsTests
{
    [Test]
    public async Task MinioOptions_DefaultConstructor_ExpectDefaultValues()
    {
        // Arrange
        var options = new MinioOptions();

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(100);
        _ = await Assert.That(options.BucketName).IsNull();
        _ = await Assert.That(options.KeyedService).IsNull();
        _ = await Assert.That(options.CommandAsync).IsNotNull();
    }
}
