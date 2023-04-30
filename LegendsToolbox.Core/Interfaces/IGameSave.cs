using System.Collections.ObjectModel;

namespace LegendsToolbox.Core.Interfaces;

public interface IGameSave : IAsyncObject
{
    public ReadOnlyCollection<IWorld> Worlds { get; }
    public IOptions Options { get; }

    public IAsyncEnumerable<string> GetAllWorldNamesAsync();
    public void RemoveWorld(string name);
    public void RemoveWorld(IWorld world);
}