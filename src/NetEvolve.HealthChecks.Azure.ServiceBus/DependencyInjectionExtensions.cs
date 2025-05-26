namespace NetEvolve.HealthChecks.Azure.ServiceBus;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetEvolve.Arguments;
using NetEvolve.HealthChecks.Abstractions;

/// <summary>
/// Extensions methods for <see cref="IHealthChecksBuilder"/> with custom Health Checks.
/// </summary>
public static class DependencyInjectionExtensions
{
    private static readonly string[] _defaultTags = ["messaging", "azure", "servicebus"];

    /// <summary>
    /// Adds a health check for an Azure Service Bus Queue.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">The name of the <see cref="ServiceBusQueueHealthCheck"/>.</param>
    /// <param name="options">An optional action to configure.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is <see langword="null" /> or <c>whitespace</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is already in use.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddAzureServiceBusQueue(
        [NotNull] this IHealthChecksBuilder builder,
        [NotNull] string name,
        Action<ServiceBusQueueOptions>? options = null,
        params string[] tags
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        Argument.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(tags);

        if (!builder.IsServiceTypeRegistered<ServiceBusQueueMarker>())
        {
            _ = builder
                .Services.AddSingleton<ServiceBusQueueMarker>()
                .AddSingleton<ServiceBusQueueHealthCheck>()
                .ConfigureOptions<ServiceBusQueueOptionsConfigure>();
        }

        if (builder.IsNameAlreadyUsed<ServiceBusQueueHealthCheck>(name))
        {
            throw new ArgumentException($"Name `{name}` already in use.", nameof(name), null);
        }

        if (options is not null)
        {
            _ = builder.Services.Configure(name, options);
        }

        return builder.AddCheck<ServiceBusQueueHealthCheck>(
            name,
            HealthStatus.Unhealthy,
            _defaultTags.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    /// <summary>
    /// Adds a health check for an Azure Service Bus Subsciption.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">The name of the <see cref="ServiceBusSubscriptionHealthCheck"/>.</param>
    /// <param name="options">An optional action to configure.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is <see langword="null" /> or <c>whitespace</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is already in use.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddAzureServiceBusSubscription(
        [NotNull] this IHealthChecksBuilder builder,
        [NotNull] string name,
        Action<ServiceBusSubscriptionOptions>? options = null,
        params string[] tags
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        Argument.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(tags);
        if (!builder.IsServiceTypeRegistered<ServiceBusSubscriptionMarker>())
        {
            _ = builder
                .Services.AddSingleton<ServiceBusSubscriptionMarker>()
                .AddSingleton<ServiceBusSubscriptionHealthCheck>()
                .ConfigureOptions<ServiceBusSubscriptionOptionsConfigure>();
        }
        if (builder.IsNameAlreadyUsed<ServiceBusSubscriptionHealthCheck>(name))
        {
            throw new ArgumentException($"Name `{name}` already in use.", nameof(name), null);
        }
        if (options is not null)
        {
            _ = builder.Services.Configure(name, options);
        }
        return builder.AddCheck<ServiceBusSubscriptionHealthCheck>(
            name,
            HealthStatus.Unhealthy,
            _defaultTags.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    /// <summary>
    /// Adds a health check for an Azure Service Bus Topic.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">The name of the <see cref="ServiceBusTopicHealthCheck"/>.</param>
    /// <param name="options">An optional action to configure.</param>
    /// <param name="tags">A list of additional tags that can be used to filter sets of health checks. Optional.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is <see langword="null" /> or <c>whitespace</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="name"/> is already in use.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="tags"/> is <see langword="null" />.</exception>
    public static IHealthChecksBuilder AddAzureServiceBusTopic(
        [NotNull] this IHealthChecksBuilder builder,
        [NotNull] string name,
        Action<ServiceBusTopicOptions>? options = null,
        params string[] tags
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        Argument.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(tags);
        if (!builder.IsServiceTypeRegistered<ServiceBusTopicMarker>())
        {
            _ = builder
                .Services.AddSingleton<ServiceBusTopicMarker>()
                .AddSingleton<ServiceBusTopicHealthCheck>()
                .ConfigureOptions<ServiceBusTopicOptionsConfigure>();
        }
        if (builder.IsNameAlreadyUsed<ServiceBusTopicHealthCheck>(name))
        {
            throw new ArgumentException($"Name `{name}` already in use.", nameof(name), null);
        }
        if (options is not null)
        {
            _ = builder.Services.Configure(name, options);
        }
        return builder.AddCheck<ServiceBusTopicHealthCheck>(
            name,
            HealthStatus.Unhealthy,
            _defaultTags.Union(tags, StringComparer.OrdinalIgnoreCase)
        );
    }

    private sealed partial class ServiceBusQueueMarker;

    private sealed partial class ServiceBusSubscriptionMarker;

    private sealed partial class ServiceBusTopicMarker;
}
