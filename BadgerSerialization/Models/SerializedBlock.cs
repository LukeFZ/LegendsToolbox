namespace BadgerSerialization.Models;

public struct SerializedBlock
{
    public uint EntityRuntimeId;
    public ulong NameHash;
    public Dictionary<ulong, byte> States;

    public SerializedBlock(ulong nameHash, Dictionary<ulong, byte> states, uint entityRuntimeId = uint.MaxValue)
    {
        EntityRuntimeId = entityRuntimeId;
        NameHash = nameHash;
        States = states;
    }

    public override string ToString()
        => EntityRuntimeId == uint.MaxValue
            ? $"{{\"nameHash\": {NameHash}, \"states\": [{string.Join(", ", States.Select(pair => $"{{\"nameHash\": {pair.Key}, \"value\": {pair.Value}}}"))}]}}"
            : $"{{\"entityRuntimeId\": {EntityRuntimeId}, \"nameHash\": {NameHash}, \"states\": [{string.Join(", ", States.Select(pair => $"{{\"nameHash\": {pair.Key}, \"value\": {pair.Value}}}"))}]}}";
}