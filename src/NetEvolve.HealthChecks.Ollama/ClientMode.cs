namespace NetEvolve.HealthChecks.Ollama;

/// <summary>
/// Defines the mode for resolving the Ollama client.
/// </summary>
public enum ClientMode
{
    /// <summary>
    /// Resolve the Ollama client from the service provider.
    /// </summary>
    ServiceProvider = 0,

    /// <summary>
    /// Create the Ollama client using a service URL.
    /// </summary>
    ServiceUrl,
}
