using System.Diagnostics;
using System.Text;

namespace BadgerSerialization.Nbt;

public static class DynamicNbt
{
    public static Dictionary<string, dynamic> Read(BadgerBinaryReader reader)
    {
        var tag = reader.ReadByte();
        if (tag != 10)
            throw new NotImplementedException();

        var nameLength = reader.ReadUInt16();
        var name = Encoding.UTF8.GetString(reader.ReadBytes(nameLength));
        var value = ReadTag(reader, tag);
        return value;
    }

    public static byte[] Write(Dictionary<string, dynamic> value)
    {
        using var ms = new MemoryStream();
        using var writer = new BadgerBinaryWriter(ms);

        writer.Write((byte) 10);
        writer.Write((ushort)0);
        WriteCompound(writer, value);

        return ms.ToArray();
    }

    private static Dictionary<string, dynamic> ReadCompound(BadgerBinaryReader reader)
    {
        var properties = new Dictionary<string, dynamic>();

        while (true)
        {
            var tag = reader.ReadByte();
            if (tag == 0)
                break;

            var nameLength = reader.ReadUInt16();
            var name = Encoding.UTF8.GetString(reader.ReadBytes(nameLength)); 
            var value = ReadTag(reader, tag);

            properties[name] = value;
        }

        return properties;
    }

    private static void WriteCompound(BadgerBinaryWriter writer, Dictionary<string, dynamic> value)
    {
        foreach (var pair in value)
        {
            var type = (byte)GetTag(pair.Value);
            writer.Write(type);
            writer.Write((ushort)pair.Key.Length);
            writer.Write(Encoding.UTF8.GetBytes(pair.Key));
            WriteTag(writer, pair.Value, type);
        }

        writer.Write((byte)0);
    }

    private static dynamic ReadTag(BadgerBinaryReader reader, byte type)
    {
        dynamic value;

        switch (type)
        {
            case 1:
                value = reader.ReadByte();
                break;
            case 2:
                value = reader.ReadInt16();
                break;
            case 3:
                value = reader.ReadInt32();
                break;
            case 4:
                value = reader.ReadInt64();
                break;
            case 5:
                value = reader.ReadSingle();
                break;
            case 6:
                value = reader.ReadDouble();
                break;
            case 7:
            case 8:
                var len = type == 7 ? reader.ReadInt32() : reader.ReadUInt16();
                value = reader.ReadBytes(len);
                break;
            case 9:
                value = ReadList(reader);
                break;
            case 10:
                value = ReadCompound(reader);
                break;
            case 11:
                value = ReadIntList(reader);
                break;
            case 12:
                value = ReadLongList(reader);
                break;
            default:
                value = null!;
                break;
        }

        return value;
    }

    private static void WriteTag(BadgerBinaryWriter writer, dynamic value, byte tag = 0)
    {
        switch (tag == 0 ? (int)GetTag(value) : tag)
        {
            case 1:
                writer.Write((byte)value);
                break;
            case 2:
                writer.Write((short)value);
                break;
            case 3:
                writer.Write((int)value);
                break;
            case 4:
                writer.Write((long)value);
                break;
            case 5:
                writer.Write((float)value);
                break;
            case 6:
                writer.Write((double)value);
                break;
            case 7:
                writer.Write(((byte[])value).Length);
                writer.Write((byte[])value);
                break;
            case 8:
                writer.Write((ushort)((byte[])value).Length);
                writer.Write((byte[])value);
                break;
            case 9:
                WriteList(writer, value);
                break;
            case 10:
                WriteCompound(writer, value);
                break;
            case 11:
                WriteIntList(writer, value);
                break;
            case 12:
                WriteLongList(writer, value);
                break;
            default:
                Debugger.Break();
                break;
        }
    }

    private static List<dynamic> ReadList(BadgerBinaryReader reader)
    {
        var type = reader.ReadByte();
        var count = reader.ReadInt32();
        var list = new List<dynamic>();
        for (int i = 0; i < count; i++)
            list.Add(ReadTag(reader, type));

        return list;
    }

    private static void WriteList(BadgerBinaryWriter writer, List<dynamic> value)
    {
        if (value.Count == 0)
        {
            writer.Write((byte) 0);
            writer.Write(value.Count);
            return;
        }

        writer.Write(GetTag(value.First()));
        writer.Write(value.Count);
        foreach (var val in value)
            WriteTag(writer, val);
    }

    private static List<int> ReadIntList(BadgerBinaryReader reader)
    {
        var count = reader.ReadInt32();
        var list = new List<int>();
        for (int i = 0; i < count; i++)
            list.Add(reader.ReadInt32());

        return list;
    }

    private static void WriteIntList(BadgerBinaryWriter writer, List<int> value)
    {
        writer.Write(value.Count);
        foreach (var val in value)
            writer.Write(val);
    }

    private static List<long> ReadLongList(BadgerBinaryReader reader)
    {
        var count = reader.ReadInt32();
        var list = new List<long>();
        for (int i = 0; i < count; i++)
            list.Add(reader.ReadInt64());

        return list;
    }

    private static void WriteLongList(BadgerBinaryWriter writer, List<long> value)
    {
        writer.Write(value.Count);
        foreach (var val in value)
            writer.Write(val);
    }

    private static byte GetTag(dynamic value)
    {
        return value switch
        {
            byte => 1,
            short => 2,
            int => 3,
            long => 4,
            float => 5,
            double => 6,
            byte[] => 8, // Treat byte arrays as strings
            List<dynamic> => 9,
            Dictionary<string, dynamic> => 10,
            List<int> => 11,
            List<long> => 12,
            _ => 0
        };
    }
}