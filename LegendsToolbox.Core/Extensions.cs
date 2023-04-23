using fNbt;
using MiNET.LevelDB;

namespace LegendsToolbox.Core;

internal static class Extensions
{
    internal static NbtFile GetNbtEntry(this Database db, Span<byte> key)
    {
        var data = db.GetEntry(key);
        var parsedData = new NbtFile
        {
            BigEndian = false,
            UseVarInt = false,
        };
        parsedData.LoadFromBuffer(data, 0, data.Length, NbtCompression.AutoDetect);
        return parsedData;
    }

    internal static byte[] GetEntry(this Database db, Span<byte> key)
    {
        var data = db.Get(key);
        if (data == null)
            throw new InvalidDataException($"Failed to find key '{Convert.ToHexString(key)}' in world database.");

        return data;
    }
}