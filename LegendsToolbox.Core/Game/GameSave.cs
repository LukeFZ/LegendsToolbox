namespace LegendsToolbox.Core.Game;

public class GameSave
{
    public List<World> Worlds { get; }
    public string FullPath => _saveDirectory.FullName;

    private readonly DirectoryInfo _saveDirectory;

    private const string WorldsFolder = "minecraftWorlds";
    private const string TutorialWorld = "AssetViewerFlatland";

    public GameSave(DirectoryInfo saveDirectory)
    {
        _saveDirectory = saveDirectory;
        Worlds = new List<World>();
    }

    public async Task LoadAsync()
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
}