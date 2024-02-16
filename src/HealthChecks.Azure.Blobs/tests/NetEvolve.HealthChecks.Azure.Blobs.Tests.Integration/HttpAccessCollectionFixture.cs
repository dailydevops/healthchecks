namespace NetEvolve.HealthChecks.Azure.Tests.Integration;

using Xunit;

[CollectionDefinition(nameof(HttpAccessCollectionFixture))]
public class HttpAccessCollectionFixture : ICollectionFixture<AzuriteHttpAccess>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
