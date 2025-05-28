namespace NetEvolve.HealthChecks.Tests.Integration.Azure;

using Xunit;

[CollectionDefinition("Azurite")]
public sealed class AzuriteCollectionFixture : ICollectionFixture<AzuriteAccess>
{
    // This class is used to define a collection fixture for the AzuriteAccess.
    // It allows sharing the same instance of AzuriteAccess across multiple tests.
}
