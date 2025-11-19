namespace NetEvolve.HealthChecks.Tests.Unit.Azure.Search;

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Azure.Search;

[TestGroup($"{nameof(Azure)}.{nameof(Search)}")]
public class ClientCreationTests
{
    [Test]
    public void CreateSearchIndexClient_InvalidMode_ThrowUnreachableException()
    {
        var options = new SearchServiceAvailableOptions { Mode = (SearchIndexClientCreationMode)13 };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(() =>
            ClientCreation.CreateSearchIndexClient(options, serviceProvider)
        );
    }

    [Test]
    public void CreateSearchClient_InvalidMode_ThrowUnreachableException()
    {
        var options = new SearchIndexAvailableOptions
        {
            Mode = (SearchIndexClientCreationMode)13,
            IndexName = "test"
        };
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        _ = Assert.Throws<UnreachableException>(() =>
            ClientCreation.CreateSearchClient(options, serviceProvider)
        );
    }
}
