namespace NetEvolve.HealthChecks.GCP.BigQuery;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class BigQueryOptionsConfigure
    : IConfigureNamedOptions<BigQueryOptions>,
        IValidateOptions<BigQueryOptions>
{
    private readonly IConfiguration _configuration;

    public BigQueryOptionsConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, BigQueryOptions options)
    {
        ArgumentNullException.ThrowIfNull(name);

        _configuration.Bind($"HealthChecks:GCP:BigQuery:{name}", options);
    }

    public void Configure(BigQueryOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, BigQueryOptions options)
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
