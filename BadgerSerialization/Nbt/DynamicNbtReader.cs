using System.Text;

namespace BadgerSerialization.Nbt;

public static class DynamicNbtReader
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

    private static List<dynamic> ReadList(BadgerBinaryReader reader)
    {
        var type = reader.ReadByte();
        var count = reader.ReadInt32();
        var list = new List<dynamic>();
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
                list.Add(ReadTag(reader, type));
        }

        return list;
    }

    private static List<int> ReadIntList(BadgerBinaryReader reader)
    {
        var count = reader.ReadInt32();
        var list = new List<int>();
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
                list.Add(reader.ReadInt32());
        }

        return list;
    }

    private static List<long> ReadLongList(BadgerBinaryReader reader)
    {
        var count = reader.ReadInt32();
        var list = new List<long>();
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
                list.Add(reader.ReadInt64());
        }

        return list;
    }
}