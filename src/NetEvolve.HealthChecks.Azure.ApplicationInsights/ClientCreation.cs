namespace NetEvolve.HealthChecks.Azure.ApplicationInsights;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;

internal class ClientCreation
{
    private ConcurrentDictionary<string, TelemetryClient>? _telemetryClients;

    internal TelemetryClient GetTelemetryClient<TOptions>(
        string name,
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, IApplicationInsightsOptions
    {
        _telemetryClients ??= new ConcurrentDictionary<string, TelemetryClient>(StringComparer.Ordinal);

        return _telemetryClients.GetOrAdd(name, _ => CreateTelemetryClient(options, serviceProvider));
    }

    internal static TelemetryClient CreateTelemetryClient<TOptions>(
        TOptions options,
        IServiceProvider serviceProvider
    )
        where TOptions : class, IApplicationInsightsOptions
    {
#pragma warning disable IDE0010 // Add missing cases
        switch (options.Mode)
        {
            case ApplicationInsightsClientCreationMode.ServiceProvider:
                return serviceProvider.GetRequiredService<TelemetryClient>();
            case ApplicationInsightsClientCreationMode.ConnectionString:
                var config = TelemetryConfiguration.CreateDefault();
                if (!string.IsNullOrEmpty(options.ConnectionString))
                {
                    config.ConnectionString = options.ConnectionString;
                }

                options.ConfigureConfiguration?.Invoke(config);
                return new TelemetryClient(config);
            case ApplicationInsightsClientCreationMode.InstrumentationKey:
                var configWithKey = TelemetryConfiguration.CreateDefault();
                if (!string.IsNullOrEmpty(options.InstrumentationKey))
                {
                    configWithKey.InstrumentationKey = options.InstrumentationKey;
                }

                options.ConfigureConfiguration?.Invoke(configWithKey);
                return new TelemetryClient(configWithKey);
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
#pragma warning restore IDE0010 // Add missing cases
    }
}