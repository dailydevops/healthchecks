namespace NetEvolve.HealthChecks.Ollama;

using System;

/// <summary>
/// Represents the options for an Ollama instance.
/// </summary>
public sealed record OllamaOptions
{
    /// <summary>
    /// Gets or sets the client mode for connecting to the Ollama instance.
    /// </summary>
    public ClientMode ClientMode { get; set; }

    /// <summary>
    /// Gets or sets the keyed service name for retrieving the Ollama client from a service provider.
    /// </summary>
    public string? KeyedService { get; set; }

    /// <summary>
    /// Gets or sets the URI for the Ollama instance.
    /// </summary>
    public Uri? Uri { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for the Ollama instance operations. Default value is 5000.
    /// </summary>
    public int Timeout { get; set; } = 5000;
}
