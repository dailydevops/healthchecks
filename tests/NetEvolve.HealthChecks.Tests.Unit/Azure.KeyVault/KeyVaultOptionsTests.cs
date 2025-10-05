namespace NetEvolve.HealthChecks.Tests.Unit.Azure.KeyVault;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.KeyVault;

[TestGroup($"{nameof(Azure)}.{nameof(KeyVault)}")]
public sealed class KeyVaultOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new KeyVaultOptions();
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
