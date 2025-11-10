namespace NetEvolve.HealthChecks.Tests.Integration.Internals;

/// <summary>
/// Provides predefined sharing strategies for test instances in the HealthChecks integration tests.
/// </summary>
/// <remarks>
/// This class defines constants that can be used with TUnit's <c>ClassDataSource</c> attribute
/// to control how test data instances are shared across test executions.
/// </remarks>
internal static class InstanceSharedType
{
    public const SharedType ActiveMQ = SharedType.PerClass;

    public const SharedType ArangoDb = SharedType.PerClass;

    public const SharedType AWS = SharedType.PerAssembly;

    public const SharedType Azure = SharedType.PerAssembly;

    public const SharedType AzureEventHubs = SharedType.PerAssembly;

    public const SharedType AzureServiceBus = SharedType.PerAssembly;

    public const SharedType ClickHouse = SharedType.PerClass;

    public const SharedType Consul = SharedType.PerClass;

    public const SharedType DB2 = SharedType.PerClass;

    public const SharedType Elasticsearch = SharedType.PerClass;

    public const SharedType EventStoreDb = SharedType.PerClass;

    public const SharedType Firebird = SharedType.PerClass;

    public const SharedType Firestore = SharedType.PerClass;

    public const SharedType InfluxDB = SharedType.PerClass;

    public const SharedType Kafka = SharedType.PerClass;

    public const SharedType Keycloak = SharedType.PerClass;

    public const SharedType Milvus = SharedType.PerClass;

    public const SharedType MongoDb = SharedType.PerClass;

    public const SharedType MySql = SharedType.PerAssembly;

    public const SharedType OpenSearch = SharedType.PerClass;

    public const SharedType Oracle = SharedType.PerClass;

    public const SharedType PostgreSql = SharedType.PerClass;

    public const SharedType Qdrant = SharedType.PerClass;

    public const SharedType RabbitMQ = SharedType.PerClass;

    public const SharedType RavenDb = SharedType.PerClass;

    public const SharedType Redis = SharedType.PerClass;

    public const SharedType Redpanda = SharedType.PerClass;

    public const SharedType SqlServer = SharedType.PerAssembly;
}
