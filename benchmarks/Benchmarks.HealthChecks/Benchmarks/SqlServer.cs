namespace Benchmarks.HealthChecks.Benchmarks;

using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using global::Benchmarks.HealthChecks.Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NetEvolve.HealthChecks.SqlServer;
using NetEvolve.HealthChecks.SqlServer.Legacy;
using Testcontainers.MsSql;

public class SqlServer
{
    private readonly MsSqlContainer _databaseOne = new MsSqlBuilder().WithLogger(NullLogger.Instance).Build();
    private readonly MsSqlContainer _databaseTwo = new MsSqlBuilder().WithLogger(NullLogger.Instance).Build();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private BenchmarkHealthCheckService _netEvolveHealthCheckExecutor;
    private BenchmarkHealthCheckService _netEvolveLegacyHealthCheckExecutor;
    private BenchmarkHealthCheckService _anotherHealthCheckExecutor;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    [GlobalSetup]
    public async Task GlobalSetupAsync()
    {
        await _databaseOne.StartAsync().ConfigureAwait(false);
        await _databaseTwo.StartAsync().ConfigureAwait(false);

        var configuration = new ConfigurationBuilder().Build();

        var netEvolveServices = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton<BenchmarkHealthCheckService>();
        _ = netEvolveServices
            .AddHealthChecks()
            .AddSqlServer(
                nameof(_databaseOne),
                options => options.ConnectionString = _databaseOne.GetConnectionString()
            )
            .AddSqlServer(
                nameof(_databaseTwo),
                options => options.ConnectionString = _databaseTwo.GetConnectionString()
            );
        var netEvolveServiceProvider = netEvolveServices.BuildServiceProvider();
        _netEvolveHealthCheckExecutor = netEvolveServiceProvider.GetRequiredService<BenchmarkHealthCheckService>();

        var netEvolveLegacyServices = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton<BenchmarkHealthCheckService>();
        _ = netEvolveLegacyServices
            .AddHealthChecks()
            .AddSqlServerLegacy(nameof(_databaseOne), options => _databaseOne.GetConnectionString())
            .AddSqlServerLegacy(nameof(_databaseTwo), options => _databaseTwo.GetConnectionString());
        var netEvolveLegacyServiceProvider = netEvolveLegacyServices.BuildServiceProvider();
        _netEvolveLegacyHealthCheckExecutor =
            netEvolveLegacyServiceProvider.GetRequiredService<BenchmarkHealthCheckService>();

        var aspnetcoreServices = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton<BenchmarkHealthCheckService>();
        _ = aspnetcoreServices
            .AddHealthChecks()
            .AddSqlServer(_databaseOne.GetConnectionString(), name: nameof(_databaseOne))
            .AddSqlServer(_databaseTwo.GetConnectionString(), name: nameof(_databaseTwo));
        var anotherServiceProvider = aspnetcoreServices.BuildServiceProvider();
        _anotherHealthCheckExecutor = anotherServiceProvider.GetRequiredService<BenchmarkHealthCheckService>();
    }

    [GlobalCleanup]
    public async Task GlobalCleanupAsync()
    {
        if (_databaseOne is not null)
        {
            await _databaseOne.StopAsync().ConfigureAwait(false);
            await _databaseOne.DisposeAsync().ConfigureAwait(false);
        }

        if (_databaseTwo is not null)
        {
            await _databaseTwo.StopAsync().ConfigureAwait(false);
            await _databaseTwo.DisposeAsync().ConfigureAwait(false);
        }
    }

    [Benchmark(Baseline = true, Description = "AspNetCore.HealthChecks.SqlServer")]
    public Task BenchmarkAspNetCoreAsync() => _ = _anotherHealthCheckExecutor.CheckHealthAsync();

    [Benchmark(Description = "NetEvolve.HealthChecks.SqlServer")]
    public Task BenchmarkNetEvolveAsync() => _ = _netEvolveHealthCheckExecutor.CheckHealthAsync();

    [Benchmark(Description = "NetEvolve.HealthChecks.SqlServer.Legacy")]
    public Task BenchmarkNetEvolveLegacyAsync() => _ = _netEvolveLegacyHealthCheckExecutor.CheckHealthAsync();
}
