using System.Text.Json.Serialization;

namespace BadgerStructureEditor.Web.Model;

public record HashInfo(Dictionary<ulong, BlockInfo> Blocks, Dictionary<ulong, StateInfo> States);

public record BlockInfo(string Name, ulong Hash, List<string> ValidStates);

public enum StateType
{
    Bool,
    Int,
    Enum
}

public record StateInfo(string Name, [property:JsonConverter(typeof(JsonStringEnumConverter))] StateType Type, ulong Hash, List<string> Values);

public class SerializedBlockInfo
{
    public string Name { get; set; }
    public ulong Hash { get; set; }
    public Dictionary<string, byte> States { get; set; }
    public bool Custom { get; set; }

    public SerializedBlockInfo(string name, ulong hash, Dictionary<string, byte> states, bool custom = false)
    {
        Name = name;
        Hash = hash;
        States = states;
        Custom = custom;
    }
}

public class StructureEntity
{
    public string Archetype { get; set; } = string.Empty;
    public float[] Position { get; set; } = new float[3];
    public float Yaw { get; set; }
}

[JsonSerializable(typeof(StateType))]
[JsonSerializable(typeof(HashInfo))]
[JsonSerializable(typeof(BlockInfo))]
[JsonSerializable(typeof(StateInfo))]
internal partial class JsonSerializers : JsonSerializerContext
{

}