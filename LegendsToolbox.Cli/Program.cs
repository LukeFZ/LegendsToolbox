using LegendsToolbox.Core.Game;
using LegendsToolbox.SaveProviders.Xbox;

namespace LegendsToolbox.Cli;

internal class Program
{
    private static async Task Main(string[] args)
    {
        //if (args.Length != 1)
        //{
        //    PrintHelp();
        //    return;
        //}

        //var savePath = args[0];

        var saveProvider = new XboxSaveDataProvider();
        await saveProvider.Initialize();

        Console.WriteLine("Available save data:");
        var users = saveProvider.GetAllUsers().ToList();
        for (int i = 0; i < users.Count; i++)
            Console.WriteLine($"{i}\t{users[i]}");

        Console.Write("Select an account: ");
        var choice = int.Parse(Console.ReadLine()!);

        await Console.Out.WriteLineAsync("Retrieving save data..");
        var saveData = saveProvider.GetSaveDataForUser(users[choice]);
        await Console.Out.WriteLineAsync("Save data retrieved.");

        await Console.Out.WriteLineAsync("Retrieving game save..");
        var save = await saveData.GetGameSaveAsync();
        await Console.Out.WriteLineAsync("Game save retrieved.");

        foreach (var world in save.Worlds) Console.WriteLine($"World Name: {world.Name} | Seed: {world.LevelDat.RootTag["RandomSeed"].LongValue}");
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Usage: LegendsToolbox.Cli.exe <save path>");
    }
}