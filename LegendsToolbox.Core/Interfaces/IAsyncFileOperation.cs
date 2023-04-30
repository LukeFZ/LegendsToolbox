namespace LegendsToolbox.Core.Interfaces;

public interface IAsyncObject
{
    public Task SaveAsync();
    public Task LoadAsync();
}