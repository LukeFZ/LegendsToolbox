using LegendsToolbox.Core.Game;

namespace LegendsToolbox.Core.Interfaces;

public interface ISaveData
{
    public Task<GameSave> GetGameSaveAsync();
    public Task SaveAsync();
}