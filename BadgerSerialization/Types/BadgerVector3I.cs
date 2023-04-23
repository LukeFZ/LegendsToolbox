using System.Text;
using BadgerSerialization.Core;
using BadgerSerialization.Models;

namespace BadgerSerialization.Types;

public class BadgerVector3I : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.Vector3I;

    public Vector3I Value { get; set; }

    public BadgerVector3I(string name, Vector3I data = default) : base(name)
    {
        Value = data;
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        writer.Write(Value.X);
        writer.Write(Value.Y);
        writer.Write(Value.Z);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        var x = reader.ReadInt32();
        var y = reader.ReadInt32();
        var z = reader.ReadInt32();
        Value = new Vector3I(x, y, z);
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value);
}