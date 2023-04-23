using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerInteger : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.Integer;

    public int Value { get; set; }

    public BadgerInteger(string name, int value = 0) : base(name)
    {
        Value = value;
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.Write(Value);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = reader.ReadInt32();
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value);
}