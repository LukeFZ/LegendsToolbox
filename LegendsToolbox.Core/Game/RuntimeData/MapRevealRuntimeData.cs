using MiNET.LevelDB;

namespace LegendsToolbox.Core.Game.RuntimeData;

public class MapRevealRuntimeData : GameLayerRuntimeData
{
    public MapRevealRuntimeData(Database db) : base(db, nameof(MapRevealRuntimeData)) { }
}