namespace NetEvolve.HealthChecks.Azure.Search;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

internal sealed class SearchServiceAvailableConfigure
    : IConfigureNamedOptions<SearchServiceAvailableOptions>
{
    private readonly IConfiguration _configuration;

    public SearchServiceAvailableConfigure(IConfiguration configuration) =>
        _configuration = configuration;

    public void Configure(string? name, SearchServiceAvailableOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var section = _configuration.GetSection(name);
        if (!section.Exists())
        {
            return;
        }

        section.Bind(options);

        if (options.ServiceUri is null && !string.IsNullOrWhiteSpace(section["ServiceUri"]))
        {
            options.ServiceUri = new Uri(section["ServiceUri"]!, UriKind.Absolute);
        }

        if (options.Mode is null && Enum.TryParse<SearchIndexClientCreationMode>(section["Mode"], out var mode))
        {
            options.Mode = mode;
        }

        if (!string.IsNullOrWhiteSpace(section["Timeout"]) && int.TryParse(section["Timeout"], out var timeout))
        {
            options.Timeout = timeout;
        }
    }

    public void Configure(SearchServiceAvailableOptions options) => Configure(Options.DefaultName, options);
}
