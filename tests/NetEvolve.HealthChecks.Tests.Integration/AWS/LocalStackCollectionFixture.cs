namespace NetEvolve.HealthChecks.Tests.Integration.AWS;

[CollectionDefinition(nameof(AWS))]
public sealed class LocalStackCollectionFixture : ICollectionFixture<LocalStackInstance>
{
    // This class is used to define a collection fixture for the LocalStackInstance.
    // It allows sharing the same instance of LocalStackInstance across multiple tests.
}
