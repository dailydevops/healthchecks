namespace NetEvolve.HealthChecks.Tests.Integration.SqlServer;

using Xunit;

[CollectionDefinition(nameof(SqlServer))]
public sealed class SqlServerCollectionFixture : ICollectionFixture<SqlServerDatabase>
{
    // This class is used to define a collection fixture for the SqlServerDatabase.
    // It allows sharing the same instance of SqlServerDatabase across multiple tests.
}
