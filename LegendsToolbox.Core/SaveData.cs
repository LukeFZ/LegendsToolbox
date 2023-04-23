namespace LegendsToolbox.Core;

public class SaveData
{
    public List<World> Worlds { get; }
    public string FullPath => _saveDirectory.FullName;

    private readonly DirectoryInfo _saveDirectory;

    private const string WorldsFolder = "minecraftWorlds";
    private const string TutorialWorld = "AssetViewerFlatland";

    private SaveData(DirectoryInfo saveDirectory)
    {
        _saveDirectory = saveDirectory;
        Worlds = new List<World>();
    }

    private async Task LoadAsync()
    {
        var path = FullPath;

        var worldsPath = Path.Join(path, WorldsFolder);
        if (!Directory.Exists(worldsPath))
            throw new DirectoryNotFoundException("Could not find worlds directory in save data folder.");

        foreach (var worldPath in Directory.EnumerateDirectories(worldsPath))
        {
            if (worldPath.EndsWith(TutorialWorld)) continue;
            try
            {
                var world = new World(worldPath);
                await world.LoadAsync();
                Worlds.Add(world);
            }
            catch (Exception _)
            {
                // ignored
            }
        }
    }

    public static async Task<SaveData> LoadFromContainerAsync(string containerPath)
    {
        var containerInterface = await ContainerInterface.Load(containerPath);
        var save = new SaveData(containerInterface.GetSaveDirectory());
        await save.LoadAsync();

        return save;
    }

    public static async Task<SaveData> LoadFromDirectoryAsync(string saveDirectory)
    {
        var save = new SaveData(new DirectoryInfo(saveDirectory));
        await save.LoadAsync();

        return save;
    }
}