﻿namespace NetEvolve.HealthChecks.Redpanda.Tests.Integration;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Testcontainers.Redpanda;
using Xunit;

[ExcludeFromCodeCoverage]
public sealed class RedpandaDatabase : IAsyncLifetime
{
    private readonly RedpandaContainer _database = new RedpandaBuilder().Build();

    public string BootstrapAddress => _database.GetBootstrapAddress();

    public async Task DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
