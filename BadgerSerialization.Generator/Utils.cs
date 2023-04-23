using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace BadgerSerialization.Generator;

public static class Utils
{
    public static TypeInfo GetTypeInfo(this Compilation compilation, SyntaxNode node)
        => compilation.GetSemanticModel(node.SyntaxTree).GetTypeInfo(node);

    public static string GetFullyQualifiedTypeName(this Compilation compilation, SyntaxNode node)
    {
        var typeInfo = GetTypeInfo(compilation, node);

        return typeInfo.Type!.ContainingNamespace == null
            ? typeInfo.Type.Name
            : $"{typeInfo.Type.ContainingNamespace.ToDisplayString()}.{typeInfo.Type.Name}";
    }

    // https://andrewlock.net/creating-a-source-generator-part-5-finding-a-type-declarations-namespace-and-type-hierarchy/#finding-the-namespace-for-a-class-syntax
    public static string GetNamespace(this BaseTypeDeclarationSyntax syntax)
    {
        // If we don't have a namespace at all we'll return an empty string
        // This accounts for the "default namespace" case
        var nameSpace = string.Empty;

        // Get the containing syntax node for the type declaration
        // (could be a nested type, for example)
        var potentialNamespaceParent = syntax.Parent;

        // Keep moving "out" of nested classes etc until we get to a namespace
        // or until we run out of parents
        while (potentialNamespaceParent != null &&
               potentialNamespaceParent is not NamespaceDeclarationSyntax
               && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        // Build up the final namespace by looping until we no longer have a namespace declaration
        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            // We have a namespace. Use that as the type
            nameSpace = namespaceParent.Name.ToString();

            // Keep moving "out" of the namespace declarations until we 
            // run out of nested namespace declarations
            while (true)
            {
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                {
                    break;
                }

                // Add the outer namespace as a prefix to the final namespace
                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                namespaceParent = parent;
            }
        }

        // return the final namespace
        return nameSpace;
    }
}