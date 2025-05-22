namespace NetEvolve.HealthChecks.Abstractions;

using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NetEvolve.Extensions.Tasks;

/// <summary>
/// Configurable implementation of <see cref="IHealthCheck"/> with focus on <see cref="DbConnection"/> based implementations.
/// </summary>
/// <typeparam name="TConfiguration">Type of Configuration</typeparam>
public abstract class SqlCheckBase<TConfiguration> : ConfigurableHealthCheckBase<TConfiguration>
    where TConfiguration : class, ISqlCheckOptions, IEquatable<TConfiguration>, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlCheckBase{TConfiguration}"/> class.
    /// </summary>
    /// <param name="optionsMonitor">
    /// The <see cref="IOptionsMonitor{TOptions}"/> used to retrieve configuration options.
    /// </param>
    /// <remarks>
    /// This constructor invokes the base class <see cref="ConfigurableHealthCheckBase{TConfiguration}"/>
    /// to initialize the health check with the provided configuration options.
    /// </remarks>
    protected SqlCheckBase(IOptionsMonitor<TConfiguration> optionsMonitor)
        : base(optionsMonitor) { }

    [SuppressMessage(
        "Security",
        "CA2100:Review SQL queries for security vulnerabilities",
        Justification = "As designed."
    )]

    /// <inheritdoc />
    protected sealed override async ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(
        string name,
        HealthStatus failureStatus,
        TConfiguration options,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(options);

        using (var connection = CreateConnection(options.ConnectionString))
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = options.Command;

                var (isHealthy, _) = await command
                    .ExecuteScalarAsync(cancellationToken)
                    .WithTimeoutAsync(options.Timeout, cancellationToken)
                    .ConfigureAwait(false);

                return HealthCheckState(isHealthy, name);
            }
        }
    }

    /// <summary>
    /// Create a new instance of <see cref="DbConnection"/> based on the given <paramref name="connectionString"/>.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    protected abstract DbConnection CreateConnection(string connectionString);
}
