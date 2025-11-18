namespace NetEvolve.HealthChecks.Azure.Search;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

internal sealed class SearchIndexAvailableConfigure
    : IConfigureNamedOptions<SearchIndexAvailableOptions>
{
    private readonly IConfiguration _configuration;

    public SearchIndexAvailableConfigure(IConfiguration configuration) =>
        _configuration = configuration;

    public void Configure(string? name, SearchIndexAvailableOptions options)
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

    public void Configure(SearchIndexAvailableOptions options) => Configure(Options.DefaultName, options);
}
