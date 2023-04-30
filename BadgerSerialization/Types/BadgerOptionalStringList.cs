using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerOptionalStringList : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.OptionalStringList;

    public List<string>? Value { get; set; }

    public BadgerOptionalStringList(string name, List<string>? value = null) : base(name)
    {
        Value = value;
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.Write(Value != null);

        if (Value != null)
        {
            writer.WriteVarUInt32(Value.Count);

            foreach (var val in Value)
                writer.WriteBadgerString(val);
        }
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = null;
        if (!reader.ReadBoolean())
            return;

        Value = new List<string>();

        var count = checked((int)reader.ReadVarUInt32());
        for (uint i = 0; i < count; i++)
            Value.Add(reader.ReadBadgerString());
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value == null ? "null" : $"[\"{string.Join("\",\"", Value)}\"]");
}