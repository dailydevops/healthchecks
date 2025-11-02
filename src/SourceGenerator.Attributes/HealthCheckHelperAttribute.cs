namespace SourceGenerator.Attributes;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Marks a sealed partial class as a health check registration marker for dependency injection.
/// This attribute enables source generation of helper code for health check service registration patterns,
/// preventing duplicate registrations and ensuring proper lifecycle management.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
[ExcludeFromCodeCoverage]
public sealed class HealthCheckHelperAttribute : Attribute;
