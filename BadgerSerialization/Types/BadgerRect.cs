using System.Text;
using BadgerSerialization.Core;
using BadgerSerialization.Models;

namespace BadgerSerialization.Types;

public class BadgerRect : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.Rect;

    public Rect Value { get; set; }

    public BadgerRect(string name, Rect data = default) : base(name)
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
        var min = reader.ReadVector2F();
        var max = reader.ReadVector2F();
        Value = new Rect(min, max);
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value);
}