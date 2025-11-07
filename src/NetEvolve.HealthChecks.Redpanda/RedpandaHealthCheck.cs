namespace NetEvolve.HealthChecks.Redpanda;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Extensions.Tasks;
using SourceGenerator.Attributes;

[ConfigurableHealthCheck(typeof(RedpandaOptions))]
internal sealed partial class RedpandaHealthCheck : IDisposable
{
    private ConcurrentDictionary<string, IProducer<string, string>>? _producers;
    private bool _disposedValue;

    private static readonly Message<string, string> _message = new Message<string, string>()
    {
        Key = "HealthCheck",
        Value = "HealthCheck",
    };

    private async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        RedpandaOptions options,
        CancellationToken cancellationToken
    )
    {
        var producer = GetProducer(name, options, _serviceProvider);

        var (isHealthy, result) = await producer
            .ProduceAsync(options.Topic, _message, cancellationToken)
            .WithTimeoutAsync(options.Timeout, cancellationToken)
            .ConfigureAwait(false);

        if (options.Configuration?.EnableDeliveryReports != false && result.Status == PersistenceStatus.NotPersisted)
        {
            return HealthCheckUnhealthy(failureStatus, name, "Message Not Persisted.");
        }

        return HealthCheckState(isHealthy, name);
    }

    private IProducer<string, string> GetProducer(
        string name,
        RedpandaOptions options,
        IServiceProvider serviceProvider
    )
    {
        if (options.Mode == ProducerHandleMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<IProducer<string, string>>();
        }

        _producers ??= new ConcurrentDictionary<string, IProducer<string, string>>(StringComparer.OrdinalIgnoreCase);

        return _producers.GetOrAdd(name, _ => CreateProducer(options));
    }

    private static IProducer<string, string> CreateProducer(RedpandaOptions options) =>
        new ProducerBuilder<string, string>(options.Configuration).Build();

    [SuppressMessage(
        "Blocker Code Smell",
        "S2953:Methods named \"Dispose\" should implement \"IDisposable.Dispose\"",
        Justification = "As designed."
    )]
    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing && _producers is not null)
            {
                _ = Parallel.ForEach(_producers.Values, producer => producer.Dispose());
                _producers.Clear();
            }
            _disposedValue = true;
        }
    }

    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
