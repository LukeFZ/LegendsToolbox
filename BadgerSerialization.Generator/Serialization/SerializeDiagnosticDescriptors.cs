using Microsoft.CodeAnalysis;

namespace BadgerSerialization.Generator.Serialization;

public static class SerializeDiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor ClassMustBePartial
        = new("BADGER001",
            "Class must be partial",
            "The class '{0}' must be partial",
            nameof(SerializeAnalyzer),
            DiagnosticSeverity.Error,
            true);

    public static readonly DiagnosticDescriptor InvalidDictionaryKeyType
        = new("BADGER002",
            "Invalid dictionary key type",
            "The dictionary key type '{0}' is not allowed for serialization",
            nameof(SerializeAnalyzer),
            DiagnosticSeverity.Error,
            true);
}