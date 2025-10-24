namespace NetEvolve.HealthChecks.Tests.Unit.Azure.KeyVault;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.KeyVault;

[TestGroup($"{nameof(Azure)}.{nameof(KeyVault)}")]
public class KeyVaultSecretAvailableOptionsTests
{
    [Test]
    public async Task Validate_WhenDefaultConstructor_ShouldPass()
    {
        // Arrange
        var options = new KeyVaultSecretAvailableOptions();

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(100);
    }
}
