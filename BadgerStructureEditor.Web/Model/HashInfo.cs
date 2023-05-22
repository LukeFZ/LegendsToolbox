using System.Numerics;
using System.Text.Json.Serialization;
using BadgerSerialization.Types;

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
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, byte> States { get; set; } = new();

    public SerializedBlockInfo(string name, Dictionary<string, byte> states)
    {
        Name = name;
        States = states;
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