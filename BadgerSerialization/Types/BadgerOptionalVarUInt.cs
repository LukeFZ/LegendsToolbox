using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerOptionalVarUInt : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.OptionalVarUInt;

    public uint? Value { get; set; }

    public BadgerOptionalVarUInt(string name, uint? value = null) : base(name)
    {
        Value = value;
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.Write(Value.HasValue);

        if (Value.HasValue)
            writer.WriteVarUInt32(Value.Value + 1);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = null;
        if (!reader.ReadBoolean())
            return;

        Value = reader.ReadVarUInt32() - 1;
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value.HasValue ? Value : "null");
}