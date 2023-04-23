using MiNET.LevelDB;

namespace LegendsToolbox.Core.RuntimeData;

public class BSharpRuntimeData : GameLayerRuntimeData
{
    public BSharpRuntimeData(Database db) : base(db, nameof(BSharpRuntimeData)) { }
}