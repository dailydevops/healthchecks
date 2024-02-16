namespace NetEvolve.HealthChecks.Azure.Tests.Integration;

using Xunit;

[CollectionDefinition(nameof(HttpsAccessCollectionFixture))]
public class HttpsAccessCollectionFixture : ICollectionFixture<AzuriteHttpsAccess>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
