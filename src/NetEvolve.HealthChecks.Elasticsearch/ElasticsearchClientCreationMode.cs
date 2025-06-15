namespace NetEvolve.HealthChecks.Elasticsearch;

using System;
using Elastic.Clients.Elasticsearch;

/// <summary>
/// Describes the mode used to create the <see cref="ElasticsearchClient"/>.
/// </summary>
public enum ElasticsearchClientCreationMode
{
    /// <summary>
    /// The <see cref="ElasticsearchClient"/> preregistered instance is retrieved from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <remarks>
    /// This is the default mode.
    /// </remarks>
    ServiceProvider = 0,

    /// <summary>
    /// The <see cref="ElasticsearchClient"/> instance is created using the <see cref="ElasticsearchOptions.ConnectionString"/>,
    /// the <see cref="ElasticsearchOptions.Username"/> and the <see cref="ElasticsearchOptions.Password"/>.
    /// </summary>
    Internal = 1,
}
