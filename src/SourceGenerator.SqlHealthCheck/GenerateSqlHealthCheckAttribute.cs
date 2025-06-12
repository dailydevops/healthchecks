namespace SourceGenerator.SqlHealthCheck;

using System;
using System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
[ExcludeFromCodeCoverage]
#pragma warning disable CS9113, CA1019 // Parameter is unread.
internal sealed class GenerateSqlHealthCheckAttribute(Type connectionType, Type optionsType, bool asyncImplementation)
#pragma warning restore CS9113, CA1019 // Parameter is unread.
    : Attribute;
