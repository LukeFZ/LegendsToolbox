using System.Diagnostics;
using BadgerSerialization.Types;
using fNbt;
using LegendsToolbox.Core.RuntimeData;
using MiNET.LevelDB;

namespace LegendsToolbox.Core;

public class World
{
    public string Name { get; set; }
    public NbtFile LevelDat { get; private set; } = null!;
    public Database WorldDatabase { get; private set; } = null!;

    private readonly string _worldPath;

    private const string LevelNameFile = "levelname.txt";
    private const string LevelDatFile = "level.dat";
    private const string WorldFolder = "db";

    public World(string worldPath)
    {
        _worldPath = worldPath;
        Name = string.Empty;
    }

    public async Task LoadAsync()
    {
        await LoadName();
        await LoadLevelDat();
        LoadWorldDatabase();

        //var localPlayer = WorldDatabase.GetNbtEntry("~local_player"u8.ToArray());
        var savedEntityData = WorldDatabase.GetEntry("SavedEntityData"u8.ToArray());
        var autonomousEntities = WorldDatabase.GetNbtEntry("AutonomousEntities"u8.ToArray());

        var entityData = BadgerCompound.Load(savedEntityData);
        var serializedEntityData = entityData.Save();
        File.WriteAllBytes("original_entitydata", savedEntityData);
        File.WriteAllBytes("serialized_entitydata", serializedEntityData);
        Debug.Assert(savedEntityData.SequenceEqual(serializedEntityData), "savedEntityData.SequenceEqual(serializedEntityData)");
        //var b = new BadgerSyncedGameStartRuntimeData(WorldDatabase);
        var a = new BSharpRuntimeData(WorldDatabase);
        //var serialb = a.Data?.Save();
        //File.WriteAllBytes("bsharp", serialb);
        var aa = new CinematicsRuntimeData(WorldDatabase);
        var aaa = new DeckRuntimeData(WorldDatabase);
        var aaaa = new EntityFactorySetupData(WorldDatabase);
        var aaaaa = new GeologyRuntimeData(WorldDatabase);
        var aaaaaa = new InvasionRuntimeData(WorldDatabase);
        var aaaaaaa = new MapRevealRuntimeData(WorldDatabase);
        var aaaaaaaa = new ServerMapRuntimeData(WorldDatabase);
        var aaaaaaaaa = new VillageRuntimeData(WorldDatabase);
        var aaaaaaaaaa = new WorldFeatureRuntimeData(WorldDatabase);
        var aaaaaaaaaaa = new WorldGenerationRuntimeData(WorldDatabase);
        var aaaaaaaaaaaa = new WorldStreamRuntimeData(WorldDatabase);
    }

    private async Task LoadName()
    {
        var levelNamePath = Path.Join(_worldPath, LevelNameFile);
        if (!File.Exists(levelNamePath))
            throw new FileNotFoundException("Could not find levelname.txt file in world directory.");

        Name = await File.ReadAllTextAsync(levelNamePath);
    }

    private async Task LoadLevelDat()
    {
        var levelDatPath = Path.Join(_worldPath, LevelDatFile);
        if (!File.Exists(levelDatPath))
            throw new FileNotFoundException("Could not find level.dat file in world directory.");

        await using var levelDatStream = File.OpenRead(levelDatPath);
        levelDatStream.Seek(8, SeekOrigin.Begin);
        LevelDat = new NbtFile
        {
            BigEndian = false,
            UseVarInt = false
        };
        LevelDat.LoadFromStream(levelDatStream, NbtCompression.AutoDetect);
    }

    private void LoadWorldDatabase()
    {
        var worldDirInfo = new DirectoryInfo(Path.Combine(_worldPath, WorldFolder));
        if (!worldDirInfo.Exists)
            throw new DirectoryNotFoundException("Could not find world database in world directory.");

        WorldDatabase = new Database(worldDirInfo);
        WorldDatabase.Open();
    }
}