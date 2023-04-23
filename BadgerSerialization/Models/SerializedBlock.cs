namespace BadgerSerialization.Models;

public struct SerializedBlock
{
    public uint BlockId;
    public ulong NameHash;
    public Dictionary<ulong, byte> States;

    public SerializedBlock(uint blockId, ulong nameHash, Dictionary<ulong, byte> states)
    {
        BlockId = blockId;
        NameHash = nameHash;
        States = states;
    }

    public override string ToString()
        =>
            $"{{\"blockId\": {BlockId}, \"nameHash\": {NameHash}, \"states\": [{string.Join(", ", States.Select(pair => $"{{\"nameHash\": {pair.Key}, \"value\": {pair.Value}}}"))}]}}";
}