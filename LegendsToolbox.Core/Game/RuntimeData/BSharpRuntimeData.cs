using MiNET.LevelDB;

namespace LegendsToolbox.Core.Game.RuntimeData;

public class BSharpRuntimeData : GameLayerRuntimeData
{
    public BSharpRuntimeData(Database db) : base(db, nameof(BSharpRuntimeData)) { }
}