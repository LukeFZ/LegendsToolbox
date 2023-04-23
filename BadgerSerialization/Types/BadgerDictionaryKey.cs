using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerDictionaryKey : BadgerString
{
    public override BadgerObjectType Type => BadgerObjectType.DictionaryKey;

    public BadgerDictionaryKey(string name, string? value = null) : base(name, value) { }
}