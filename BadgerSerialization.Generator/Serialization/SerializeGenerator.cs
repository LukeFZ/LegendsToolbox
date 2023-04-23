using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using BadgerSerialization.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BadgerSerialization.Generator.Serialization;

[Generator]
public class SerializeGenerator : IIncrementalGenerator
{
    private const string BadgerObjectAttribute = "BadgerSerialization.Core.Attributes.BadgerObjectAttribute";
    private const string BadgerPropertyAttribute = "BadgerSerialization.Core.Attributes.BadgerPropertyAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ClassDeclarationSyntax> ivpClasses = context.SyntaxProvider.CreateSyntaxProvider(
            static (s, _) => IsSyntaxTarget(s),
            static (ctx, _) => HasBadgerAttribute(ctx))
            .Where(static m => m is not null)!;

        var ivpCompilation = context.CompilationProvider.Combine(ivpClasses.Collect());

        context.RegisterSourceOutput(ivpCompilation,
            static (spc, source) => Execute(source.Left, source.Right, spc));
    }

    public static bool IsBadgerObject(ISymbol type)
        => type.GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == BadgerObjectAttribute);

    public static bool IsPartial(ClassDeclarationSyntax classDecl)
        => classDecl.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword));

    private static bool IsSyntaxTarget(SyntaxNode syntaxNode)
        => syntaxNode is ClassDeclarationSyntax {AttributeLists.Count: > 0} classNode 
           && IsPartial(classNode);

    private static ClassDeclarationSyntax? HasBadgerAttribute(GeneratorSyntaxContext ctx)
    {
        var classSyntax = (ClassDeclarationSyntax) ctx.Node;

        var classDecl = ctx.SemanticModel.GetDeclaredSymbol(classSyntax);
        return classDecl == null || !IsBadgerObject(classDecl) ? null : classSyntax;
    }

    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes,
        SourceProductionContext ctx)
    {
        if (classes.IsDefaultOrEmpty)
            return;

        var distinctClasses = classes.Distinct();
        var classSerializationInfos = GetSerializationInfo(compilation, distinctClasses, ctx);

        ctx.CancellationToken.ThrowIfCancellationRequested();

        var serializationSources = GenerateSerializationSource(classSerializationInfos, ctx.CancellationToken);
        var deserializationSources = GenerateDeserializationSource(classSerializationInfos, ctx.CancellationToken);
        ctx.AddSource("BadgerSerializers.g.cs", serializationSources);
        ctx.AddSource("BadgerDeserializers.g.cs", deserializationSources);
    }

    private static List<ClassSerializationInfo> GetSerializationInfo(Compilation compilation,
        IEnumerable<ClassDeclarationSyntax> classes, SourceProductionContext ctx)
    {
        var classInfos = new List<ClassSerializationInfo>();
        var propertyAttributeType = compilation.GetTypeByMetadataName(BadgerPropertyAttribute);
        if (propertyAttributeType == null)
            return classInfos;

        foreach (var classDeclaration in classes)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();

            var name = classDeclaration.Identifier.ValueText;
            var ns = classDeclaration.GetNamespace();
            var propertyInfos = new List<PropertySerializationInfo>();

            foreach (var member in classDeclaration.Members)
            {
                if (member is not PropertyDeclarationSyntax property || property.AttributeLists.Count == 0)
                    continue;

                var semanticModel = compilation.GetSemanticModel(property.SyntaxTree);
                var propertySymbol = semanticModel.GetDeclaredSymbol(property);
                if (propertySymbol == null)
                    continue;

                var propertyName = property.Identifier.ValueText;

                foreach (var attributeData in propertySymbol.GetAttributes())
                {
                    if (!propertyAttributeType.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default))
                        continue;

                    var propertyBadgerName = (string) attributeData.ConstructorArguments[0].Value!;
                    var propertyType = (BadgerObjectType) attributeData.ConstructorArguments[1].Value!;
                    var propertyInfo = new PropertySerializationInfo(propertyName, propertyBadgerName, propertyType);

                    if (!propertyInfo.Type.IsSimpleType())
                        propertyInfo.ComplexTypeName = property.Type.ToString();

                    propertyInfos.Add(propertyInfo);
                    break;
                }
            }

            classInfos.Add(new ClassSerializationInfo(name, ns, propertyInfos));
        }

        return classInfos;
    }

    private static string GenerateSerializationSource(List<ClassSerializationInfo> classSerializationInfos,
        CancellationToken cancellationToken)
    {
        var generator = new CodeGenerator();
        generator.AddLine("#nullable restore");
        generator.AddLine();
        generator.AddLine("using BadgerSerialization;");
        generator.AddLine("using BadgerSerialization.Core;");
        generator.AddLine("using BadgerSerialization.Types;");
        generator.AddLine("using System.Collections.Generic;");

        foreach (var classSerializationInfo in classSerializationInfos)
        {
            cancellationToken.ThrowIfCancellationRequested();

            generator.EnterScope($"namespace {classSerializationInfo.Namespace}");
            generator.EnterScope($"public partial class {classSerializationInfo.Name}");

            generator.EnterScope("public Dictionary<string, BadgerObject> Serialize()");

            generator.AddLine("var dict = new Dictionary<string, BadgerObject>();");
            foreach (var propertyInfo in classSerializationInfo.Properties)
            {
                var propertyName = propertyInfo.PropertyName;
                var badgerName = propertyInfo.BadgerName;
                var type = propertyInfo.Type;

                if (type.IsSimpleType())
                    generator.AddLine($"dict[\"{badgerName}\"] = new Badger{type}(\"{badgerName}\", {propertyName});");
                else
                {
                    switch (type)
                    {
                        case BadgerObjectType.Compound:
                            generator.AddLine($"dict[\"{badgerName}\"] = new Badger{type}(\"{badgerName}\", {propertyName}.Serialize());");
                            break;
                        case {} when type.IsList():
                            generator.AddLine($"dict[\"{badgerName}\"] = new Badger{type}(\"{badgerName}\", {propertyName}.Select(x => x.Serialize()).ToList());");
                            break;
                        case BadgerObjectType.Dictionary:
                            generator.AddLine($"dict[\"{badgerName}\"] = new Badger{type}(\"{badgerName}\", {propertyName}.Select(x => x.Value.Serialize()).ToList());");
                            break;
                    }
                }
            }
            generator.AddLine("return dict;");

            generator.LeaveScope();

            generator.AddLine("public BadgerCompound SerializeToCompound() => new BadgerCompound(\"\", Serialize());");

            generator.LeaveScope();
            generator.LeaveScope();
        }

        return generator.ToString();
    }

    private static string GenerateDeserializationSource(List<ClassSerializationInfo> classSerializationInfos,
        CancellationToken cancellationToken)
    {
        var generator = new CodeGenerator();
        generator.AddLine("#nullable restore");
        generator.AddLine();
        generator.AddLine("using BadgerSerialization;");
        generator.AddLine("using BadgerSerialization.Core;");
        generator.AddLine("using BadgerSerialization.Types;");
        generator.AddLine("using System.Collections.Generic;");

        foreach (var classSerializationInfo in classSerializationInfos)
        {
            cancellationToken.ThrowIfCancellationRequested();

            generator.EnterScope($"namespace {classSerializationInfo.Namespace}");
            generator.EnterScope($"public partial class {classSerializationInfo.Name}");

            generator.EnterScope($"private {classSerializationInfo.Name}(Dictionary<string, BadgerObject> data)");
            foreach (var propertyInfo in classSerializationInfo.Properties)
            {
                var propertyName = propertyInfo.PropertyName;
                var badgerName = propertyInfo.BadgerName;
                var type = propertyInfo.Type;

                if (type.IsSimpleType())
                    generator.AddLine($"{propertyName} = ((Badger{type})data[\"{badgerName}\"]).Value;");
                else
                {
                    static string[] ParseGenericTypes(string type)
                    {
                        var genericStart = type.IndexOf('<') + 1;
                        var genericEnd = type.LastIndexOf('>');

                        string[] genericValues;
                        if (genericStart == -1 || genericEnd == -1)
                            genericValues = Array.Empty<string>();
                        else
                            genericValues = type
                                .Substring(genericStart, genericEnd - genericStart)
                                .Replace(" ", "")
                                .Split(',');

                        return genericValues;
                    }

                    var genericValues = ParseGenericTypes(propertyInfo.ComplexTypeName);

                    switch (type)
                    {
                        case BadgerObjectType.Compound:
                            if (genericValues.Length != 0)
                                break;

                            generator.AddLine($"{propertyName} = {propertyInfo.ComplexTypeName}.Deserialize(((Badger{type})data[\"{badgerName}\"]).Value);");
                            break;
                        case {} when type.IsList():
                            if (genericValues.Length != 1)
                                break;

                            generator.AddLine($"{propertyName} = new {propertyInfo.ComplexTypeName}();");

                            generator.EnterScope($"foreach (var obj in ((Badger{type})data[\"{badgerName}\"]).Value)");
                            generator.AddLine($"{propertyName}.Add({genericValues[0]}.Deserialize(obj));");
                            generator.LeaveScope();
                            break;

                        case BadgerObjectType.Dictionary:
                            if (genericValues.Length != 2)
                                break;

                            generator.AddLine($"{propertyName} = new {propertyInfo.ComplexTypeName}();");
                            generator.EnterScope($"foreach (var obj in ((Badger{type})data[\"{badgerName}\"]).Value)");

                            /*if (keyType.IsSimpleType())
                                generator.AddLine($"var key = ((Badger{keyType})obj.Key).Value;");
                            else
                            {
                                if (keyType == BadgerObjectType.Compound)
                                {
                                    generator.AddLine($"var key = {genericValues[0]}.Deserialize(((Badger{keyType})obj.Key).Value);");
                                }
                            }*/
                            generator.AddLine($"var value = {genericValues[1]}.Deserialize(obj);");

                            generator.AddLine($"{propertyName}[value.Key] = value;");
                            generator.LeaveScope();
                            break;
                    }
                }
            }
            generator.LeaveScope();
            generator.AddLine();

            generator.EnterScope($"public static {classSerializationInfo.Name} Deserialize(Dictionary<string, BadgerObject> data)");
            generator.AddLine($"var obj = new {classSerializationInfo.Name}(data);");
            generator.AddLine($"return obj;");
            generator.LeaveScope();
            generator.AddLine();

            generator.AddLine($"public static {classSerializationInfo.Name} Deserialize(BadgerCompound data) => Deserialize(data.Value);");

            generator.LeaveScope();
            generator.LeaveScope();
            generator.AddLine();
        }

        return generator.ToString();
    }
}