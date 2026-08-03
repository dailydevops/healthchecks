namespace NetEvolve.HealthChecks.Seq;

using System;
using System.Collections.Concurrent;
using global::Seq.Api;
using Microsoft.Extensions.DependencyInjection;

internal sealed class SeqClientProvider
{
    private ConcurrentDictionary<string, SeqConnection>? _seqConnections;

    internal SeqConnection GetClient(string name, SeqOptions options, IServiceProvider serviceProvider)
    {
        if (options.Mode == SeqClientCreationMode.ServiceProvider)
        {
            return string.IsNullOrWhiteSpace(options.KeyedService)
                ? serviceProvider.GetRequiredService<SeqConnection>()
                : serviceProvider.GetRequiredKeyedService<SeqConnection>(options.KeyedService);
        }

        _seqConnections ??= new ConcurrentDictionary<string, SeqConnection>(StringComparer.OrdinalIgnoreCase);

        return _seqConnections.GetOrAdd(name, _ => CreateClient(options));
    }

    internal static SeqConnection CreateClient(SeqOptions options) =>
        options.Mode switch
        {
            SeqClientCreationMode.ServerUrl => CreateClientWithServerUrl(options),
            _ => throw new ArgumentOutOfRangeException(nameof(options), options.Mode, "The mode is not supported."),
        };

    private static SeqConnection CreateClientWithServerUrl(SeqOptions options)
    {
        ArgumentNullException.ThrowIfNull(options.ServerUrl);

        return new SeqConnection(options.ServerUrl.ToString(), options.ApiKey);
    }
}
