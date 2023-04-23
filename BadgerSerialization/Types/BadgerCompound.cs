using System.Diagnostics;
using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerCompound : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.Compound;

    public Dictionary<string, BadgerObject> Value { get; set; }

    public BadgerCompound(string name, Dictionary<string, BadgerObject>? value = null) : base(name)
    {
        Value = value ?? new Dictionary<string, BadgerObject>();
    }

    public BadgerCompound(string name, IEnumerable<BadgerObject> value) : base(name)
    {
        Value = new Dictionary<string, BadgerObject>();
        foreach (var val in value)
            Value[val.Name] = val;
    }

    public static BadgerCompound Load(byte[] data)
    {
        using var ms = new MemoryStream(data);
        return Load(ms);
    }

    public static BadgerCompound Load(Stream stream)
    {
        var obj = new BadgerCompound("");
        using var reader = new BadgerBinaryReader(stream);
        obj.Deserialize(reader, (uint)stream.Length);
        return obj;
    }

    public byte[] Save()
    {
        using var ms = new MemoryStream();
        using var writer = new BadgerBinaryWriter(ms);
        Serialize(writer);
        return ms.ToArray();
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        foreach (var value in Value.Values)
            writer.Write(value.Write());

        writer.Write((byte)BadgerObjectType.End);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = new Dictionary<string, BadgerObject>();
        var id = reader.ReadType();
        while (id != BadgerObjectType.End)
        {
            //Debug.WriteLineIf(Name == "", $"Currently deserializing type {id} @ 0x{reader.BaseStream.Position:x}");
            var property = Read(reader, id);
            Value[property.Name] = property;
            id = reader.ReadType();
        }
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
    {
        var currentIndent = string.Join("", Enumerable.Repeat(indentString, indentLevel));
        if (!string.IsNullOrEmpty(Name)) sb.AppendLine("{");

        var i = 0;
        foreach (var item in Value.Values)
        {
            item.Print(sb, indentLevel + 1, indentString);

            i++;
            sb.AppendLine(i == Value.Count ? "" : ",");
        }

        if (!string.IsNullOrEmpty(Name)) sb.Append($"{currentIndent}}}");
    }
}