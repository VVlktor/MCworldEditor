using MCworldEditor.CommandsProvider;
using System.CommandLine;

namespace MCworldEditor
{
    public class Commands
    {
        private RootCommand rootCommand;
        Option<int> worldOption;

        public Commands(InventoryCommandsProvider inventoryCommandsProvider, PlayerCommandsProvider playerCommandsProvider, WorldCommandsProvider worldCommandsProvider)
        {
            rootCommand = new($"CLI for editing Minecraft beta worlds.\nKeep your world in default directory for it to work ( {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves")} )");
            worldOption = CreateWorldOption();
            rootCommand.Options.Add(worldOption);
            inventoryCommandsProvider.Register(rootCommand, worldOption);
            playerCommandsProvider.Register(rootCommand, worldOption);
            worldCommandsProvider.Register(rootCommand, worldOption);
        }

        public int CallCommand(string[] args)
        {
            return rootCommand.Parse(args).Invoke();
        }


        private Option<int> CreateWorldOption()
        {
            worldOption = new("--world")
            {
                Required = true,
                Description = "Number of world (1-5)",
                Recursive = true,
            };
            worldOption.Validators.Add(result =>
            {
                if (result.Tokens.Count == 0)
                {
                    result.AddError("World number is required.");
                    return;
                }

                if (!int.TryParse(result.Tokens[0].Value, out int worldNumber))
                {
                    result.AddError("World number must be a valid integer.");
                    return;
                }

                if (worldNumber > 5 || worldNumber < 1)
                {
                    result.AddError("There are only 5 world slots in minecraft beta.");
                    return;
                }

                string worldPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldNumber}");

                if (!Directory.Exists(worldPath))
                    result.AddError("World with specified number does not exist.");

            });
            return worldOption;
        }
    }
}
