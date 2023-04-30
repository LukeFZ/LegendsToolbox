using System.Text;
using BadgerSerialization.Types;
using fNbt;
using MiNET.LevelDB;

namespace LegendsToolbox.Core.Game.RuntimeData;

public abstract class GameLayerRuntimeData
{
    public int Version;
    public readonly BadgerCompound? Data;
    private readonly NbtFile _entry;

    public GameLayerRuntimeData(Database db, string name)
    {
        _entry = db.GetNbtEntry(Encoding.UTF8.GetBytes(name));

        Version = _entry.RootTag["data"]["Version"].IntValue;
        if (name == "BSharpRuntimeData")
            File.WriteAllBytes("current_BSharpRuntimeData", _entry.RootTag["data"][name].ByteArrayValue);

        if (_entry.RootTag["data"][name] != null)
            Data = BadgerCompound.Load(_entry.RootTag["data"][name].ByteArrayValue);
    }

    public override string ToString()
        => $"{Data?.ToString() ?? "{}"}";
}