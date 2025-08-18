namespace NetEvolve.HealthChecks.Azure.EventHubs;

using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

/// <summary>
/// Represents the base configuration options for Azure Event Hubs health checks.
/// </summary>
public abstract record EventHubsOptionsBase
{
    /// <summary>
    /// Gets or sets the client creation mode. Default is <see cref="ClientCreationMode.ServiceProvider"/>.
    /// </summary>
    public ClientCreationMode? Mode { get; set; }

    /// <summary>
    /// Gets or sets the Azure Event Hubs connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the fully qualified namespace for the Azure Event Hubs resource.
    /// </summary>
    public string? FullyQualifiedNamespace { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds to use when connecting and executing tasks against the Event Hub. Default is 100 milliseconds.
    /// </summary>
    public int Timeout { get; set; } = 100;

    internal static ValidateOptionsResult? InternalValidate<T>(string? name, T options)
        where T : EventHubsOptionsBase
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Fail("The name cannot be null or whitespace.");
        }

        if (options is null)
        {
            return Fail("The option cannot be null.");
        }

        if (options.Timeout < System.Threading.Timeout.Infinite)
        {
            return Fail("The timeout value must be a positive number in milliseconds or -1 for an infinite timeout.");
        }

        if (options.Mode is null)
        {
            return Fail("The client creation mode cannot be null.");
        }
        else if (
            options.Mode is ClientCreationMode.DefaultAzureCredentials
            && string.IsNullOrWhiteSpace(options.FullyQualifiedNamespace)
        )
        {
            return Fail(
                "The fully qualified namespace cannot be null or whitespace when using DefaultAzureCredentials."
            );
        }
        else if (
            options.Mode is ClientCreationMode.ConnectionString
            && string.IsNullOrWhiteSpace(options.ConnectionString)
        )
        {
            return Fail("The connection string cannot be null or whitespace when using ConnectionString.");
        }

        return null;
    }
}