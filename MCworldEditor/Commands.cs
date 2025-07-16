using Microsoft.VisualBasic.FileIO;
using System.CommandLine;
using System.Numerics;

namespace MCworldEditor
{
    public class Commands
    {
        private InventoryCommands inventoryCommands = new();
        private ChunkCommands chunkCommands = new();

        private RootCommand rootCommand;
        private Argument<int> _worldNumberArgument;

        public Commands()
        {
            rootCommand = new($"CLI for editing Minecraft beta worlds.\nKeep your world in default directory for it to work ( {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves")} )");
            _worldNumberArgument = CreateWorldNumberArgument();
        }

        public void PrepareCommands(string[] args)
        {
            AddInventoryCommands();
            AddChunkCommands();
        }

        public int CallCommand(string[] args)
        {
            return rootCommand.Parse(args).Invoke();
        }

        public void AddChunkCommands()
        {
            Command chunkCommand = new("chunk", "Allows to operate on chunks");

            rootCommand.Subcommands.Add(chunkCommand);

            Argument<int> searchForBlockIdArgument = new("blockId") { 
                Description = "ID of the block to search for."
            };

            Option<int> xOption = new("-x")
            {
                Description = "In-world X coordinates",
                Required = true
            };

            Option<int> zOption = new("-z")
            {
                Description = "In-world Z coordinates",
                Required = true
            };

            Command searchForBlockCommand = new("count", "Searches for block with specified id on chunk with provided coordinates") { _worldNumberArgument, searchForBlockIdArgument, xOption, zOption };
            searchForBlockCommand.SetAction(arguments=>chunkCommands.CountBlocksOnChunk(arguments.GetValue(_worldNumberArgument), arguments.GetValue(searchForBlockIdArgument), arguments.GetValue(xOption), arguments.GetValue(zOption)));
            chunkCommand.Subcommands.Add(searchForBlockCommand);
        }

        public void AddInventoryCommands()
        {
            Command inventoryCommand = new("inventory", "Handles operations related to player's inventory.");

            rootCommand.Subcommands.Add(inventoryCommand);

            Argument<int> addItemArgument = new("itemId")
            {
                Description = "Id of item"
            };

            Option<int> addItemCountOption = new("--count")
            {
                DefaultValueFactory = count => 1,
                Description = "Number of items to add"
            };
            addItemCountOption.Aliases.Add("--amount");

            Command addItemCommand = new("add", "Adds item with specified id to inventory") { _worldNumberArgument, addItemArgument };
            addItemCommand.Options.Add(addItemCountOption);
            inventoryCommand.Subcommands.Add(addItemCommand);
            addItemCommand.SetAction(arguments => inventoryCommands.AddItemToInventory(arguments.GetValue(_worldNumberArgument), arguments.GetValue(addItemArgument), arguments.GetValue(addItemCountOption)));

            Command clearInventoryCommand = new("clear", "Clears inventory") { _worldNumberArgument };
            clearInventoryCommand.Aliases.Add("clean");
            inventoryCommand.Subcommands.Add(clearInventoryCommand);
            clearInventoryCommand.SetAction(arguments => inventoryCommands.ClearInventory(arguments.GetValue(_worldNumberArgument)));
        }

        private Argument<int> CreateWorldNumberArgument()
        {
            Argument<int> worldNumberArgument = new("worldNumber")
            {
                Description = "Number of world (1-5)"
            };

            worldNumberArgument.Validators.Add(result =>
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

            return worldNumberArgument;
        }
    }
}
