using MiNET.LevelDB;

namespace LegendsToolbox.Core.RuntimeData;

public class GeologyRuntimeData : GameLayerRuntimeData
{
    public GeologyRuntimeData(Database db) : base(db, nameof(GeologyRuntimeData)) { }
}