namespace SourceGenerator.HealthChecks.Generators;

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NetEvolve.CodeBuilder;

[Generator(LanguageNames.CSharp)]
internal sealed class SqlHealthCheckGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var toBeImplemented = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                AttributeName.Namespace + AttributeName.GenerateSqlHealthCheck,
                static (s, _) => IsTargetForGeneration(s),
                static (ctx, _) => GetCandidate(ctx)!
            )
            .Where(c => c is not null);

        context.RegisterSourceOutput(toBeImplemented, static (ctx, candidate) => Generate(ctx, candidate));
    }

    private static void Generate(SourceProductionContext context, Candidate candidate)
    {
        var codeBuilder = CodeGenerator.ConfigurableHealthCheck(
            candidate,
            builder => GenerateSqlCode(builder, candidate)
        );

        context.AddSource($"{candidate.Name}.g.cs", SourceText.From(codeBuilder.ToString(), Encoding.UTF8));
    }

    private static void GenerateSqlCode(CSharpCodeBuilder builder, Candidate candidate)
    {
        _ = builder
            .Append("private ")
            .AppendIf(candidate.AsyncImplementation, "async ")
            .AppendLine("ValueTask<HealthCheckResult> ExecuteHealthCheckAsync(")
            .Intend()
            .AppendLine("string name,")
            .Intend()
            .AppendLine("HealthStatus failureStatus,")
            .Intend()
            .Append(candidate.OptionsTypeName)
            .AppendLine(" options,")
            .Intend()
            .AppendLine("CancellationToken cancellationToken)");
        using (builder.Scope())
        {
            _ = builder.AppendLine("ArgumentNullException.ThrowIfNull(options);").AppendLine();
            if (candidate.AsyncImplementation)
            {
                _ = builder.AppendLine(
                    $"var connection = new global::{candidate.ConnectionTypeNamespace}.{candidate.ConnectionTypeName}(options.ConnectionString);"
                );

                using (builder.ScopeLine("await using (connection.ConfigureAwait(false))"))
                {
                    _ = builder.AppendLine("var command = connection.CreateCommand();");

                    using (builder.ScopeLine("await using (command.ConfigureAwait(false))"))
                    {
                        _ = builder
                            .AppendLine(
                                "#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities"
                            )
                            .AppendLine("command.CommandText = options.Command;")
                            .AppendLine(
                                "#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities"
                            )
                            .AppendLine(
                                "var (isTimelyResponse, _) = await command.ExecuteNonQueryAsync(cancellationToken).WithTimeoutAsync(options.Timeout, cancellationToken).ConfigureAwait(false);"
                            )
                            .AppendLine()
                            .HealthCheckState("isTimelyResponse");
                    }
                }
            }
            else
            {
                _ = builder
                    .AppendLine(
                        $"using var connection = new global::{candidate.ConnectionTypeNamespace}.{candidate.ConnectionTypeName}(options.ConnectionString);"
                    )
                    .AppendLine("connection.Open();")
                    .AppendLine("using var command = connection.CreateCommand();")
                    .AppendLine("#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities")
                    .AppendLine("command.CommandText = options.Command;")
                    .AppendLine("#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities")
                    .AppendLine("var sw = Stopwatch.StartNew();")
                    .AppendLine("_ = command.ExecuteNonQuery();")
                    .AppendLine("var isTimelyResponse = options.Timeout >= sw.Elapsed.TotalMilliseconds;")
                    .HealthCheckStateValueTask("isTimelyResponse");
            }
        }
    }

    private static Candidate? GetCandidate(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetNode is not ClassDeclarationSyntax classDeclaration)
        {
            return null;
        }

        var attributeData = context.Attributes.FirstOrDefault(a =>
            a.AttributeClass!.Name == AttributeName.GenerateSqlHealthCheck
        );
        if (attributeData is null)
        {
            return null;
        }

        return GetCandidateInformation(context.SemanticModel, classDeclaration, attributeData);
    }

    private static Candidate? GetCandidateInformation(
        SemanticModel semanticModel,
        ClassDeclarationSyntax classDeclaration,
        AttributeData attributeData
    )
    {
        if (attributeData.ConstructorArguments[0].Value is not ISymbol connectionType)
        {
            return null;
        }

        if (attributeData.ConstructorArguments[1].Value is not ISymbol optionsType)
        {
            return null;
        }

        if (semanticModel.GetDeclaredSymbol(classDeclaration) is not ISymbol classSymbol)
        {
            return null;
        }

        return new Candidate
        {
            AsyncImplementation = attributeData.ConstructorArguments[2].Value is bool value && value,
            Namespace = classSymbol.ContainingNamespace.ToDisplayString(),
            Name = classDeclaration.Identifier.Text,
            ConnectionTypeName = connectionType.Name,
            ConnectionTypeNamespace = connectionType.ContainingNamespace.ToDisplayString(),
            OptionsTypeName = optionsType.Name,
            OptionsTypeNamespace = optionsType.ContainingNamespace.ToDisplayString(),
        };
    }

    private static bool IsTargetForGeneration(SyntaxNode node) =>
        node is ClassDeclarationSyntax { AttributeLists.Count: > 0 } classNode
        && classNode.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

    private sealed record Candidate : ICandidate
    {
        public bool AsyncImplementation { get; set; }
        public string Namespace { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? ConnectionTypeName { get; set; }
        public string? ConnectionTypeNamespace { get; set; }
        public string OptionsTypeName { get; internal set; } = default!;
        public string? OptionsTypeNamespace { get; internal set; }
    }
}
