using MiNET.LevelDB;

namespace LegendsToolbox.Core.RuntimeData;

public class BadgerSyncedGameStartRuntimeData : GameLayerRuntimeData
{
    public BadgerSyncedGameStartRuntimeData(Database db) : base(db, nameof(BadgerSyncedGameStartRuntimeData)) { }
}