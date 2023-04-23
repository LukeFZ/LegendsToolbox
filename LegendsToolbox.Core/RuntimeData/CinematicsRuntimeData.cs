using MiNET.LevelDB;

namespace LegendsToolbox.Core.RuntimeData;

public class CinematicsRuntimeData : GameLayerRuntimeData
{
    public CinematicsRuntimeData(Database db) : base(db, nameof(CinematicsRuntimeData)) { }
}