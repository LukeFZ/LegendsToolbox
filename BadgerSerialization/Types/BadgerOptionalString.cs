using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerOptionalString : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.OptionalString;

    public string? Value { get; set; }

    public BadgerOptionalString(string name, string? value = null) : base(name)
    {
        Value = value;
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.Write(Value != null);

        if (Value != null)
            writer.WriteBadgerString(Value);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = null;
        if (!reader.ReadBoolean())
            return;

        Value = reader.ReadBadgerString();
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value == null ? "null" : $"\"{Value}\"");
}