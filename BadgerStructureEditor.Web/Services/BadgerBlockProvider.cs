using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text;
using BadgerSerialization.Models;
using BadgerStructureEditor.Web.Model;

namespace BadgerStructureEditor.Web.Services;

public class BadgerBlockProvider : IBlockProvider
{
    [MemberNotNullWhen(true, nameof(BlockHashInfo))]
    public bool Initialized { get; private set; }
    private HashInfo BlockHashInfo { get; set; }

    private readonly HttpClient _client;
    private Dictionary<string, BlockInfo> _blockNameLookup;
    private Dictionary<string, StateInfo> _stateNameLookup;

    public BadgerBlockProvider(HttpClient client)
    {
        BlockHashInfo = new HashInfo(new Dictionary<ulong, BlockInfo>(), new Dictionary<ulong, StateInfo>());
        _blockNameLookup = new Dictionary<string, BlockInfo>();
        _stateNameLookup = new Dictionary<string, StateInfo>();
        _client = client;
    }

    public async Task Initialize()
    {
        var hashInfo = await _client.GetFromJsonAsync<HashInfo>("hash_info.json");
        if (hashInfo != null)
        {
            BlockHashInfo = hashInfo;
            _blockNameLookup = hashInfo.Blocks.ToDictionary(x => x.Value.Name, x => x.Value);
            _stateNameLookup = hashInfo.States.ToDictionary(x => x.Value.Name, x => x.Value);
            Initialized = true;
        }
    }

    public SerializedBlockInfo ConvertBlock(SerializedBlock block)
    {
        if (!Initialized)
            throw new InvalidDataException("Not initialized.");

        if (!BlockHashInfo.Blocks.TryGetValue(block.NameHash, out var blockInfo))
            throw new InvalidDataException($"Could not find block for hash {block.NameHash}");

        var states = new Dictionary<string, byte>();

        foreach (var statePair in block.States)
        {
            if (!BlockHashInfo.States.TryGetValue(statePair.Key, out var state))
                throw new InvalidDataException($"Could not find state for hash {statePair.Key} in block {blockInfo.Name}");

            states[state.Name] = statePair.Value;
        }

        return new SerializedBlockInfo(blockInfo.Name, block.NameHash, states);
    }

    public SerializedBlockInfo ConvertBlock(Dictionary<string, dynamic> block)
    {
        var blockName = Encoding.UTF8.GetString(block["name"]);
        return new SerializedBlockInfo(blockName,
            CalculateHash(blockName),
            ((Dictionary<string, dynamic>) block["states"])
            .Where(x => x.Key != "block_automata_type" && x.Key != "block_automata_can_change" && x.Value.GetType() != typeof(byte[]))
            .ToDictionary(x => x.Key, x => (byte)x.Value));
    }

    public SerializedBlockInfo Create(string name)
        => new(name,
            CalculateHash(name),
            _stateNameLookup
                .Where(x => x.Key != "block_automata_type" && x.Key != "block_automata_can_change")
                .ToDictionary(x => x.Value.Name, _ => (byte) 0),
            !_blockNameLookup.ContainsKey(name));

    public BlockInfo? GetBlock(string name)
        => _blockNameLookup.GetValueOrDefault(name);

    public StateInfo? GetState(string name)
        => _stateNameLookup.GetValueOrDefault(name);


    private static ulong CalculateHash(string name)
    {
        var hashed = name.Contains(':')
            ? name[(name.IndexOf(":", StringComparison.Ordinal) + 1)..]
            : name;

        return Encoding.UTF8.GetBytes(hashed).Aggregate(0xCBF29CE484222325uL, (current, byt) => byt ^ (0x100000001B3 * current));
    }
}