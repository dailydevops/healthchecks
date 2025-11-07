namespace SourceGenerator.Attributes;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Marks a health check class as configurable, indicating that it uses a specific options type for configuration.
/// This attribute is used by source generators to create configuration and validation infrastructure for health checks
/// that inherit from ConfigurableHealthCheckBase&lt;TConfiguration&gt;.
/// </summary>
/// <param name="optionsType">
/// The <see cref="Type"/> of the configuration options class used by the health check.
/// This type must be a reference type that implements <see cref="IEquatable{T}"/> and has a parameterless constructor.
/// The options type defines the configurable properties for the health check, such as connection strings, timeouts, or other service-specific settings.
/// </param>
/// <param name="includeWin32Handling">
/// Indicates whether to include Win32 error code handling in the generated health check code.
/// </param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
[ExcludeFromCodeCoverage]
#pragma warning disable CS9113, CA1019 // Parameter is unread.
public sealed class ConfigurableHealthCheckAttribute(Type optionsType, bool includeWin32Handling = false) : Attribute;
#pragma warning restore CS9113, CA1019 // Parameter is unread.
