using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerShort : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.Short;

    public short Value { get; set; }

    public BadgerShort(string name, short value = 0) : base(name)
    {
        Value = value;
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.Write(Value);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = reader.ReadInt16();
    }
    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value);
}