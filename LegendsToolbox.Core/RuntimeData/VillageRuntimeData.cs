using MiNET.LevelDB;

namespace LegendsToolbox.Core.RuntimeData;

public class VillageRuntimeData : GameLayerRuntimeData
{
    public VillageRuntimeData(Database db) : base(db, nameof(VillageRuntimeData)) { }
}