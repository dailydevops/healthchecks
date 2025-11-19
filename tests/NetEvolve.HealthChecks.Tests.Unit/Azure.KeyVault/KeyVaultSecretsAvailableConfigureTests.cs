namespace NetEvolve.HealthChecks.Tests.Unit.Azure.KeyVault;

using System;
using System.Threading.Tasks;
using global::Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.KeyVault;

[TestGroup($"{nameof(Azure)}.{nameof(KeyVault)}")]
public class KeyVaultSecretsAvailableConfigureTests
{
    [Test]
    public void Configure_OnlyOptions_ThrowsArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configure = new KeyVaultSecretsAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );
        var options = new KeyVaultSecretsAvailableOptions();

        // Act / Assert
        _ = Assert.Throws<ArgumentException>("name", () => configure.Configure(options));
    }

    [Test]
    [Arguments(false, null, null, null)]
    [Arguments(false, null, "Test", null)]
    [Arguments(false, null, "Test", -2)]
    [Arguments(false, null, "Test", KeyVaultClientCreationMode.ServiceProvider, false)]
    [Arguments(true, null, "Test", KeyVaultClientCreationMode.ServiceProvider, true)]
    [Arguments(false, null, "Test", KeyVaultClientCreationMode.DefaultAzureCredentials, null)]
    [Arguments(false, null, "Test", KeyVaultClientCreationMode.DefaultAzureCredentials, "/test")]
    [Arguments(true, null, "Test", KeyVaultClientCreationMode.DefaultAzureCredentials, "https://test.vault.azure.net")]
    public async Task Validate_Theory_Expected(
        bool expectedResult,
        string? expectedMessage,
        string? name,
        object? modeOrTimeout,
        object? additionalParam = null
    )
    {
        // Arrange
        var services = new ServiceCollection();

        if (modeOrTimeout is KeyVaultClientCreationMode.ServiceProvider && additionalParam is true)
        {
            _ = services.AddSingleton<SecretClient>(_ => null!);
        }

        var configure = new KeyVaultSecretsAvailableConfigure(
            new ConfigurationBuilder().Build(),
            services.BuildServiceProvider()
        );

        KeyVaultSecretsAvailableOptions? options = null;
        if (modeOrTimeout is int timeout)
        {
            options = new KeyVaultSecretsAvailableOptions { Timeout = timeout };
        }
        else if (modeOrTimeout is KeyVaultClientCreationMode mode)
        {
            options = new KeyVaultSecretsAvailableOptions { Mode = mode };

            if (additionalParam is string uriString && !string.IsNullOrEmpty(uriString))
            {
                options.VaultUri = uriString.StartsWith("http", StringComparison.Ordinal)
                    ? new Uri(uriString)
                    : new Uri(uriString, UriKind.Relative);
            }
        }

        // Act
        var result = configure.Validate(name, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsEqualTo(expectedResult);

            if (!string.IsNullOrWhiteSpace(expectedMessage))
            {
                _ = await Assert.That(result.FailureMessage).Contains(expectedMessage);
            }
        }
    }
}
