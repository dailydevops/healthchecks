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
            // AWS
            typeof(AWS.SNS.SimpleNotificationServiceHealthCheck).Assembly,
            // Azure
            typeof(Azure.Blobs.BlobContainerAvailableHealthCheck).Assembly,
            typeof(Azure.Queues.QueueClientAvailableHealthCheck).Assembly,
            typeof(Azure.ServiceBus.ServiceBusQueueHealthCheck).Assembly,
            typeof(Azure.Tables.TableClientAvailableHealthCheck).Assembly,
            // others
            typeof(Abstractions.HealthCheckBase).Assembly,
            typeof(ClickHouse.ClickHouseHealthCheck).Assembly,
            typeof(Dapr.DaprHealthCheck).Assembly,
            typeof(DB2.DB2HealthCheck).Assembly,
            typeof(DuckDB.DuckDBHealthCheck).Assembly,
            typeof(Firebird.FirebirdHealthCheck).Assembly,
            typeof(MongoDb.MongoDbHealthCheck).Assembly,
            typeof(MySql.MySqlHealthCheck).Assembly,
            typeof(MySql.Connector.MySqlHealthCheck).Assembly,
            typeof(Npgsql.NpgsqlHealthCheck).Assembly,
            typeof(Oracle.OracleHealthCheck).Assembly,
            typeof(RabbitMQ.RabbitMQHealthCheck).Assembly,
            typeof(RavenDb.RavenDbHealthCheck).Assembly,
            typeof(Redis.RedisHealthCheck).Assembly,
            typeof(Redpanda.RedpandaHealthCheck).Assembly,
            typeof(Qdrant.QdrantHealthCheck).Assembly,
            typeof(SQLite.SQLiteHealthCheck).Assembly,
            typeof(SQLite.Legacy.SQLiteLegacyHealthCheck).Assembly,
            typeof(SqlServer.SqlServerHealthCheck).Assembly,
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
