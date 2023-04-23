using MiNET.LevelDB;

namespace LegendsToolbox.Core.RuntimeData;

public class InvasionRuntimeData : GameLayerRuntimeData
{
    public InvasionRuntimeData(Database db) : base(db, nameof(InvasionRuntimeData)) { }
}