using System.Diagnostics;
using BadgerSerialization.Types;
using fNbt;
using LegendsToolbox.Core.Game.RuntimeData;
using LegendsToolbox.Core.RuntimeData;
using MiNET.LevelDB;

namespace LegendsToolbox.Core.Game;

public class World
{
    public string Name { get; set; }
    public NbtFile LevelDat { get; private set; } = null!;

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

        var worldDirInfo = new DirectoryInfo(Path.Combine(_worldPath, WorldFolder));
        if (!worldDirInfo.Exists)
            throw new DirectoryNotFoundException("Could not find world database in world directory.");

        using var database = new Database(worldDirInfo);
        database.Open();

        //var localPlayer = database.GetNbtEntry("~local_player"u8.ToArray());
        var savedEntityData = database.GetEntry("SavedEntityData"u8.ToArray());
        var autonomousEntities = database.GetNbtEntry("AutonomousEntities"u8.ToArray());

        var entityData = BadgerCompound.Load(savedEntityData);
        /*var serializedEntityData = entityData.Save();
        File.WriteAllBytes("original_entitydata", savedEntityData);
        File.WriteAllBytes("serialized_entitydata", serializedEntityData);
        Debug.Assert(savedEntityData.SequenceEqual(serializedEntityData), "savedEntityData.SequenceEqual(serializedEntityData)");*/
        var b = new BadgerSyncedGameStartRuntimeData(database);
        var a = new BSharpRuntimeData(database);
        var aa = new CinematicsRuntimeData(database);
        var aaa = new DeckRuntimeData(database);
        var aaaa = new EntityFactorySetupData(database);
        var aaaaa = new GeologyRuntimeData(database);
        var aaaaaa = new InvasionRuntimeData(database);
        var aaaaaaa = new MapRevealRuntimeData(database);
        var aaaaaaaa = new ServerMapRuntimeData(database);
        var aaaaaaaaa = new VillageRuntimeData(database);
        var aaaaaaaaaa = new WorldFeatureRuntimeData(database);
        var aaaaaaaaaaa = new WorldGenerationRuntimeData(database);
        var aaaaaaaaaaaa = new WorldStreamRuntimeData(database);
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
}