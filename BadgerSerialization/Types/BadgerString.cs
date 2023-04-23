using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerString : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.String;

    public string Value { get; set; }

    public BadgerString(string name, string? value = null) : base(name)
    {
        Value = value ?? string.Empty;
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.WriteBadgerString(Value);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = reader.ReadBadgerString();
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append($"\"{Value}\"");
}