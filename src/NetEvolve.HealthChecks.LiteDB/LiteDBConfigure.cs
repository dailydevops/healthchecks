namespace NetEvolve.HealthChecks.LiteDB;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class LiteDBConfigure(IConfiguration configuration)
    : IConfigureNamedOptions<LiteDBOptions>,
        IValidateOptions<LiteDBOptions>
{
    /// <inheritdoc />
    public void Configure(string? name, LiteDBOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        configuration.Bind($"HealthChecks:LiteDB:{name}", options);
    }

    /// <inheritdoc />
    public void Configure(LiteDBOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, LiteDBOptions options)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The options cannot be null.");
        }

        if (options.Timeout < Timeout.Infinite)
        {
            return Fail("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.");
        }

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return Fail("The connection string cannot be null or whitespace.");
        }

        if (string.IsNullOrWhiteSpace(options.CollectionName))
        {
            return Fail("The collection name cannot be null or whitespace.");
        }

        return Success;
    }
}
