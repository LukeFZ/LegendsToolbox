using System.Globalization;
using System.Numerics;
using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerVector2F : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.Vector2F;

    public Vector2 Value { get; set; }

    public BadgerVector2F(string name, Vector2 data = default) : base(name)
    {
        Value = data;
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.Write(Value);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = reader.ReadVector2F();
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append($"{{\"x\": {Value.X.ToString(CultureInfo.InvariantCulture)}, \"y\": {Value.Y.ToString(CultureInfo.InvariantCulture)}}}");
}