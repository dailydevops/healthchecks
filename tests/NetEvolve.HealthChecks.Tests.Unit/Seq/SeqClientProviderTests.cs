namespace NetEvolve.HealthChecks.Tests.Unit.Seq;

using System;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Seq;

[TestGroup(nameof(Seq))]
public sealed class SeqClientProviderTests
{
    [Test]
    [MethodDataSource(nameof(InvalidArgumentsTestData))]
    public void CreateClient_Theory_Expected(Type expectedException, SeqClientCreationMode mode, Uri? serverUrl)
    {
        var options = new SeqOptions { Mode = mode, ServerUrl = serverUrl };
        _ = Assert.Throws(expectedException, () => SeqClientProvider.CreateClient(options));
    }

    public static IEnumerable<Func<(Type, SeqClientCreationMode, Uri?)>> InvalidArgumentsTestData()
    {
        yield return () =>
            (typeof(ArgumentOutOfRangeException), (SeqClientCreationMode)(-1), new Uri("http://localhost:5341"));
        yield return () =>
            (
                typeof(ArgumentOutOfRangeException),
                SeqClientCreationMode.ServiceProvider,
                new Uri("http://localhost:5341")
            );
        yield return () => (typeof(ArgumentNullException), SeqClientCreationMode.ServerUrl, null);
    }
}
