using MiNET.LevelDB;
using BadgerSerialization.Core;
using BadgerSerialization.Core.Attributes;
using BadgerSerialization.Interfaces;
using LegendsToolbox.Core.Game.RuntimeData;

namespace LegendsToolbox.Core.RuntimeData;

public class DeckRuntimeData : GameLayerRuntimeData
{
    private readonly BadgerDeckRuntimeData? _badgerData;

    public DeckRuntimeData(Database db) : base(db, nameof(DeckRuntimeData))
    {
        if (Data != null)
        {
            _badgerData = BadgerDeckRuntimeData.Deserialize(Data.Value);
            var b = _badgerData!.SerializeToCompound().Save();
        }
    }
}

[BadgerObject]
public partial class BadgerDeckRuntimeData
{
    [BadgerProperty("mStoredDecks", BadgerObjectType.Dictionary)]
    public Dictionary<string, BadgerDeck> StoredDecks { get; set; }

    [BadgerProperty("mDiscardedDecks", BadgerObjectType.Dictionary)]
    public Dictionary<string, BadgerDeck> DiscardedDecks { get; set; }

    [BadgerProperty("maw_patched", BadgerObjectType.Boolean)]
    public bool MawPatched { get; set; }

    [BadgerProperty("mLibraryIndices", BadgerObjectType.Dictionary)]
    public Dictionary<string, BadgerLibraryIndex> LibraryIndices { get; set; }

    [BadgerProperty("mCardIndices", BadgerObjectType.Dictionary)]
    public Dictionary<string, BadgerCardIndex> CardIndices { get; set; }
}

[BadgerObject]
public partial class BadgerLibraryIndex : IBadgerDictionaryEntry<string>
{
    [BadgerProperty("library_name", BadgerObjectType.DictionaryKey)]
    public string Name { get; set; }

    [BadgerProperty("library_index", BadgerObjectType.VarUInt)]
    public uint Index { get; set; }

    public string Key => Name;
}

[BadgerObject]
public partial class BadgerCardIndex : IBadgerDictionaryEntry<string>
{
    [BadgerProperty("card_id", BadgerObjectType.DictionaryKey)]
    public string Id { get; set; }

    [BadgerProperty("card_index", BadgerObjectType.VarUInt)]
    public uint Index { get; set; }

    public string Key => Id;
}

[BadgerObject]
public partial class BadgerDeck : IBadgerDictionaryEntry<string>
{
    [BadgerProperty("deckName", BadgerObjectType.DictionaryKey)]
    public string Name { get; set; }

    [BadgerProperty("cards", BadgerObjectType.List)]
    public List<BadgerCard> Cards { get; set; }

    public string Key => Name;
}

[BadgerObject]
public partial class BadgerCard
{
    [BadgerProperty("subcards", BadgerObjectType.List)]
    public List<BadgerSubcard> Subcards { get; set; }
}

[BadgerObject]
public partial class BadgerSubcard
{
    [BadgerProperty("library_index", BadgerObjectType.Integer)]
    public int LibraryIndex { get; set; }

    [BadgerProperty("card_index", BadgerObjectType.Integer)]
    public int CardIndex { get; set; }
}