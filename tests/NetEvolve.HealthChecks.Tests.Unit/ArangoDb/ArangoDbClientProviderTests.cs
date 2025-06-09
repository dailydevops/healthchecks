namespace NetEvolve.HealthChecks.Tests.Unit.ArangoDb;

using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.ArangoDb;

[TestGroup(nameof(ArangoDb))]
public sealed class ArangoDbClientProviderTests
{
    [Test]
    [MethodDataSource(nameof(InvalidArgumentsTestData))]
    public void CreateClient_Theory_Expected(
        Type expectedException,
        ArangoDbClientCreationMode mode,
        string? transportAddress,
        string? username,
        string? password
    )
    {
        var options = new ArangoDbOptions
        {
            Mode = mode,
            TransportAddress = transportAddress,
            Username = username,
            Password = password,
        };
        _ = Assert.Throws(expectedException, () => ArangoDbClientProvider.CreateClient(options));
    }

    public static IEnumerable<
        Func<(Type, ArangoDbClientCreationMode, string?, string?, string?)>
    > InvalidArgumentsTestData()
    {
        yield return () =>
            (
                typeof(ArgumentOutOfRangeException),
                (ArangoDbClientCreationMode)(-1),
                "transport-address",
                "username",
                "password"
            );
        yield return () =>
            (
                typeof(ArgumentOutOfRangeException),
                ArangoDbClientCreationMode.ServiceProvider,
                "transport-address",
                "username",
                "password"
            );
        yield return () =>
            (typeof(ArgumentNullException), ArangoDbClientCreationMode.Internal, null, "username", "password");
        yield return () =>
            (typeof(ArgumentNullException), ArangoDbClientCreationMode.Internal, "transport-address", null, "password");
        yield return () =>
            (typeof(ArgumentNullException), ArangoDbClientCreationMode.Internal, "transport-address", "username", null);
    }
}
