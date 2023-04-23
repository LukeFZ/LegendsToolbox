using MiNET.LevelDB;

namespace LegendsToolbox.Core.RuntimeData;

public class MapRevealRuntimeData : GameLayerRuntimeData
{
    public MapRevealRuntimeData(Database db) : base(db, nameof(MapRevealRuntimeData)) { }
}