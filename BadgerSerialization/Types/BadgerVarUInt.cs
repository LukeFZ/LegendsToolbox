using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerVarUInt : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.VarUInt;

    public uint Value { get; set; }

    public BadgerVarUInt(string name, uint value = 0) : base(name)
    {
        Value = value;
    }
    
    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.WriteVarUInt32(Value + 1);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = reader.ReadVarUInt32() - 1;
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value);
}