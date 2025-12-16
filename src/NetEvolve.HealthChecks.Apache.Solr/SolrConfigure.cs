namespace NetEvolve.HealthChecks.Apache.Solr;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

/// <summary>
/// Binds and validates named <see cref="SolrOptions"/> instances from configuration.
/// </summary>
internal sealed class SolrConfigure : IConfigureNamedOptions<SolrOptions>, IValidateOptions<SolrOptions>
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="SolrConfigure"/> class.
    /// </summary>
    /// <param name="configuration">The configuration source that contains Solr settings.</param>
    public SolrConfigure(IConfiguration configuration) => _configuration = configuration;

    /// <summary>
    /// Binds Solr configuration for a given health check name.
    /// </summary>
    /// <param name="name">The health check name.</param>
    /// <param name="options">The options instance to populate.</param>
    public void Configure(string? name, SolrOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _configuration.Bind($"HealthChecks:Solr:{name}", options);
    }

    /// <summary>
    /// Binds Solr configuration using the default health check name.
    /// </summary>
    /// <param name="options">The options instance to populate.</param>
    public void Configure(SolrOptions options) => Configure(Options.DefaultName, options);

    /// <summary>
    /// Validates the configured <see cref="SolrOptions"/> for the specified health check name.
    /// </summary>
    /// <param name="name">The health check name.</param>
    /// <param name="options">The options instance to validate.</param>
    /// <returns>A <see cref="ValidateOptionsResult"/> indicating the validation outcome.</returns>
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

        if (options.CreationMode == ClientCreationMode.Create && string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            return Fail("The BaseUrl cannot be null or whitespace.");
        }

        return Success;
    }
}
