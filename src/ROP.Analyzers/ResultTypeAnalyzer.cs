using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace EdCourtenay.ROP.Analyzers;

/// <summary>Reports <c>Result&lt;TSuccess, TFailure&gt;</c> uses where the success and failure types are the same.</summary>
/// <remarks>A result whose two tracks share a type cannot tell a value apart from an error, so the analyzer reports such uses.</remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ResultTypeAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The identifier of the diagnostic reported by this analyzer.</summary>
    public const string DiagnosticId = "ROP0001";

    private const string ResultTypeName = "Result";
    private const string ResultNamespace = "EdCourtenay.ROP";
    private const string ResultMetadataName = "Result`2";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Result success and failure types must differ",
        "'Result<{0}, {1}>' uses the same type for the success and failure tracks, so a value cannot be told apart from an error",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Result<TSuccess, TFailure> requires TSuccess and TFailure to be different types; otherwise the two tracks are indistinguishable.");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeGenericName, SyntaxKind.GenericName);
    }

    private static void AnalyzeGenericName(SyntaxNodeAnalysisContext context)
    {
        var genericName = (GenericNameSyntax)context.Node;

        if (genericName.Identifier.ValueText != ResultTypeName
            || genericName.TypeArgumentList.Arguments.Count != 2)
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(genericName, context.CancellationToken).Symbol is not INamedTypeSymbol resultType
            || resultType.MetadataName != ResultMetadataName
            || resultType.ContainingNamespace?.ToDisplayString() != ResultNamespace
            || resultType.TypeArguments.Length != 2)
        {
            return;
        }

        var success = resultType.TypeArguments[0];
        var failure = resultType.TypeArguments[1];

        if (success.TypeKind == TypeKind.Error
            || failure.TypeKind == TypeKind.Error
            || !SymbolEqualityComparer.Default.Equals(success, failure))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Rule, genericName.GetLocation(), success.ToDisplayString(), failure.ToDisplayString()));
    }
}