namespace NetEvolve.HealthChecks.Tests.Unit.Elasticsearch;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Elasticsearch;

[TestGroup(nameof(Elasticsearch))]
public sealed class ElasticsearchClientProviderTests
{
    [Test]
    [MethodDataSource(nameof(InvalidArgumentsTestData))]
    public void CreateClient_Theory_Expected(
        Type expectedException,
        ElasticsearchClientCreationMode mode,
        string? connectionString,
        string? username,
        string? password
    )
    {
        var options = new ElasticsearchOptions
        {
            Mode = mode,
            ConnectionString = connectionString,
            Username = username,
            Password = password,
        };
        _ = Assert.Throws(expectedException, () => ElasticsearchClientProvider.CreateClient(options));
    }

    public static IEnumerable<
        Func<(Type, ElasticsearchClientCreationMode, string?, string?, string?)>
    > InvalidArgumentsTestData()
    {
        yield return () =>
            (
                typeof(ArgumentOutOfRangeException),
                (ElasticsearchClientCreationMode)(-1),
                "connection-string",
                "username",
                "password"
            );
        yield return () =>
            (
                typeof(ArgumentOutOfRangeException),
                ElasticsearchClientCreationMode.ServiceProvider,
                "connection-string",
                "username",
                "password"
            );
        yield return () =>
            (typeof(ArgumentNullException), ElasticsearchClientCreationMode.Internal, null, "username", "password");
    }
}
