namespace NetEvolve.HealthChecks.Tests.Unit.Minio;

using System.Threading;
using System.Threading.Tasks;
using global::Minio;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Minio;
using NSubstitute;

[TestGroup(nameof(Minio))]
public class MinioHealthCheckTests
{
    [Test]
    public async Task DefaultCommandAsync_WhenBucketExists_ReturnsTrue()
    {
        // Arrange
        var client = Substitute.For<IMinioClient>();
        _ = client
            .BucketExistsAsync(Arg.Any<global::Minio.DataModel.Args.BucketExistsArgs>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        // Act
        var result = await MinioHealthCheck.DefaultCommandAsync(client, "test-bucket", CancellationToken.None);

        // Assert
        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task DefaultCommandAsync_WhenBucketDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var client = Substitute.For<IMinioClient>();
        _ = client
            .BucketExistsAsync(Arg.Any<global::Minio.DataModel.Args.BucketExistsArgs>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // Act
        var result = await MinioHealthCheck.DefaultCommandAsync(client, "non-existing-bucket", CancellationToken.None);

        // Assert
        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task CheckHealthAsync_WhenCommandReturnsFalse_ShouldReturnUnhealthyWithMessage()
    {
        // Arrange
        var client = Substitute.For<IMinioClient>();
        var options = new MinioOptions
        {
            KeyedService = null,
            BucketName = "test-bucket",
            Timeout = 1000,
            CommandAsync = async (_, _, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                return false;
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<MinioOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(client);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new MinioHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert
                .That(result.Description)
                .IsEqualTo("test: The Minio command did not return a valid result.");
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenCommandReturnsTrue_ShouldReturnHealthy()
    {
        // Arrange
        var client = Substitute.For<IMinioClient>();
        var options = new MinioOptions
        {
            KeyedService = null,
            BucketName = "test-bucket",
            Timeout = 1000,
            CommandAsync = async (_, _, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                return true;
            },
        };

        var optionsMonitor = Substitute.For<IOptionsMonitor<MinioOptions>>();
        _ = optionsMonitor.Get("test").Returns(options);

        var serviceCollection = new ServiceCollection();
        _ = serviceCollection.AddSingleton(client);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var healthCheck = new MinioHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("test", healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Healthy);
            _ = await Assert.That(result.Description).IsEqualTo("test: Healthy");
        }
    }
}
