namespace NetEvolve.HealthChecks.RavenDb;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class RavenDbConfigure : IConfigureNamedOptions<RavenDbOptions>, IValidateOptions<RavenDbOptions>
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbConfigure"/> class.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to bind configuration values.</param>
    public RavenDbConfigure(IConfiguration configuration) => _configuration = configuration;

    /// <inheritdoc />
    public void Configure(string? name, RavenDbOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:RavenDb:{name}", options);
    }

    /// <inheritdoc />
    public void Configure(RavenDbOptions options) => Configure(Options.DefaultName, options);

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, RavenDbOptions options)
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
            return Fail("The timeout cannot be less than infinite (-1).");
        }

        return Success;
    }
}
