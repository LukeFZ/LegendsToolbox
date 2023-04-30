using System.Runtime.InteropServices;
using LegendsToolbox.Core.Game.RuntimeData;

namespace LegendsToolbox.Core.Interfaces;

public interface IWorld : IAsyncObject
{
    public string Name { get; }
    public string Type { get; }

    public IAsyncEnumerable<GameLayerRuntimeData> GetAllRuntimeDataAsync();
    public Task<GameLayerRuntimeData> GetRuntimeDataAsync(string name);
    public Task SetRuntimeDataAsync(GameLayerRuntimeData runtimeData);
}