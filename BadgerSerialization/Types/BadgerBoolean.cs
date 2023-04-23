using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerBoolean : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.Boolean;

    public bool Value { get; set; }

    public BadgerBoolean(string name, bool value = false) : base(name)
    {
        Value = value;
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.Write(Value);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = reader.ReadBoolean();
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value ? "true" : "false");
}