namespace NetEvolve.HealthChecks.Tests.Unit.Elasticsearch.Cluster;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Elasticsearch.Cluster;

[TestGroup($"{nameof(Elasticsearch)}.{nameof(Cluster)}")]
public sealed class ElasticsearchClusterClientProviderTests
{
    [Test]
    [MethodDataSource(nameof(InvalidArgumentsTestData))]
    public void CreateClient_Theory_Expected(
        Type expectedException,
        ElasticsearchClusterClientCreationMode mode,
        IEnumerable<string>? connectionStrings,
        string? username,
        string? password
    )
    {
        var options = new ElasticsearchClusterOptions
        {
            Mode = mode,
            ConnectionStrings = connectionStrings,
            Username = username,
            Password = password,
        };
        _ = Assert.Throws(expectedException, () => ElasticsearchClusterClientProvider.CreateClient(options));
    }

    public static IEnumerable<
        Func<(Type, ElasticsearchClusterClientCreationMode, IEnumerable<string>?, string?, string?)>
    > InvalidArgumentsTestData()
    {
        yield return () =>
            (
                typeof(ArgumentOutOfRangeException),
                (ElasticsearchClusterClientCreationMode)(-1),
                ["connection-string"],
                "username",
                "password"
            );
        yield return () =>
            (
                typeof(ArgumentOutOfRangeException),
                ElasticsearchClusterClientCreationMode.ServiceProvider,
                ["connection-string"],
                "username",
                "password"
            );
        yield return () =>
            (typeof(ArgumentException), ElasticsearchClusterClientCreationMode.Internal, null, "username", "password");
        yield return () =>
            (typeof(ArgumentException), ElasticsearchClusterClientCreationMode.Internal, [], "username", "password");
    }
}
