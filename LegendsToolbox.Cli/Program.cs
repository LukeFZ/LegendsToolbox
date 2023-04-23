using LegendsToolbox.Core;

namespace LegendsToolbox.Cli;

internal class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Length != 1)
        {
            PrintHelp();
            return;
        }

        var savePath = args[0];
        SaveData save;
        if (File.Exists(Path.Join(savePath, "containers.index")))
            save = await SaveData.LoadFromContainerAsync(savePath);
        else
            save = await SaveData.LoadFromDirectoryAsync(savePath);

        foreach (var world in save.Worlds) Console.WriteLine($"World Name: {world.Name} | Seed: {world.LevelDat.RootTag["RandomSeed"].LongValue}");
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Usage: LegendsToolbox.Cli.exe <save path>");
    }
}