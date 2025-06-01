namespace NetEvolve.HealthChecks.Tests.Integration.MongoDb;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;
using NetEvolve.Extensions.XUnit;
using NetEvolve.HealthChecks.MongoDb;
using Xunit;

[TestGroup(nameof(MongoDb))]
public class MongoDbHealthCheckTests : HealthCheckTestBase, IClassFixture<MongoDbDatabase>
{
    private readonly MongoDbDatabase _database;

    public MongoDbHealthCheckTests(MongoDbDatabase database) => _database = database;

    [Fact]
    public async Task AddMongoDb_UseOptions_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMongoDb(
                    "TestContainerHealthy",
                    options => options.ConnectionString = _database.ConnectionString
                );
            },
            HealthStatus.Healthy
        );

    [Fact]
    public async Task AddMongoDb_UseOptionsDoubleRegistered_ShouldReturnHealthy() =>
        _ = await Assert.ThrowsAsync<ArgumentException>(
            "name",
            async () =>
            {
                await RunAndVerify(
                    healthChecks => healthChecks.AddMongoDb("TestContainerHealthy").AddMongoDb("TestContainerHealthy"),
                    HealthStatus.Healthy
                );
            }
        );

    [Fact]
    public async Task AddMongoDb_UseOptions_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMongoDb(
                    "TestContainerDegraded",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.CommandAsync = async (MongoClient client, CancellationToken cancellationToken) =>
                        {
                            await Task.Delay(1000, cancellationToken);

                            var database = client.GetDatabase("admin");
                            var command = new BsonDocument("ping", 1);

                            return await database
                                .RunCommandAsync<BsonDocument>(command, null, cancellationToken)
                                .ConfigureAwait(false);
                        };
                        options.Timeout = 0;
                    }
                );
            },
            HealthStatus.Degraded
        );

    [Fact]
    public async Task AddMongoDb_UseOptions_ShouldReturnUnhealthy() =>
        await RunAndVerify(
            healthChecks =>
            {
                _ = healthChecks.AddMongoDb(
                    "TestContainerUnhealthy",
                    options =>
                    {
                        options.ConnectionString = _database.ConnectionString;
                        options.CommandAsync = async (MongoClient client, CancellationToken cancellationToken) =>
                        {
                            var database = client.GetDatabase("admin");
                            var invalidCommand = new BsonDocument("invalid", 1);

                            return await database
                                .RunCommandAsync<BsonDocument>(invalidCommand, null, cancellationToken)
                                .ConfigureAwait(false);
                        };
                    }
                );
            },
            HealthStatus.Unhealthy
        );

    [Fact]
    public async Task AddMongoDb_UseConfiguration_ShouldReturnHealthy() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMongoDb("TestContainerHealthy"),
            HealthStatus.Healthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MongoDb:TestContainerHealthy:ConnectionString", _database.ConnectionString },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddMongoDb_UseConfiguration_ShouldReturnDegraded() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMongoDb("TestContainerDegraded"),
            HealthStatus.Degraded,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MongoDb:TestContainerDegraded:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:MongoDb:TestContainerDegraded:Timeout", "0" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddMongoDb_UseConfigration_ConnectionStringEmpty_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMongoDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MongoDb:TestNoValues:ConnectionString", "" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );

    [Fact]
    public async Task AddMongoDb_UseConfigration_TimeoutMinusTwo_ThrowException() =>
        await RunAndVerify(
            healthChecks => healthChecks.AddMongoDb("TestNoValues"),
            HealthStatus.Unhealthy,
            config =>
            {
                var values = new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    { "HealthChecks:MongoDb:TestNoValues:ConnectionString", _database.ConnectionString },
                    { "HealthChecks:MongoDb:TestNoValues:Timeout", "-2" },
                };
                _ = config.AddInMemoryCollection(values);
            }
        );
}
