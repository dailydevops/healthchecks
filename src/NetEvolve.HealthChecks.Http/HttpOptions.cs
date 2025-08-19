namespace NetEvolve.HealthChecks.Http;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Options for <see cref="HttpHealthCheck"/>
/// </summary>
public sealed record HttpOptions
{
    /// <summary>
    /// The HTTP endpoint URI to check.
    /// </summary>
#pragma warning disable CA1056 // URI-like properties should not be strings
    public string Uri { get; set; } = default!;
#pragma warning restore CA1056 // URI-like properties should not be strings

    /// <summary>
    /// HTTP method to use for the health check. Default is GET.
    /// </summary>
    public string HttpMethod { get; set; } = "GET";

    /// <summary>
    /// Expected HTTP status codes that indicate a healthy response. Default is 200.
    /// </summary>
    public IList<int> ExpectedHttpStatusCodes { get; } = [200];

    /// <summary>
    /// HTTP headers to include in the request.
    /// </summary>
    public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>(StringComparer.Ordinal);

    /// <summary>
    /// Timeout in milliseconds for the HTTP request. Default is 5000ms (5 seconds).
    /// </summary>
    public int Timeout { get; set; } = 5000;

    /// <summary>
    /// Optional content to include in the request body.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Content type for the request body. Default is "application/json".
    /// </summary>
    public string ContentType { get; set; } = "application/json";

    /// <summary>
    /// Indicates whether to follow redirect responses.
    /// </summary>
    public bool AllowAutoRedirect { get; set; } = true;
}
