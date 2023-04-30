namespace LegendsToolbox.Core.Interfaces;

public interface ISaveDataProvider
{
    public string Name { get; }

    public Task Initialize();
    public bool HasSaveData();
    public string[] GetAllUsers();
    public ISaveData GetSaveDataForUser(string user);
    public Task DeleteAllSaveDataAsync();
    public Task DeleteSaveDataForUserAsync(string user);
}