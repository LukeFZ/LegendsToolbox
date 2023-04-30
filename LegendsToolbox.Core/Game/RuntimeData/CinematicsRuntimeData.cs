using MiNET.LevelDB;

namespace LegendsToolbox.Core.Game.RuntimeData;

public class CinematicsRuntimeData : GameLayerRuntimeData
{
    public CinematicsRuntimeData(Database db) : base(db, nameof(CinematicsRuntimeData)) { }
}