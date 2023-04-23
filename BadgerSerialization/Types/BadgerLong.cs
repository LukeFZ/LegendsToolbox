using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerLong : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.Long;

    public long Value { get; set; }

    public BadgerLong(string name, long value = 0) : base(name)
    {
        Value = value;
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.Write(Value);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = reader.ReadInt64();
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value);
}