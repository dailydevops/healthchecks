namespace NetEvolve.HealthChecks.Tests.Unit.GCP.Firestore;

using System.Threading.Tasks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.GCP.Firestore;

[TestGroup(nameof(Firestore))]
public sealed class FirestoreOptionsTests
{
    [Test]
    public async Task Options_NotSame_Expected()
    {
        var options1 = new FirestoreOptions { Timeout = 100 };
        var options2 = options1 with { };

        _ = await Assert.That(options1).IsEqualTo(options2).And.IsNotSameReferenceAs(options2);
    }
}
