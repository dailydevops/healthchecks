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
        _ = await Assert.That(options.AccessKey).IsNull();
        _ = await Assert.That(options.SecretKey).IsNull();
        _ = await Assert.That(options.BucketName).IsNull();
        _ = await Assert.That(options.ServiceUrl).IsNull();
        _ = await Assert.That(options.Mode).IsNull();
        _ = await Assert.That(options.RegionEndpoint).IsNull();
    }

    [Test]
    public async Task MinioOptions_GetCredentials_WhenModeIsBasicAuthentication_ReturnsCredentials()
    {
        // Arrange
        var options = new MinioOptions
        {
            Mode = CreationMode.BasicAuthentication,
            AccessKey = "test-access-key",
            SecretKey = "test-secret-key",
        };

        // Act
        var credentials = options.GetCredentials();

        // Assert
        _ = await Assert.That(credentials).IsNotNull();
    }

    [Test]
    public async Task MinioOptions_GetCredentials_WhenModeIsNull_ReturnsNull()
    {
        // Arrange
        var options = new MinioOptions { Mode = null };

        // Act
        var credentials = options.GetCredentials();

        // Assert
        _ = await Assert.That(credentials).IsNull();
    }
}
