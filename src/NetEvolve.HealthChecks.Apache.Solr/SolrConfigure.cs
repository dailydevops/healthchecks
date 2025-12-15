namespace NetEvolve.HealthChecks.Apache.Solr;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class SolrConfigure : IConfigureNamedOptions<SolrOptions>, IValidateOptions<SolrOptions>
{
    private readonly IConfiguration _configuration;

    public SolrConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(string? name, SolrOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Solr:{name}", options);
    }

    public void Configure(SolrOptions options) => Configure(Options.DefaultName, options);

    public ValidateOptionsResult Validate(string? name, SolrOptions options)
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

        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            return Fail("The BaseUrl cannot be null or whitespace.");
        }

        return Success;
    }
}
