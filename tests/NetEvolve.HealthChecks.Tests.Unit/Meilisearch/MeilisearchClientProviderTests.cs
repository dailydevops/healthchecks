namespace NetEvolve.HealthChecks.Tests.Unit.Meilisearch;

using System;
using System.Collections.Generic;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Meilisearch;

[TestGroup(nameof(Meilisearch))]
public sealed class MeilisearchClientProviderTests
{
    [Test]
    [MethodDataSource(nameof(InvalidArgumentsTestData))]
    public void CreateClient_Theory_Expected(Type expectedException, MeilisearchClientCreationMode mode, string? host)
    {
        var options = new MeilisearchOptions { Mode = mode, Host = host };
        _ = Assert.Throws(expectedException, () => MeilisearchClientProvider.CreateClient(options));
    }

    public static IEnumerable<Func<(Type, MeilisearchClientCreationMode, string?)>> InvalidArgumentsTestData()
    {
        yield return () =>
            (typeof(ArgumentOutOfRangeException), (MeilisearchClientCreationMode)(-1), "http://localhost:7700");
        yield return () =>
            (
                typeof(ArgumentOutOfRangeException),
                MeilisearchClientCreationMode.ServiceProvider,
                "http://localhost:7700"
            );
        yield return () => (typeof(ArgumentNullException), MeilisearchClientCreationMode.Internal, null);
    }
}
