using MiNET.LevelDB;

namespace LegendsToolbox.Core.RuntimeData;

public class WorldFeatureRuntimeData : GameLayerRuntimeData
{
    public WorldFeatureRuntimeData(Database db) : base(db, nameof(WorldFeatureRuntimeData)) { }
}