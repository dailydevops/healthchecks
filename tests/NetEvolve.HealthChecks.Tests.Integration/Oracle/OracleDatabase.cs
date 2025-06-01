﻿namespace NetEvolve.HealthChecks.Tests.Integration.Oracle;

using System.Threading.Tasks;
using Testcontainers.Oracle;

public sealed class OracleDatabase : IAsyncInitializer, IAsyncDisposable
{
    private readonly OracleContainer _database = new OracleBuilder().Build();

    public string GetConnectionString() => _database.GetConnectionString();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
