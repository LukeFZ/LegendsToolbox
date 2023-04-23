using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BadgerSerialization.Generator.Serialization;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SerializeAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
        => ImmutableArray.Create(SerializeDiagnosticDescriptors.ClassMustBePartial);

    public override void Initialize(AnalysisContext context)
    {
       context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
       context.EnableConcurrentExecution();

       context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext ctx)
    {
        if (!SerializeGenerator.IsBadgerObject(ctx.Symbol))
            return;

        var type = (INamedTypeSymbol) ctx.Symbol;

        foreach (var declaringSyntaxReference in type.DeclaringSyntaxReferences)
        {
            if (declaringSyntaxReference.GetSyntax() is not ClassDeclarationSyntax classDeclaration
                || SerializeGenerator.IsPartial(classDeclaration))
                continue;

            var error = Diagnostic.Create(
                SerializeDiagnosticDescriptors.ClassMustBePartial,
                classDeclaration.Identifier.GetLocation(),
                type.Name
            );

            ctx.ReportDiagnostic(error);
        }
    }
}