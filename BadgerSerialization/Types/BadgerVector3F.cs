using System.Globalization;
using System.Numerics;
using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerVector3F : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.Vector3F;

    public Vector3 Value { get; set; }

    public BadgerVector3F(string name, Vector3 data = default) : base(name)
    {
        Value = data;
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.Write(Value);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = reader.ReadVector3F();
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append($"{{\"x\": {Value.X.ToString(CultureInfo.InvariantCulture)}, \"y\": {Value.Y.ToString(CultureInfo.InvariantCulture)}, \"z\": {Value.Z.ToString(CultureInfo.InvariantCulture)}}}");
}