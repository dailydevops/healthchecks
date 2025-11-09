namespace NetEvolve.HealthChecks.EventStoreDb;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class EventStoreDbConfigure
    : IConfigureNamedOptions<EventStoreDbOptions>,
        IValidateOptions<EventStoreDbOptions>
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreDbConfigure"/> class.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to bind configuration values.</param>
    public EventStoreDbConfigure(IConfiguration configuration) => _configuration = configuration;

    /// <inheritdoc />
    public void Configure(string? name, EventStoreDbOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:EventStoreDb:{name}", options);
    }

    /// <inheritdoc />
    public void Configure(EventStoreDbOptions options) => Configure(Options.DefaultName, options);

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, EventStoreDbOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        if (options.Timeout < Timeout.Infinite)
        {
            return Fail("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.");
        }

        return Success;
    }
}
