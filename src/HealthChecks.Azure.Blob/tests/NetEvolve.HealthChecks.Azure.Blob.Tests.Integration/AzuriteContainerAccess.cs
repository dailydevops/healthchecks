namespace NetEvolve.HealthChecks.Azure.Blob.Tests.Integration;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Testcontainers.Azurite;
using Xunit;

[ExcludeFromCodeCoverage]
public sealed class AzuriteContainerAccess : IAsyncLifetime
{
    private readonly AzuriteContainer _container = new AzuriteBuilder()
        .WithImage("mcr.microsoft.com/azure-storage/azurite:3.26.0")
        .Build();

    public string ConnectionString => _container.GetConnectionString();
    public string ConnectionStringHttps =>
        _container.GetConnectionString().Replace("http", "https", StringComparison.Ordinal);

    public string BlobEndpoint =>
        new UriBuilder(
            Uri.UriSchemeHttp,
            _container.Hostname,
            _container.GetMappedPublicPort(AzuriteBuilder.BlobPort),
            "devstoreaccount1"
        ).ToString();
    public string QueueEndpoint =>
        new UriBuilder(
            Uri.UriSchemeHttp,
            _container.Hostname,
            _container.GetMappedPublicPort(AzuriteBuilder.QueuePort),
            "devstoreaccount1"
        ).ToString();
    public string TableEndpoint =>
        new UriBuilder(
            Uri.UriSchemeHttp,
            _container.Hostname,
            _container.GetMappedPublicPort(AzuriteBuilder.TablePort),
            "devstoreaccount1"
        ).ToString();

    public async Task DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
