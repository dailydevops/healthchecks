namespace NetEvolve.HealthChecks.Tests.Unit.Azure.KeyVault;

using System;
using System.Threading;
using System.Threading.Tasks;
using global::Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.KeyVault;
using NSubstitute;

[TestGroup($"{nameof(Azure)}.{nameof(KeyVault)}")]
public sealed class KeyVaultHealthCheckTests
{
    [Test]
    public async Task CheckHealthAsync_WhenContextNull_ThrowArgumentNullException()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<KeyVaultOptions>>();
        var check = new KeyVaultHealthCheck(serviceProvider, optionsMonitor);

        // Act
        async Task Act() => _ = await check.CheckHealthAsync(null!, default);

        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>("context", Act);
    }

    [Test]
    public async Task CheckHealthAsync_WhenCancellationTokenIsCancelled_ShouldReturnUnhealthy()
    {
        // Arrange
        var testName = "TestHealthCheck";
        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<KeyVaultOptions>>();
        var options = new KeyVaultOptions 
        { 
            Mode = KeyVaultClientCreationMode.DefaultAzureCredentials,
            VaultUri = new Uri("https://test.vault.azure.net/"),
            Timeout = 100
        };

        _ = optionsMonitor.Get(testName).Returns(options);

        var healthCheck = new KeyVaultHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(testName, healthCheck, HealthStatus.Unhealthy, null),
        };

        using var cancellationToken = new CancellationTokenSource();
        cancellationToken.Cancel();

        // Act
        var result = await healthCheck.CheckHealthAsync(context, cancellationToken.Token);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).Contains($"{testName}: ", StringComparison.Ordinal);
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenOptionsAreNull_ShouldReturnUnhealthy()
    {
        // Arrange
        var testName = "TestHealthCheck";
        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<KeyVaultOptions>>();

        _ = optionsMonitor.Get(testName).Returns((KeyVaultOptions?)null);

        var healthCheck = new KeyVaultHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(testName, healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).Contains($"{testName}: ", StringComparison.Ordinal);
        }
    }

    [Test]
    public async Task CheckHealthAsync_WhenConnectionFails_ShouldReturnUnhealthy()
    {
        // Arrange
        var testName = "TestHealthCheck";
        var serviceProvider = Substitute.For<IServiceProvider>();
        var optionsMonitor = Substitute.For<IOptionsMonitor<KeyVaultOptions>>();
        var options = new KeyVaultOptions 
        { 
            Mode = KeyVaultClientCreationMode.DefaultAzureCredentials,
            VaultUri = new Uri("https://invalid-vault.vault.azure.net/"),
            Timeout = 100
        };

        _ = optionsMonitor.Get(testName).Returns(options);

        var healthCheck = new KeyVaultHealthCheck(serviceProvider, optionsMonitor);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(testName, healthCheck, HealthStatus.Unhealthy, null),
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Status).IsEqualTo(HealthStatus.Unhealthy);
            _ = await Assert.That(result.Description).Contains($"{testName}: ", StringComparison.Ordinal);
        }
    }
}