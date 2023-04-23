using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerUShort : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.UShort;

    public ushort Value { get; set; }

    public BadgerUShort(string name, ushort value = 0) : base(name)
    {
        Value = value;
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.Write(Value);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = reader.ReadUInt16();
    }
    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value);
}