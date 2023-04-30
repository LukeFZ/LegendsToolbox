using LegendsToolbox.Core.Interfaces;
using Microsoft.Win32;

namespace LegendsToolbox.SaveProviders.Xbox;

public class XboxSaveDataProvider : ISaveDataProvider
{
    public string Name => "Xbox";

    private string _currentUserName;
    private XboxSaveData? _saveData;

    public XboxSaveDataProvider()
    {
        _currentUserName = "";
    }

    public async Task Initialize()
    {
        if (!OperatingSystem.IsWindows())
            return;

        var xblRegKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\XboxLive");
        if (xblRegKey?.GetValue("Xuid") is not string xuid || 
            xblRegKey.GetValue("Gamertag") is not string gamertag)
            return;

        _currentUserName = gamertag;
        _saveData = new XboxSaveData(ulong.Parse(xuid));
    }

    public bool HasSaveData() => _saveData != null;

    public string[] GetAllUsers()
    {
        return _saveData == null ? Array.Empty<string>() : new[] {_currentUserName};
    }

    public ISaveData GetSaveDataForUser(string user)
    {
        if (_saveData == null)
            throw new InvalidOperationException("No save data available.");

        if (user != _currentUserName)
            throw new ArgumentException($"User {user} has no Xbox save data.");

        return _saveData;
    }

    public async Task DeleteAllSaveDataAsync()
    {
        if (_saveData == null)
            throw new InvalidOperationException("No save data available.");

        _saveData.Delete();
        await _saveData.SaveAsync();
    }

    public async Task DeleteSaveDataForUserAsync(string user)
    {
        if (_saveData == null)
            throw new InvalidOperationException("No save data available.");

        if (user != _currentUserName)
            throw new ArgumentException($"User {user} has no Xbox save data.");

        _saveData.Delete();
        await _saveData.SaveAsync();
    }
}