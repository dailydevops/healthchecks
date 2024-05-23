namespace NetEvolve.HealthChecks.Abstractions.Tests.Architecture;

using System;
using System.Threading;
using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using NetEvolve.HealthChecks.Apache.Kafka;
using NetEvolve.HealthChecks.Azure.Blobs;
using NetEvolve.HealthChecks.Azure.Queues;
using NetEvolve.HealthChecks.Azure.Tables;
using NetEvolve.HealthChecks.ClickHouse;
using NetEvolve.HealthChecks.Dapr;
using NetEvolve.HealthChecks.Npgsql;
using NetEvolve.HealthChecks.Oracle;
using NetEvolve.HealthChecks.Redis;
using NetEvolve.HealthChecks.Redpanda;
using NetEvolve.HealthChecks.SqlEdge;
using NetEvolve.HealthChecks.SQLite;
using NetEvolve.HealthChecks.SqlServer;
using NetEvolve.HealthChecks.SqlServer.Legacy;
using MySqlCheck = MySql.MySqlCheck;
using MySqlConnectorCheck = MySql.Connector.MySqlCheck;

internal static class HealthCheckArchitecture
{
    // TIP: load your architecture once at the start to maximize performance of your tests
    private static readonly Lazy<Architecture> _instance = new Lazy<Architecture>(
        () => LoadArchitecture(),
        LazyThreadSafetyMode.PublicationOnly
    );

    public static Architecture Instance => _instance.Value;

    private static Architecture LoadArchitecture()
    {
        System.Reflection.Assembly[] assemblies =
        [
            typeof(KafkaCheck).Assembly,
            typeof(BlobContainerAvailableHealthCheck).Assembly,
            typeof(QueueClientAvailableHealthCheck).Assembly,
            typeof(TableClientAvailableHealthCheck).Assembly,
            typeof(ClickHouseCheck).Assembly,
            typeof(DaprHealthCheck).Assembly,
            typeof(MySqlCheck).Assembly,
            typeof(MySqlConnectorCheck).Assembly,
            typeof(NpgsqlCheck).Assembly,
            typeof(OracleCheck).Assembly,
            typeof(RedisDatabaseHealthCheck).Assembly,
            typeof(RedpandaCheck).Assembly,
            typeof(SqlEdgeCheck).Assembly,
            typeof(SQLiteCheck).Assembly,
            typeof(SqlServerCheck).Assembly,
            typeof(SqlServerLegacyCheck).Assembly
        ];
        var architecture = new ArchLoader()
            .LoadAssembliesRecursively(
                assemblies,
                x =>
                    x.Name.Name.StartsWith(
                        "NetEvolve.HealthChecks",
                        StringComparison.OrdinalIgnoreCase
                    )
                        ? FilterResult.LoadAndContinue
                        : FilterResult.SkipAndContinue
            )
            .Build();
        return architecture;
    }
}
