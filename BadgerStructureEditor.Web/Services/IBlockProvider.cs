using BadgerSerialization.Models;
using BadgerStructureEditor.Web.Model;

namespace BadgerStructureEditor.Web.Services;

public interface IBlockProvider
{
    public bool Initialized { get; }
    public Task Initialize();
    public SerializedBlockInfo ConvertBlock(SerializedBlock block);
    public SerializedBlockInfo ConvertBlock(Dictionary<string, dynamic> block);
    public SerializedBlockInfo Create(string name);

    public BlockInfo? GetBlock(string name);
    public StateInfo? GetState(string name);
}