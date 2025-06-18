namespace Benchmarks.HealthChecks.Internals;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

internal static class BenchmarkServiceFactory
{
    private static readonly IConfiguration _emptyConfiguration = new ConfigurationBuilder().Build();

    public static BenchmarkHealthCheckService Create(Action<IHealthChecksBuilder> builder)
    {
        var serviceCollection = new ServiceCollection()
            .AddSingleton(_emptyConfiguration)
            .AddSingleton<BenchmarkHealthCheckService>();

        var healthCheckBuilder = serviceCollection.AddHealthChecks();
        builder.Invoke(healthCheckBuilder);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        return serviceProvider.GetRequiredService<BenchmarkHealthCheckService>();
    }
}
