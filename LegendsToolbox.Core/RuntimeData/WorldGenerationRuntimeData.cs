using MiNET.LevelDB;

namespace LegendsToolbox.Core.RuntimeData;

public class WorldGenerationRuntimeData : GameLayerRuntimeData
{
    public WorldGenerationRuntimeData(Database db) : base(db, nameof(WorldGenerationRuntimeData)) { }
}