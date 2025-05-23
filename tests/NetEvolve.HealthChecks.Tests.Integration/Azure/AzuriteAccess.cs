﻿namespace NetEvolve.HealthChecks.Tests.Integration.Azure;

using System;
using System.Threading.Tasks;
using Testcontainers.Azurite;
using Xunit;

public sealed class AzuriteAccess : IAsyncLifetime, IAsyncDisposable
{
    public const string AccountName = "devstoreaccount1";
    public const string AccountKey =
        "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

    private readonly AzuriteContainer _container = new AzuriteBuilder().WithCommand("--skipApiVersionCheck").Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);

    async ValueTask IAsyncDisposable.DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);
}
