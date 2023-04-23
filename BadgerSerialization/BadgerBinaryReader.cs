using System.Diagnostics;
using System.Numerics;
using System.Text;
using BadgerSerialization.BlockApi;
using BadgerSerialization.Core;

namespace BadgerSerialization;

public delegate T ReadEntry<out T>(Dictionary<string, BadgerBinaryReader> readers);

public class BadgerBinaryReader : BinaryReader
{
    public BadgerBinaryReader(Stream input) : base(input) { }

    public BadgerBinaryReader(Stream input, Encoding encoding) : base(input, encoding) { }

    public BadgerBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen) { }

    [DebuggerStepThrough]
    public uint ReadVarUInt32()
        => (uint) ReadVarUInt64();

 //   [DebuggerStepThrough]
    public ulong ReadVarUInt64()
    {
        ulong value = 0;
        var shiftCount = 0;
        ulong current;

        do
        {
            current = ReadByte();
            value |= (current & 0x7f) << shiftCount;
            shiftCount += 7;
        } while ((current & 0x80) != 0);

        return value;
    }

    [DebuggerStepThrough]
    public BadgerObjectType ReadType()
    {
        var val = ReadByte();
        var type = (BadgerObjectType)val;
        if (!Enum.IsDefined(typeof(BadgerObjectType), type))
            throw new InvalidDataException($"Read invalid badger tag type. Value: 0x{val:x2}, Position: 0x{BaseStream.Position - 1:x}");

        return type;
    }

    [DebuggerStepThrough]
    public string ReadBadgerString()
    {
        var len = checked((int)ReadVarUInt32());
        return Encoding.UTF8.GetString(ReadBytes(len));
    }

    [DebuggerStepThrough]
    public Vector3 ReadVector3F()
    {
        var x = ReadSingle();
        var y = ReadSingle();
        var z = ReadSingle();
        return new Vector3(x, y, z);
    }

    [DebuggerStepThrough]
    public Vector2 ReadVector2F()
    {
        var x = ReadSingle();
        var y = ReadSingle();
        return new Vector2(x, y);
    }

    #region Block API Methods

    public List<T> ReadList<T>(ReadEntry<T> readEntry, uint count = uint.MaxValue)
    {
        if (count == uint.MaxValue)
            count = ReadVarUInt32();

        if (count == 0)
            return new List<T>();

        var entries = new List<T>();

        var readers = ReadCompound().ToDictionary(x => x.Value.Name, x => x.Value.GetReader());
        for (int i = 0; i < count; i++)
            entries.Add(readEntry(readers));

        Debug.Assert((uint)entries.Count == count, "entries.Count == count");

        foreach (var reader in readers.Values)
            reader.Dispose();

        return entries;
    }

    public Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(ReadEntry<(TKey key, TValue value)> readPair)
        where TKey: notnull
    {
        var count = ReadVarUInt32();
        if (count == 0)
            return new Dictionary<TKey, TValue>();

        var keyBlock = new BadgerBlock(this);
        var readers = ReadCompound().ToDictionary(x => x.Value.Name, x => x.Value.GetReader());

        readers[keyBlock.Name] = keyBlock.GetReader();

        var dict = new Dictionary<TKey, TValue>();
        for (uint i = 0; i < count; i++)
        {
            var (key, value) = readPair(readers);
            dict[key] = value;
        }

        foreach (var reader in readers.Values)
            reader.Dispose();

        return dict;
    }

    public Dictionary<string, BadgerBlock> ReadCompound()
    {
        var compound = new Dictionary<string, BadgerBlock>();

        var block = new BadgerBlock(this);
        while (block.Type != BadgerObjectType.End)
        {
            compound[block.Name] = block;
            block = new BadgerBlock(this);
        }

        return compound;
    }

    #endregion
}