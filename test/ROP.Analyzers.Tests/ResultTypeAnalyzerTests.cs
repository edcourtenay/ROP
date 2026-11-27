using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace EdCourtenay.ROP.Analyzers.Tests;

public class ResultTypeAnalyzerTests
{
    [Fact]
    public async Task Reports_when_success_and_failure_types_are_the_same()
    {
        var diagnostics = await Analyze("public class C { public EdCourtenay.ROP.Result<int, int> M() => throw new System.Exception(); }");

        var diagnostic = diagnostics.Should().ContainSingle().Subject;
        diagnostic.Id.Should().Be(ResultTypeAnalyzer.DiagnosticId);
    }

    [Fact]
    public async Task Does_not_report_when_the_types_differ()
    {
        var diagnostics = await Analyze("public class C { public EdCourtenay.ROP.Result<int, string> M() => throw new System.Exception(); }");

        diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task Does_not_report_another_type_named_Result()
    {
        var diagnostics = await Analyze("public class Result<T1, T2> { } public class C { public Result<int, int> M() => throw new System.Exception(); }");

        diagnostics.Should().BeEmpty();
    }

    private static async Task<ImmutableArray<Diagnostic>> Analyze(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(source);

        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Select(MetadataReference (path) => MetadataReference.CreateFromFile(path))
            .Append(MetadataReference.CreateFromFile(typeof(Result<,>).Assembly.Location));

        var compilation = CSharpCompilation.Create(
            "AnalyzerTests",
            [tree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var withAnalyzers = compilation.WithAnalyzers([new ResultTypeAnalyzer()]);

        return await withAnalyzers.GetAnalyzerDiagnosticsAsync();
    }
}
