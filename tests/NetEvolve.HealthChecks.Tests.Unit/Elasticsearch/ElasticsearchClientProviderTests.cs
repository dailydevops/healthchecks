namespace NetEvolve.HealthChecks.Tests.Unit.Elasticsearch;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.Elasticsearch;

[TestGroup(nameof(Elasticsearch))]
public sealed class ElasticsearchClientProviderTests
{
    //[Test]
    //public void CreateClient_OptionsWithoutConnectionStrings_ThrowsArgumentNullException()
    //{
    //    // Arrange
    //    var options = new ElasticsearchOptions
    //    {
    //        Mode = ElasticsearchClientCreationMode.UsernameAndPassword,
    //        Username = "username",
    //        Password = "password",
    //    };

    //    // Act
    //    // Assert
    //    _ = Assert.Throws<ArgumentNullException>(() => ElasticsearchClientProvider.CreateClient(options));
    //}

    //[Test]
    //[MethodDataSource(nameof(InvalidArgumentsTestData))]
    //public void CreateClient_Theory_Expected(
    //    Type expectedException,
    //    ElasticsearchClientCreationMode mode,
    //    IList<string>? connectionStrings,
    //    string? username,
    //    string? password
    //)
    //{
    //    var options = new ElasticsearchOptions
    //    {
    //        Mode = mode,
    //        Username = username,
    //        Password = password,
    //    };

    //    foreach (var connectionString in connectionStrings ?? [])
    //    {
    //        options.ConnectionStrings.Add(connectionString);
    //    }

    //    _ = Assert.Throws(expectedException, () => ElasticsearchClientProvider.CreateClient(options));
    //}

    //public static IEnumerable<
    //    Func<(Type, ElasticsearchClientCreationMode, IList<string>?, string?, string?)>
    //> InvalidArgumentsTestData()
    //{
    //    yield return () =>
    //        (
    //            typeof(ArgumentOutOfRangeException),
    //            (ElasticsearchClientCreationMode)(-1),
    //            ["connection-string"],
    //            "username",
    //            "password"
    //        );
    //    yield return () =>
    //        (
    //            typeof(ArgumentOutOfRangeException),
    //            ElasticsearchClientCreationMode.ServiceProvider,
    //            ["connection-string"],
    //            "username",
    //            "password"
    //        );
    //    yield return () =>
    //        (typeof(ArgumentNullException), ElasticsearchClientCreationMode.UsernameAndPassword, null, "username", "password");
    //}
}
