using fNbt;
using MiNET.LevelDB;

namespace LegendsToolbox.Core.Game.RuntimeData;

public class EntityFactorySetupData
{
    private readonly NbtFile _entry;

    public EntityFactorySetupData(Database db)
    {
        _entry = db.GetNbtEntry("EntityFactorySetupData"u8.ToArray());
    }
}