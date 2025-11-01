namespace NetEvolve.HealthChecks.Tests.Integration.LiteDB;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using global::LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.LiteDB;

[TestGroup(nameof(LiteDB))]
public class LiteDBHealthCheckTests : HealthCheckTestBase
{
    private static string CreateTempDatabase()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"litedb_test_{Guid.NewGuid():N}.db");
        using var db = new LiteDatabase(tempPath);
        var col = db.GetCollection<BsonDocument>("test");
        _ = col.Insert(new BsonDocument { ["Id"] = 1 });
        return tempPath;
    }

    private static string CreateTempDatabaseWithCollection(string collectionName)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"litedb_test_{Guid.NewGuid():N}.db");
        using var db = new LiteDatabase(tempPath);
        var col = db.GetCollection<BsonDocument>(collectionName);
        _ = col.Insert(new BsonDocument { ["Id"] = 1 });
        return tempPath;
    }

    [Test]
    public async Task AddLiteDB_UseOptions_Healthy()
    {
        var dbPath = CreateTempDatabaseWithCollection("TestCollection");
        try
        {
            await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddLiteDB(
                        "TestContainerHealthy",
                        options =>
                        {
                            options.ConnectionString = $"filename={dbPath}";
                            options.CollectionName = "TestCollection";
                            options.Timeout = 10000;
                        }
                    );
                },
                HealthStatus.Healthy
            );
        }
        finally
        {
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
        }
    }

    [Test]
    public async Task AddLiteDB_UseOptions_Degraded()
    {
        var dbPath = CreateTempDatabaseWithCollection("TestCollection");
        try
        {
            await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddLiteDB(
                        "TestContainerDegraded",
                        options =>
                        {
                            options.ConnectionString = $"filename={dbPath}";
                            options.CollectionName = "TestCollection";
                            options.Timeout = 0;
                        }
                    );
                },
                HealthStatus.Degraded
            );
        }
        finally
        {
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
        }
    }

    [Test]
    public async Task AddLiteDB_UseOptions_Unhealthy()
    {
        var dbPath = CreateTempDatabase();
        try
        {
            await RunAndVerify(
                healthChecks =>
                {
                    _ = healthChecks.AddLiteDB(
                        "TestContainerUnhealthy",
                        options =>
                        {
                            options.ConnectionString = $"filename={dbPath}";
                            options.CollectionName = "NonExistentCollection";
                            options.Timeout = 10000;
                        }
                    );
                },
                HealthStatus.Unhealthy
            );
        }
        finally
        {
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
        }
    }

    [Test]
    public async Task AddLiteDB_UseConfiguration_Healthy()
    {
        var dbPath = CreateTempDatabaseWithCollection("TestCollection");
        try
        {
            await RunAndVerify(
                healthChecks => healthChecks.AddLiteDB("TestContainerHealthy"),
                HealthStatus.Healthy,
                config =>
                {
                    var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                    {
                        { "HealthChecks:LiteDB:TestContainerHealthy:ConnectionString", $"filename={dbPath}" },
                        { "HealthChecks:LiteDB:TestContainerHealthy:CollectionName", "TestCollection" },
                        { "HealthChecks:LiteDB:TestContainerHealthy:Timeout", "10000" },
                    };
                    _ = config.AddInMemoryCollection(values);
                }
            );
        }
        finally
        {
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
        }
    }

    [Test]
    public async Task AddLiteDB_UseConfiguration_Degraded()
    {
        var dbPath = CreateTempDatabaseWithCollection("TestCollection");
        try
        {
            await RunAndVerify(
                healthChecks => healthChecks.AddLiteDB("TestContainerDegraded"),
                HealthStatus.Degraded,
                config =>
                {
                    var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                    {
                        { "HealthChecks:LiteDB:TestContainerDegraded:ConnectionString", $"filename={dbPath}" },
                        { "HealthChecks:LiteDB:TestContainerDegraded:CollectionName", "TestCollection" },
                        { "HealthChecks:LiteDB:TestContainerDegraded:Timeout", "0" },
                    };
                    _ = config.AddInMemoryCollection(values);
                }
            );
        }
        finally
        {
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
        }
    }

    [Test]
    public async Task AddLiteDB_UseConfiguration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddLiteDB("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:LiteDB:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Test]
    public async Task AddLiteDB_UseConfiguration_TimeoutMinusTwo_ThrowException()
    {
        var dbPath = CreateTempDatabaseWithCollection("TestCollection");
        try
        {
            await RunAndVerify(
                healthChecks => healthChecks.AddLiteDB("TestNoValues"),
                HealthStatus.Unhealthy,
                config =>
                {
                    var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                    {
                        { "HealthChecks:LiteDB:TestNoValues:ConnectionString", $"filename={dbPath}" },
                        { "HealthChecks:LiteDB:TestNoValues:CollectionName", "TestCollection" },
                        { "HealthChecks:LiteDB:TestNoValues:Timeout", "-2" },
                    };
                    _ = config.AddInMemoryCollection(values);
                }
            );
        }
        finally
        {
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
        }
    }

    [Test]
    public async Task AddLiteDB_UseConfiguration_CollectionNameEmpty_ThrowException()
    {
        var dbPath = CreateTempDatabase();
        try
        {
            await RunAndVerify(
                healthChecks => healthChecks.AddLiteDB("TestNoValues"),
                HealthStatus.Unhealthy,
                config =>
                {
                    var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                    {
                        { "HealthChecks:LiteDB:TestNoValues:ConnectionString", $"filename={dbPath}" },
                        { "HealthChecks:LiteDB:TestNoValues:CollectionName", "" },
                    };
                    _ = config.AddInMemoryCollection(values);
                }
            );
        }
        finally
        {
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
        }
    }
}
