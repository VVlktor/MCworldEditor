﻿using MCworldEditor.CommandsProvider;
using MCworldEditor.CommandsToCall;
using MCworldEditor.Services;
using MCworldEditor.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MCworldEditor
{
    internal class Program
    {
        static int Main(string[] args)
        {
            ServiceCollection builder = new();
            builder.AddTransient<CommandProvider>();
            
            builder.AddTransient<InventoryCommands>();
            builder.AddTransient<InventoryCommandsProvider>();

            builder.AddTransient<PlayerCommands>();
            builder.AddTransient<PlayerCommandsProvider>();

            builder.AddTransient<WorldCommands>();
            builder.AddTransient<WorldCommandsProvider>();

            builder.AddTransient<ISeedService, SeedService>();
            builder.AddTransient<IChunkService, ChunkService>();
            builder.AddTransient<IFileService, FileService>();
            builder.AddTransient<IPlayerPositionService, PlayerPositionService>();
            builder.AddTransient<ITimeService, TimeService>();
            builder.AddTransient<IInventoryService, InventoryService>();
            builder.AddTransient<ISeedService, SeedService>();
            builder.AddTransient<ISpawnService, SpawnService>();
            builder.AddTransient<IHealthService, HealthService>();

            ServiceProvider services = builder.BuildServiceProvider();
            CommandProvider commands = services.GetRequiredService<CommandProvider>();
            return commands.CallCommand(args);
        }
    }
}
















































//    Option<FileInfo> fileOption = new("--file")
//    {
//        Description = "The file to read and display on the console."
//    };

//    Option<int> delayOption = new("--delay")
//    {
//        Description = "Delay between lines, specified as milliseconds per character in a line.",
//        DefaultValueFactory = parseResult => 42
//    };
//    Option<ConsoleColor> fgcolorOption = new("--fgcolor")
//    {
//        Description = "Foreground color of text displayed on the console.",
//        DefaultValueFactory = parseResult => ConsoleColor.White
//    };
//    Option<bool> lightModeOption = new("--light-mode")
//    {
//        Description = "Background color of text displayed on the console: default is black, light mode is white."
//    };

//    RootCommand rootCommand = new("Sample app for System.CommandLine");

//    Command readCommand = new("read", "Read and display the file.")
//    {
//        fileOption,
//        delayOption,
//        fgcolorOption,
//        lightModeOption
//    };

//    rootCommand.Subcommands.Add(readCommand);

//    readCommand.SetAction(parseResult => ReadFile(
//        parseResult.GetValue(fileOption),
//        parseResult.GetValue(delayOption),
//        parseResult.GetValue(fgcolorOption),
//        parseResult.GetValue(lightModeOption)));

//    ParseResult parseResult = rootCommand.Parse(args);
//    return parseResult.Invoke();
//}

//internal static void ReadFile(FileInfo file, int delay, ConsoleColor fgColor, bool lightMode)
//{
//    Console.BackgroundColor = lightMode ? ConsoleColor.White : ConsoleColor.Black;
//    Console.ForegroundColor = fgColor;
//    foreach (string line in File.ReadLines(file.FullName))
//    {
//        Console.WriteLine(line);
//        Thread.Sleep(TimeSpan.FromMilliseconds(delay * line.Length));
//    }