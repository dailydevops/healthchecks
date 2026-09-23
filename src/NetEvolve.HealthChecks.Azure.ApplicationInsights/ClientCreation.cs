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

        if (options.Mode == ApplicationInsightsClientCreationMode.ServiceProvider)
        {
            // If the mode is ServiceProvider, we should not cache the TelemetryClient.
            // Instead, we will retrieve it directly from the service provider each time.
            return serviceProvider.GetRequiredService<TelemetryClient>();
        }

        return _telemetryClients.GetOrAdd(name, _ => CreateTelemetryClient(options));
    }

    internal static TelemetryClient CreateTelemetryClient<TOptions>(TOptions options)
        where TOptions : class, IApplicationInsightsOptions
    {
#pragma warning disable IDE0010 // Add missing cases
        switch (options.Mode)
        {
            case ApplicationInsightsClientCreationMode.ConnectionString:
                // TelemetryConfiguration.CreateDefault() returns a shared, already-built instance in this SDK
                // version, whose settings can no longer be changed. A fresh instance must be created instead.
#pragma warning disable CA2000 // Dispose objects before losing scope
                var config = new TelemetryConfiguration();
#pragma warning restore CA2000 // Dispose objects before losing scope
                if (!string.IsNullOrEmpty(options.ConnectionString))
                {
                    config.ConnectionString = options.ConnectionString;
                }

                options.ConfigureConfiguration?.Invoke(config);
                return new TelemetryClient(config);
            case ApplicationInsightsClientCreationMode.InstrumentationKey:
#pragma warning disable CA2000 // Dispose objects before losing scope
                var configWithKey = new TelemetryConfiguration();
#pragma warning restore CA2000 // Dispose objects before losing scope
                if (!string.IsNullOrEmpty(options.InstrumentationKey))
                {
                    configWithKey.ConnectionString = $"InstrumentationKey={options.InstrumentationKey}";
                }

                options.ConfigureConfiguration?.Invoke(configWithKey);
                return new TelemetryClient(configWithKey);
            default:
                throw new UnreachableException($"Invalid client creation mode `{options.Mode}`.");
        }
#pragma warning restore IDE0010 // Add missing cases
    }
}
