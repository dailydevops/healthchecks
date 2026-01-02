namespace NetEvolve.HealthChecks.Tests.Integration.Apache.Kafka;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Kafka;

public sealed class KafkaContainer : IAsyncInitializer, IAsyncDisposable
{
    private readonly Testcontainers.Kafka.KafkaContainer _database = new KafkaBuilder(
        /*dockerimage*/"confluentinc/cp-kafka:7.5.12"
    )
        .WithLogger(NullLogger.Instance)
        .Build();

    public string BootstrapAddress => _database.GetBootstrapAddress();

    public async ValueTask DisposeAsync() => await _database.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _database.StartAsync().ConfigureAwait(false);
}
