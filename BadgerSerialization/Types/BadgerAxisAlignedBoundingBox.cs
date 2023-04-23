using System.Text;
using BadgerSerialization.Core;
using BadgerSerialization.Models;

namespace BadgerSerialization.Types;

public class BadgerAxisAlignedBoundingBox : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.AxisAlignedBoundingBox;

    public AxisAlignedBoundingBox Value { get; set; }

    public BadgerAxisAlignedBoundingBox(string name, AxisAlignedBoundingBox data = default) : base(name)
    {
        Value = data;
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.Write(Value.Min);
        writer.Write(Value.Max);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        var min = reader.ReadVector3F();
        var max = reader.ReadVector3F();
        Value = new AxisAlignedBoundingBox(min, max);
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value);
}