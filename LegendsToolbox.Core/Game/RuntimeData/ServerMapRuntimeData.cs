using MiNET.LevelDB;

namespace LegendsToolbox.Core.Game.RuntimeData;

public class ServerMapRuntimeData : GameLayerRuntimeData
{
    public ServerMapRuntimeData(Database db) : base(db, nameof(ServerMapRuntimeData)) { }
}