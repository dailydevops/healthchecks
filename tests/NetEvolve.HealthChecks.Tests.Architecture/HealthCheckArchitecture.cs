﻿namespace NetEvolve.HealthChecks.Tests.Architecture;

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
            typeof(Apache.Kafka.KafkaCheck).Assembly,
            typeof(Azure.Blobs.BlobContainerAvailableHealthCheck).Assembly,
            typeof(Azure.Queues.QueueClientAvailableHealthCheck).Assembly,
            typeof(Azure.Tables.TableClientAvailableHealthCheck).Assembly,
            typeof(ClickHouse.ClickHouseCheck).Assembly,
            typeof(Dapr.DaprHealthCheck).Assembly,
            typeof(MySql.MySqlCheck).Assembly,
            typeof(MySql.Connector.MySqlCheck).Assembly,
            typeof(Npgsql.NpgsqlCheck).Assembly,
            typeof(Oracle.OracleCheck).Assembly,
            typeof(Redis.RedisDatabaseHealthCheck).Assembly,
            typeof(Redpanda.RedpandaCheck).Assembly,
            typeof(SQLite.SQLiteCheck).Assembly,
            typeof(SqlServer.SqlServerCheck).Assembly,
            typeof(SqlServer.Legacy.SqlServerLegacyCheck).Assembly,
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
