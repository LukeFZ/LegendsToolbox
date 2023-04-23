using System.Globalization;
using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerFloat : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.Float;

    public float Value { get; set; }

    public BadgerFloat(string name, float value = 0) : base(name)
    {
        Value = value;
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.Write(Value);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = reader.ReadSingle();
    }
    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value.ToString(CultureInfo.InvariantCulture));
}