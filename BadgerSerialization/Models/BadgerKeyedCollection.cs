using BadgerSerialization.Interfaces;
using System.Collections.ObjectModel;

namespace BadgerSerialization.Models;

public class BadgerKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem> 
    where TKey : notnull
    where TItem : IBadgerDictionaryEntry<TKey>
{
    protected override TKey GetKeyForItem(TItem item)
        => item.Key;
}