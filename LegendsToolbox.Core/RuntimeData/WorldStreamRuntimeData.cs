using MiNET.LevelDB;

namespace LegendsToolbox.Core.RuntimeData;

public class WorldStreamRuntimeData : GameLayerRuntimeData
{
    public WorldStreamRuntimeData(Database db) : base(db, nameof(WorldStreamRuntimeData)) { }
}