using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerVarULong : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.VarULong;

    public ulong Value { get; set; }

    public BadgerVarULong(string name, ulong value = 0) : base(name)
    {
        Value = value;
    }
    
    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.WriteVarUInt64(Value + 1);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = reader.ReadVarUInt64() - 1;
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value);
}