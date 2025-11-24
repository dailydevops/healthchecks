namespace NetEvolve.HealthChecks.Tests.Unit.Minio;

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Minio;

[TestGroup(nameof(Minio))]
public class MinioConfigureTests
{
    [Test]
    public async Task Validate_WhenArgumentNameNull_ReturnFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MinioConfigure(configuration);
        const string? name = default;

        // Act
        var result = configure.Validate(name, new MinioOptions());

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
        _ = await Assert.That(result.FailureMessage).IsEqualTo("The name cannot be null or whitespace.");
    }

    [Test]
    public async Task Validate_WhenArgumentOptionsNull_ReturnFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MinioConfigure(configuration);
        const string name = "Test";

        // Act
        var result = configure.Validate(name, null!);

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
        _ = await Assert.That(result.FailureMessage).IsEqualTo("The option cannot be null.");
    }

    [Test]
    public async Task Validate_WhenArgumentTimeoutMinusTwo_ReturnFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MinioConfigure(configuration);
        const string name = "Test";

        // Act
        var result = configure.Validate(
            name,
            new MinioOptions
            {
                Timeout = -2,
                BucketName = "test-bucket",
                ServiceUrl = "http://localhost:9000",
            }
        );

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
        _ = await Assert
            .That(result.FailureMessage)
            .IsEqualTo("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.");
    }

    [Test]
    public async Task Validate_WhenBucketNameIsNull_ReturnFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MinioConfigure(configuration);
        const string name = "Test";

        // Act
        var result = configure.Validate(name, new MinioOptions { ServiceUrl = "http://localhost:9000" });

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
        _ = await Assert.That(result.FailureMessage).IsEqualTo("The bucket name cannot be null or whitespace.");
    }

    [Test]
    public async Task Validate_WhenServiceUrlIsNull_ReturnFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MinioConfigure(configuration);
        const string name = "Test";

        // Act
        var result = configure.Validate(name, new MinioOptions { BucketName = "test-bucket" });

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
        _ = await Assert.That(result.FailureMessage).IsEqualTo("The service URL cannot be null or whitespace.");
    }

    [Test]
    public async Task Validate_WhenModeIsBasicAuthenticationAndAccessKeyIsNull_ReturnFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MinioConfigure(configuration);
        const string name = "Test";

        // Act
        var result = configure.Validate(
            name,
            new MinioOptions
            {
                BucketName = "test-bucket",
                ServiceUrl = "http://localhost:9000",
                Mode = CreationMode.BasicAuthentication,
                SecretKey = "test-secret",
            }
        );

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
        _ = await Assert.That(result.FailureMessage).IsEqualTo("The access key cannot be null or whitespace.");
    }

    [Test]
    public async Task Validate_WhenModeIsBasicAuthenticationAndSecretKeyIsNull_ReturnFail()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MinioConfigure(configuration);
        const string name = "Test";

        // Act
        var result = configure.Validate(
            name,
            new MinioOptions
            {
                BucketName = "test-bucket",
                ServiceUrl = "http://localhost:9000",
                Mode = CreationMode.BasicAuthentication,
                AccessKey = "test-access",
            }
        );

        // Assert
        _ = await Assert.That(result.Failed).IsTrue();
        _ = await Assert.That(result.FailureMessage).IsEqualTo("The secret key cannot be null or whitespace.");
    }

    [Test]
    public async Task Validate_WhenArgumentsValid_ReturnSuccess()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var configure = new MinioConfigure(configuration);
        const string name = "Test";

        // Act
        var result = configure.Validate(
            name,
            new MinioOptions
            {
                BucketName = "test-bucket",
                ServiceUrl = "http://localhost:9000",
                Mode = CreationMode.BasicAuthentication,
                AccessKey = "test-access",
                SecretKey = "test-secret",
            }
        );

        // Assert
        _ = await Assert.That(result.Succeeded).IsTrue();
    }
}
