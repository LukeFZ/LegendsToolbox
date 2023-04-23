using System.Diagnostics;
using BadgerSerialization.Core;

namespace BadgerSerialization.BlockApi;

// NOTE: Does not support all block types, use the BadgerObject API instead
public class BadgerBlock
{
    public BadgerObjectType Type { get; }
    public string Name { get; }
    public byte[] Data { get; }

    public BadgerBlock(BadgerBinaryReader reader)
    {
        Type = reader.ReadType();
        if (Type == BadgerObjectType.End)
        {
            Name = string.Empty;
            Data = Array.Empty<byte>();
            return;
        }

        Name = reader.ReadBadgerString();
        var dataLength = checked((int)reader.ReadVarUInt32());
        Data = reader.ReadBytes(dataLength);
    }

    public BadgerBlock(BadgerObjectType type, string name, byte[] data)
    {
        Type = type;
        Name = name;
        Data = data;
    }

    public static Dictionary<string, BadgerBlock> Load(byte[] data)
    {
        var root = new BadgerBlock(BadgerObjectType.Compound, string.Empty, data);
        return root.AsCompound();
    }

    public BadgerBinaryReader GetReader() => new(new MemoryStream(Data));

    public string AsString()
    {
        Debug.Assert(Type == BadgerObjectType.String || Type == BadgerObjectType.DictionaryKey, "Type == BadgerObjectType.String || Type == BadgerObjectType.DictionaryKey");
        using var reader = GetReader();
        return reader.ReadBadgerString();
    }

    public bool AsBoolean()
    {
        Debug.Assert(Type == BadgerObjectType.Boolean, "Type == BadgerObjectType.Boolean");
        using var reader = GetReader();
        return reader.ReadBoolean();
    }

    public uint AsVarUInt()
    {
        Debug.Assert(Type == BadgerObjectType.VarUInt, "Type == BadgerObjectType.VarUInt");
        using var reader = GetReader();
        return reader.ReadVarUInt32();
    }

    public ulong AsVarULong()
    {
        Debug.Assert(Type == BadgerObjectType.VarULong, "Type == BadgerObjectType.VarULong");
        using var reader = GetReader();
        return reader.ReadVarUInt64();
    }

    public int AsInt()
    {
        Debug.Assert(Type == BadgerObjectType.Integer, "Type == BadgerObjectType.Integer");
        using var reader = GetReader();
        return reader.ReadInt32();
    }

    public float AsFloat()
    {
        Debug.Assert(Type == BadgerObjectType.Float, "Type == BadgerObjectType.Float");
        using var reader = GetReader();
        return reader.ReadSingle();
    }

    public Dictionary<string, BadgerBlock> AsCompound()
    {
        Debug.Assert(Type == BadgerObjectType.Compound, "Type == BadgerObjectType.Compound");
        using var reader = GetReader();
        return reader.ReadCompound();
    }

    public List<T> AsList<T>(ReadEntry<T> readEntry)
    {
        Debug.Assert(Type == BadgerObjectType.List, "Type == BadgerObjectType.List");
        using var reader = GetReader();
        return reader.ReadList(readEntry);
    }

    public Dictionary<TKey, TValue> AsDictionary<TKey, TValue>(ReadEntry<(TKey key, TValue value)> readPair)
        where TKey: notnull
    {
        Debug.Assert(Type == BadgerObjectType.Dictionary, "Type == BadgerObjectType.Dictionary");
        using var reader = GetReader();
        return reader.ReadDictionary(readPair);
    }
}