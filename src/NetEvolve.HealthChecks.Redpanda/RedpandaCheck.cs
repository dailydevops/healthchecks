namespace NetEvolve.HealthChecks.Redpanda;

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;
using NetEvolve.HealthChecks.Abstractions;

internal sealed class RedpandaCheck : ConfigurableHealthCheckBase<RedpandaOptions>
{
    private readonly IServiceProvider _serviceProvider;
    private static ConcurrentDictionary<string, IProducer<string, string>>? _producers;

    private static readonly Message<string, string> _message = new Message<string, string>()
    {
        Key = "HealthCheck",
        Value = "HealthCheck",
    };

    public RedpandaCheck(IServiceProvider serviceProvider, IOptionsMonitor<RedpandaOptions> optionsMonitor)
        : base(optionsMonitor) => _serviceProvider = serviceProvider;

    protected override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
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

        if (result.Status == PersistenceStatus.NotPersisted)
        {
            return new HealthCheckResult(failureStatus, $"{name}: Unhealthy");
        }

        return HealthCheckState(isHealthy, name);
    }

    private static IProducer<string, string> GetProducer(
        string name,
        RedpandaOptions options,
        IServiceProvider serviceProvider
    )
    {
        if (options.Mode == ProducerHandleMode.ServiceProvider)
        {
            return serviceProvider.GetRequiredService<IProducer<string, string>>();
        }

        if (_producers is null)
        {
            _producers = new ConcurrentDictionary<string, IProducer<string, string>>(StringComparer.OrdinalIgnoreCase);
        }

        return _producers.GetOrAdd(name, _ => CreateProducer(options));
    }

    private static IProducer<string, string> CreateProducer(RedpandaOptions options) =>
        new ProducerBuilder<string, string>(options.Configuration).Build();
}
