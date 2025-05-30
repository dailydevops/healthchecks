namespace NetEvolve.HealthChecks.Tests.Integration.Apache.Kafka;

using System.Threading.Tasks;
using Testcontainers.Kafka;

public sealed class KafkaContainer : IAsyncInitializer, IAsyncDisposable
{
    private readonly Testcontainers.Kafka.KafkaContainer _database = new KafkaBuilder().Build();

    public string BootstrapAddress => _database.GetBootstrapAddress();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
