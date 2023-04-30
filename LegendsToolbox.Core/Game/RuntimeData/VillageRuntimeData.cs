using MiNET.LevelDB;

namespace LegendsToolbox.Core.Game.RuntimeData;

public class VillageRuntimeData : GameLayerRuntimeData
{
    public VillageRuntimeData(Database db) : base(db, nameof(VillageRuntimeData)) { }
}