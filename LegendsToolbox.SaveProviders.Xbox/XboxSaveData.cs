using System.Text;
using LegendsToolbox.Core.Game;
using LegendsToolbox.Core.Interfaces;
using LibXblContainer;

namespace LegendsToolbox.SaveProviders.Xbox;

internal class XboxSaveData : ISaveData
{
    private readonly ConnectedStorage _connectedStorage;
    private readonly DirectoryInfo _tempDirectory;
    private GameSave? _gameSave;

    private const string SaveDataPathSuffix = "Packages\\Microsoft.BadgerWin10_8wekyb3d8bbwe\\SystemAppData\\wgs";
    private const string LegendsScid = "00000000000000000000000061B215AE"; // 00000000-0000-0000-0000-000061B215AE

    public XboxSaveData(ulong xuid)
    {
        var xuidHex = Convert.ToHexString(BitConverter.GetBytes(xuid).Reverse().ToArray()).PadLeft(8, '0');
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var containerPath = Path.Combine(
            localAppData,
            SaveDataPathSuffix,
            $"{xuidHex}_{LegendsScid}"
        );

        _connectedStorage = new ConnectedStorage(containerPath);
        _tempDirectory = Directory.CreateTempSubdirectory("ltoolbox-xbox-");
    }

    public async Task<GameSave> GetGameSaveAsync()
    {
        _gameSave ??= await InitializeGameSave();
        return _gameSave;
    }

    public async Task SaveAsync()
    {
        if (_gameSave == null)
            return;

        var currentContainerList = _connectedStorage.Containers.ToList();
        foreach (var filePath in Directory.EnumerateFiles(_tempDirectory.FullName, "*", SearchOption.AllDirectories))
        {
            var file = filePath.Replace(_tempDirectory.FullName + Path.DirectorySeparatorChar, "").Replace('\\', '/');
            var container = _connectedStorage.Get(file, true);
            await using var fs = File.OpenRead(filePath);

            if (container == null)
            {
                container = _connectedStorage.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(file)), file);
                await container.AddAsync(null, fs);
            }
            else
            {
                currentContainerList.Remove(container);
                await container.UpdateAsync(fs);
            }

        }

        foreach (var removedContainer in currentContainerList)
            _connectedStorage.Remove(removedContainer);
    }

    public void Delete()
    {
        foreach (var container in _connectedStorage.Containers)
            _connectedStorage.Remove(container);
    }

    private async Task<GameSave> InitializeGameSave()
    {
        foreach (var container in _connectedStorage.Containers)
        {
            if (container.Blobs.Count > 1)
                throw new InvalidOperationException("Unexpected number of blob records, count > 1");

            var dest = Path.Join(_tempDirectory.FullName, container.MetaData.EntryName);
            if (container.MetaData.EntryName.Contains('/')) 
                Directory.CreateDirectory(Path.GetDirectoryName(dest)!);

            await using var fs = File.OpenWrite(dest);
            await using var blobStream = container.Open();
            await blobStream.CopyToAsync(fs);
        }

        var save = new GameSave(_tempDirectory);
        await save.LoadAsync();
        return save;
    }
}