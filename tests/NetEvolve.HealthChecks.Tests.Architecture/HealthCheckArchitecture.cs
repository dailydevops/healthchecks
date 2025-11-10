namespace NetEvolve.HealthChecks.Tests.Architecture;

using System;
using System.Threading;
using ArchUnitNET.Domain;
using ArchUnitNET.Loader;

internal static class HealthCheckArchitecture
{
    // TIP: load your architecture once at the start to maximize performance of your tests
    private static readonly Lazy<Architecture> _instance = new Lazy<Architecture>(
        LoadArchitecture,
        LazyThreadSafetyMode.PublicationOnly
    );

    public static Architecture Instance => _instance.Value;

    private static Architecture LoadArchitecture()
    {
        System.Reflection.Assembly[] assemblies =
        [
            // Apache
            typeof(Apache.ActiveMq.ActiveMqHealthCheck).Assembly,
            typeof(Apache.Kafka.KafkaHealthCheck).Assembly,
            typeof(Apache.Pulsar.PulsarHealthCheck).Assembly,
            // AWS
            typeof(AWS.EC2.ElasticComputeCloudHealthCheck).Assembly,
            typeof(AWS.DynamoDB.DynamoDbHealthCheck).Assembly,
            typeof(AWS.S3.SimpleStorageServiceHealthCheck).Assembly,
            typeof(AWS.SNS.SimpleNotificationServiceHealthCheck).Assembly,
            typeof(AWS.SQS.SimpleQueueServiceHealthCheck).Assembly,
            // Azure
            typeof(Azure.ApplicationInsights.ApplicationInsightsAvailabilityHealthCheck).Assembly,
            typeof(Azure.Blobs.BlobContainerAvailableHealthCheck).Assembly,
            typeof(Azure.EventHubs.EventHubsHealthCheck).Assembly,
            typeof(Azure.Queues.QueueClientAvailableHealthCheck).Assembly,
            typeof(Azure.ServiceBus.ServiceBusQueueHealthCheck).Assembly,
            typeof(Azure.Tables.TableClientAvailableHealthCheck).Assembly,
            // GCP
            typeof(GCP.Firestore.FirestoreHealthCheck).Assembly,
            // others
            typeof(ArangoDb.ArangoDbHealthCheck).Assembly,
            typeof(ClickHouse.ClickHouseHealthCheck).Assembly,
            typeof(Consul.ConsulHealthCheck).Assembly,
            typeof(Dapr.DaprHealthCheck).Assembly,
            typeof(DB2.DB2HealthCheck).Assembly,
            typeof(DuckDB.DuckDBHealthCheck).Assembly,
            typeof(Elasticsearch.ElasticsearchHealthCheck).Assembly,
            typeof(EventStoreDb.EventStoreDbHealthCheck).Assembly,
            typeof(Firebird.FirebirdHealthCheck).Assembly,
            typeof(Http.HttpHealthCheck).Assembly,
            typeof(InfluxDB.InfluxDBHealthCheck).Assembly,
            typeof(JanusGraph.JanusGraphHealthCheck).Assembly,
            typeof(Keycloak.KeycloakHealthCheck).Assembly,
            typeof(LiteDB.LiteDBHealthCheck).Assembly,
            typeof(MariaDb.MariaDbHealthCheck).Assembly,
            typeof(Milvus.MilvusHealthCheck).Assembly,
            typeof(MongoDb.MongoDbHealthCheck).Assembly,
            typeof(MySql.MySqlHealthCheck).Assembly,
            typeof(MySql.Devart.MySqlDevartHealthCheck).Assembly,
            typeof(MySql.Connector.MySqlHealthCheck).Assembly,
            typeof(Npgsql.NpgsqlHealthCheck).Assembly,
            typeof(OpenSearch.OpenSearchHealthCheck).Assembly,
            typeof(Odbc.OdbcHealthCheck).Assembly,
            typeof(Oracle.OracleHealthCheck).Assembly,
            typeof(RabbitMQ.RabbitMQHealthCheck).Assembly,
            typeof(RavenDb.RavenDbHealthCheck).Assembly,
            typeof(Redis.RedisHealthCheck).Assembly,
            typeof(Redpanda.RedpandaHealthCheck).Assembly,
            typeof(Qdrant.QdrantHealthCheck).Assembly,
            typeof(SQLite.SQLiteHealthCheck).Assembly,
            typeof(SQLite.Devart.SQLiteDevartHealthCheck).Assembly,
            typeof(SQLite.Legacy.SQLiteLegacyHealthCheck).Assembly,
            typeof(SqlServer.SqlServerHealthCheck).Assembly,
            typeof(SqlServer.Devart.SqlServerDevartHealthCheck).Assembly,
            typeof(SqlServer.Legacy.SqlServerLegacyHealthCheck).Assembly,
        ];

        return new ArchLoader()
            .LoadAssembliesRecursively(
                assemblies,
                x =>
                    x.Name.Name.StartsWith("NetEvolve.HealthChecks", StringComparison.OrdinalIgnoreCase)
                        ? FilterResult.LoadAndContinue
                        : FilterResult.SkipAndContinue
            )
            .Build();
    }
}
