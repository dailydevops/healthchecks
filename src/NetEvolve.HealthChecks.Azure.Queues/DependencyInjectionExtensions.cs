﻿namespace NetEvolve.HealthChecks.Azure.Queues;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Extensions methods for <see cref="IHealthChecksBuilder"/> with custom Health Checks.
/// </summary>
public static class DependencyInjectionExtensions
{
    private static readonly string[] _defaultTags = ["storage", "azure", "queue"];

    /// <summary>
    /// Adds a health check for the Azure Queue Storage, to check the availability of a named queue.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">The name of the <see cref="QueueClientAvailableHealthCheck"/>.</param>
    /// <param name="options">An optional action to configure.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is <see langword="null" /> or <c>whitespace</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is already in use.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddQueueClientAvailability(
        [NotNull] this IHealthChecksBuilder builder,
        [NotNull] string name,
        Action<QueueClientAvailableOptions>? options = null,
        params string[] tags
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(tags);

        if (!builder.IsServiceTypeRegistered<AzureQueueClientCheckMarker>())
        {
            _ = builder
                .Services.AddSingleton<AzureQueueClientCheckMarker>()
                .AddSingleton<QueueClientAvailableHealthCheck>()
                .ConfigureOptions<QueueClientAvailableConfigure>();

            builder.Services.TryAddSingleton<ClientCreation>();
        }

        builder.ThrowIfNameIsAlreadyUsed<QueueClientAvailableHealthCheck>(name);

        if (options is not null)
        {
            _ = builder.Services.Configure(name, options);
        }

        return builder.AddCheck<QueueClientAvailableHealthCheck>(
            name,
            HealthStatus.Unhealthy,
            _defaultTags.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    /// <summary>
    /// Adds a health check for the Azure Queue Storage, to check the availability of the queue service.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">The name of the <see cref="QueueServiceAvailableHealthCheck"/>.</param>
    /// <param name="options">An optional action to configure.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is <see langword="null" /> or <c>whitespace</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is already in use.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddQueueServiceAvailability(
        [NotNull] this IHealthChecksBuilder builder,
        [NotNull] string name,
        Action<QueueServiceAvailableOptions>? options = null,
        params string[] tags
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(tags);

        if (!builder.IsServiceTypeRegistered<AzureQueueServiceCheckMarker>())
        {
            _ = builder
                .Services.AddSingleton<AzureQueueServiceCheckMarker>()
                .AddSingleton<QueueServiceAvailableHealthCheck>()
                .ConfigureOptions<QueueServiceAvailableConfigure>();

            builder.Services.TryAddSingleton<ClientCreation>();
        }

        builder.ThrowIfNameIsAlreadyUsed<QueueServiceAvailableHealthCheck>(name);

        if (options is not null)
        {
            _ = builder.Services.Configure(name, options);
        }

        return builder.AddCheck<QueueServiceAvailableHealthCheck>(
            name,
            HealthStatus.Unhealthy,
            _defaultTags.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    private sealed partial class AzureQueueClientCheckMarker;

    private sealed partial class AzureQueueServiceCheckMarker;
}
