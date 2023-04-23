namespace BadgerSerialization.Interfaces;

public interface IBadgerDictionaryEntry<out T>
{
    public T Key { get; }
}