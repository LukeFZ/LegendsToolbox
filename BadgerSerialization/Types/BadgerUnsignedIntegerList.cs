using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerUnsignedIntegerList : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.UnsignedIntegerList;

    public List<uint> Value { get; set; }

    public BadgerUnsignedIntegerList(string name, List<uint>? value = null) : base(name)
    {
        Value = value ?? new List<uint>();
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.WriteVarUInt32(Value.Count);

        foreach (var val in Value)
            writer.Write(val);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = new List<uint>();

        var count = checked((int)reader.ReadVarUInt32());
        for (uint i = 0; i < count; i++)
            Value.Add(reader.ReadUInt32());
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append($"[{string.Join(", ", Value)}]");
}