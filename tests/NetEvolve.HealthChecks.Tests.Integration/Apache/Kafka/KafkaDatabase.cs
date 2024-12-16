namespace NetEvolve.HealthChecks.Tests.Integration.Apache.Kafka;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Testcontainers.Kafka;
using Xunit;

public sealed class KafkaDatabase : IAsyncLifetime
{
    private readonly KafkaContainer _database = new KafkaBuilder().Build();

    public string BootstrapAddress => _database.GetBootstrapAddress();

    public async Task DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
