using System.Diagnostics;
using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public abstract class BadgerObject
{
    public abstract BadgerObjectType Type { get; }
    public string Name { get; }

    internal BadgerObject(string name)
    {
        Name = name;
    }

    public abstract void Serialize(BadgerBinaryWriter writer);
    public abstract void Deserialize(BadgerBinaryReader reader, uint maxLength);
    public abstract void PrintValue(StringBuilder sb, int indentLevel, string indentString);

    internal static BadgerObject GetObjectForType(BadgerObjectType type, string name)
    {
        //Debug.WriteLine($"Creating object of type {type} ({name})");

        return type switch
        {
            BadgerObjectType.Boolean => new BadgerBoolean(name),
            BadgerObjectType.Short => new BadgerShort(name),
            BadgerObjectType.Integer => new BadgerInteger(name),
            BadgerObjectType.Long => new BadgerLong(name),
            BadgerObjectType.VarUInt => new BadgerVarUInt(name),
            BadgerObjectType.VarULong => new BadgerVarULong(name),
            BadgerObjectType.UShort => new BadgerUShort(name),
            BadgerObjectType.Float => new BadgerFloat(name),
            BadgerObjectType.String => new BadgerString(name),
            BadgerObjectType.Compound => new BadgerCompound(name),
            BadgerObjectType.HashedEnum => new BadgerHashedEnum(name),
            BadgerObjectType.UnsignedIntegerList => new BadgerUnsignedIntegerList(name),
            BadgerObjectType.OptionalVarUInt => new BadgerOptionalVarUInt(name),
            BadgerObjectType.DynamicList => new BadgerDynamicList(name),
            BadgerObjectType.Vector2F => new BadgerVector2F(name),
            BadgerObjectType.Vector3F => new BadgerVector3F(name),
            BadgerObjectType.AxisAlignedBoundingBox => new BadgerAxisAlignedBoundingBox(name),
            BadgerObjectType.Rect => new BadgerRect(name),
            BadgerObjectType.DictionaryKey => new BadgerDictionaryKey(name),
            BadgerObjectType.Vector3I => new BadgerVector3I(name),
            BadgerObjectType.Dictionary => new BadgerDictionary(name),
            BadgerObjectType.PositionList => new BadgerPositionList(name),
            BadgerObjectType.List => new BadgerList(name),
            BadgerObjectType.End => throw new InvalidOperationException("Tried to read End object type."),
            BadgerObjectType.OptionalString => new BadgerOptionalString(name),
            BadgerObjectType.OptionalStringList => new BadgerOptionalStringList(name),
            BadgerObjectType.OptionalString2 => new BadgerOptionalString2(name),
            BadgerObjectType.EntityUniqueId => new BadgerEntityUniqueId(name),
            BadgerObjectType.OptionalHashedEnum => new BadgerOptionalHashedEnum(name),
            BadgerObjectType.OptionalString3 => new BadgerOptionalString3(name),
            BadgerObjectType.SerializedBlock => new BadgerSerializedBlock(name),
            _ => throw new InvalidOperationException($"Tried to create invalid object type. Type: {type}")
        };
    }

    internal static BadgerObject Read(BadgerBinaryReader reader, BadgerObjectType type)
    {
        var name = reader.ReadBadgerString();

        var dataLength = reader.ReadVarUInt32();
        if (dataLength > int.MaxValue)
            throw new NotImplementedException();

        var startingPos = reader.BaseStream.Position;

        var obj = GetObjectForType(type, name);

        obj.Deserialize(reader, dataLength);

        var readCount = reader.BaseStream.Position - startingPos;
        if (readCount < 0)
            throw new InvalidDataException("Read negative bytes while reading tag value.");

        if ((ulong)readCount != dataLength)
            throw new InvalidDataException(
                $"Did not read the correct amount of bytes while reading tag value. Read: {readCount}, expected: {dataLength}");

        return obj;
    }

    internal byte[] Write()
    {
        using var ms = new MemoryStream();
        using var writer = new BadgerBinaryWriter(ms);

        writer.Write((byte)Type);
        writer.WriteBadgerString(Name);

        using var valueMs = new MemoryStream();
        using var valueWriter = new BadgerBinaryWriter(valueMs);
        Serialize(valueWriter);
        var valueBuffer = valueMs.ToArray();

        writer.WriteVarUInt32(valueBuffer.Length);
        writer.Write(valueBuffer);
        return ms.ToArray();
    }

    internal static List<Dictionary<string, BadgerObject>> ReadList(BadgerBinaryReader reader, int count)
    {
        var objs = new Dictionary<string, List<BadgerObject>>();

        var type = reader.ReadType();
        while (type != BadgerObjectType.End)
        {
            var name = reader.ReadBadgerString();
            var length = reader.ReadVarUInt32();
            var list = new List<BadgerObject>();

            var startPos = reader.BaseStream.Position;

            for (int i = 0; i < count; i++)
            {
                var obj = GetObjectForType(type, name);
                obj.Deserialize(reader, length);
                list.Add(obj);
            }

            Debug.Assert(startPos + length == reader.BaseStream.Position, "startPos + length == reader.BaseStream.Position");

            objs[name] = list;
            type = reader.ReadType();
        }

        var entries = new List<Dictionary<string, BadgerObject>>();

        for (int i = 0; i < count; i++)
        {
            var currentObj = new Dictionary<string, BadgerObject>();
            foreach (var pair in objs)
                currentObj[pair.Key] = pair.Value[i];

            entries.Add(currentObj);
        }

        return entries;
    }

    internal static void WriteList(BadgerBinaryWriter writer, List<Dictionary<string, BadgerObject>> objects)
    {
        var properties = new Dictionary<string, List<BadgerObject>>();

        foreach (var property in objects.SelectMany(obj => obj))
        {
            if (properties.TryGetValue(property.Key, out var propertyList))
                propertyList.Add(property.Value);
            else
                properties[property.Key] = new List<BadgerObject> {property.Value};
        }

        foreach (var property in properties)
        {
            writer.Write((byte)property.Value.First().Type);
            writer.WriteBadgerString(property.Key);

            using var propertyValueMs = new MemoryStream();
            using var propertyValueWriter = new BadgerBinaryWriter(propertyValueMs);

            foreach (var value in property.Value)
                value.Serialize(propertyValueWriter);

            var propertyValueBuf = propertyValueMs.ToArray();

            writer.WriteVarUInt32(propertyValueBuf.Length);
            writer.Write(propertyValueBuf);
        }

        writer.Write((byte)BadgerObjectType.End);
    }

    public void Print(StringBuilder sb, int indentLevel, string indentString)
    {
        if (!string.IsNullOrEmpty(Name))
            sb.Append($"{string.Join("", Enumerable.Repeat(indentString, indentLevel))}\"{Name}_{Type}\": ");
        PrintValue(sb, indentLevel, indentString);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        Print(sb, 0, "  ");
        return sb.ToString();
    }
}