namespace SourceGenerator.Attributes;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Marks a partial class to generate SQL health check implementation code via source generation.
/// This attribute triggers code generation that creates an <c>ExecuteHealthCheckAsync</c> method
/// which opens a database connection, executes a command, and returns a health check result.
/// </summary>
/// <param name="connectionType">
/// The database connection type to be used in the generated health check implementation.
/// This type must represent a database connection class (e.g., <c>SqlConnection</c>, <c>NpgsqlConnection</c>)
/// that has a constructor accepting a connection string and supports <c>Open</c>/<c>OpenAsync</c> methods
/// and <c>CreateCommand</c> functionality.
/// </param>
/// <param name="optionsType">
/// The options type that contains the health check configuration.
/// This type must provide at minimum a <c>ConnectionString</c> property, a <c>Command</c> property for the
/// SQL command to execute, and a <c>Timeout</c> property to validate response time. The options instance
/// is passed to the generated <c>ExecuteHealthCheckAsync</c> method.
/// </param>
/// <param name="asyncImplementation">
/// A value indicating whether the generated implementation should use asynchronous database operations.
/// When <c>true</c>, the generated code uses <c>async</c>/<c>await</c> patterns with methods like
/// <c>OpenAsync</c>, <c>ExecuteNonQueryAsync</c>, and includes <c>await using</c> for resource disposal.
/// It also integrates timeout handling via <c>WithTimeoutAsync</c>.
/// When <c>false</c>, the generated code uses synchronous operations (<c>Open</c>, <c>ExecuteNonQuery</c>)
/// with traditional <c>using</c> statements and <c>Stopwatch</c>-based timeout validation.
/// Choose <c>true</c> for modern async database providers and better scalability, or <c>false</c>
/// for legacy providers or environments where async operations are not supported or necessary.
/// </param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
[ExcludeFromCodeCoverage]
#pragma warning disable CS9113, CA1019 // Parameter is unread.
public sealed class GenerateSqlHealthCheckAttribute(Type connectionType, Type optionsType, bool asyncImplementation)
#pragma warning restore CS9113, CA1019 // Parameter is unread.
    : Attribute;
