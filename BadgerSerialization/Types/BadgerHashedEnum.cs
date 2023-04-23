using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerHashedEnum : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.HashedEnum;

    public uint Value { get; set; }

    public BadgerHashedEnum(string name, uint value = 0) : base(name)
    {
        Value = value;
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.Write(Value);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = reader.ReadUInt32();
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value);
}