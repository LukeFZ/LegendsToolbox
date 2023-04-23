using System.Diagnostics;
using LibXblContainer;

namespace LegendsToolbox.Core;

public class ContainerInterface
{
    private readonly string _containerPath;
    private readonly DirectoryInfo _tempDir;

    public ContainerInterface(string containerPath)
    {
        _containerPath = containerPath;
        _tempDir = Directory.CreateTempSubdirectory("legendstoolbox-");
    }

    public DirectoryInfo GetSaveDirectory() => _tempDir;

    public static async Task<ContainerInterface> Load(string xblSavePath)
    {
        var save = new ContainerInterface(xblSavePath);
        await save.LoadContainer();
        return save;
    }

    private async Task LoadContainer()
    {
        var storage = new ConnectedStorage(_containerPath);
        var tempDirPath = _tempDir.FullName;
            
        foreach (var container in storage.Containers)
        {
            Debug.Assert(container.Records.Count == 1, "Unexpected number of blob records, count > 1");

            var entryName = container.MetaData.EntryName;
            if (entryName.Contains('/'))
                Directory.CreateDirectory(Path.Join(tempDirPath, entryName[..entryName.LastIndexOf('/')]));

            await using var outFile = File.OpenWrite(Path.Join(tempDirPath, entryName));
            await using var inFile = container.Open();
            await inFile.CopyToAsync(outFile);
        }
    }

    public async Task Save()
    {
        var storage = new ConnectedStorage(_containerPath);
        var tempDirPath = _tempDir.FullName;

        foreach (var container in storage.Containers)
        {
            var entryName = container.MetaData.EntryName;
            var updatedFilePath = Path.Join(tempDirPath, entryName);

            await using var inFile = File.OpenRead(updatedFilePath);
            await container.UpdateAsync(inFile);
        }

        storage.Save();
    }
}