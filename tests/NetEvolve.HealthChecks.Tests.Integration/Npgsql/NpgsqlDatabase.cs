﻿namespace NetEvolve.HealthChecks.Tests.Integration.Npgsql;

using System.Threading.Tasks;
using Testcontainers.PostgreSql;

public sealed class NpgsqlDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder().Build();

    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
