using System.CommandLine;

namespace MCworldEditor
{
    public class Commands
    {
        private InventoryCommands inventoryCommands = new();
        private ChunkCommands chunkCommands = new();
        PlayerCommands playerCommands = new();

        private RootCommand rootCommand;
        private Argument<int> _worldNumberArgument;

        public Commands()
        {
            rootCommand = new($"CLI for editing Minecraft beta worlds.\nKeep your world in default directory for it to work ( {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves")} )");
            _worldNumberArgument = CreateWorldNumberArgument();
            AddInventoryCommands();
            AddChunkCommands();
            AddPlayerCommands();
        }

        public int CallCommand(string[] args)
        {
            return rootCommand.Parse(args).Invoke();
        }

        private void AddPlayerCommands()
        {
            Command playerCommand = new("player", "Commands realted to player.");
            rootCommand.Subcommands.Add(playerCommand);

            Command positionCommand = new("position", "Operations on player's position");
            playerCommand.Subcommands.Add(positionCommand);

            Command changePosition = new("change", "Changes position of player to specified one");//TODO dodac akcje, A TAKZE opcje/flage zeby nie moglo przeniesc jesli są w tym miejscu bloki
            changePosition.Aliases.Add("move");
            positionCommand.Subcommands.Add(changePosition);

            Option<bool> specificReadPosition = new("--specific") { 
                Description = "Defines wheather coordinates should be rounded."
            };
            specificReadPosition.Aliases.Add("--exact");
            specificReadPosition.Aliases.Add("--not-rounded");

            Command readPosition = new("read", "Reads position of player and displays it") { _worldNumberArgument, specificReadPosition };
            readPosition.Aliases.Add("check");
            positionCommand.Subcommands.Add(readPosition);
            readPosition.SetAction(arguments => playerCommands.ReadPosition(arguments.GetValue(_worldNumberArgument), arguments.GetValue(specificReadPosition)));
        }

        private void AddChunkCommands()
        {
            Command chunkCommand = new("chunk", "Allows to operate on chunks");

            rootCommand.Subcommands.Add(chunkCommand);

            Argument<int> searchForBlockIdArgument = new("blockId")
            {
                Description = "ID of the block to search for."
            };

            Option<int> xOption = new("-x")
            {
                Description = "In-world X coordinates. If not specified player's position will be used."
            };

            Option<int> zOption = new("-z")
            {
                Description = "In-world Z coordinates. If not specified player's position will be used."
            };

            Command searchForBlockCommand = new("count", "Searches for block with specified id on chunk with provided coordinates") { _worldNumberArgument, searchForBlockIdArgument, xOption, zOption };
            searchForBlockCommand.SetAction(arguments => chunkCommands.CountBlocksOnChunk(arguments.GetValue(_worldNumberArgument), arguments.GetValue(searchForBlockIdArgument), arguments.GetValue(xOption), arguments.GetValue(zOption)));
            chunkCommand.Subcommands.Add(searchForBlockCommand);

            Command chunkDimensionsCommand = new("dimensions", "Displays the X, Y, Z coordinates of the chunk corners for the given position.") { xOption, zOption };
            chunkDimensionsCommand.Aliases.Add("corners");
            chunkDimensionsCommand.SetAction(arguments => chunkCommands.ChunkDimensions(arguments.GetValue(xOption), arguments.GetValue(zOption)));
            chunkCommand.Subcommands.Add(chunkDimensionsCommand);
        }

        private void AddInventoryCommands()
        {
            Command inventoryCommand = new("inventory", "Handles operations related to player's inventory.");

            rootCommand.Subcommands.Add(inventoryCommand);

            Argument<int> itemIdArgument = new("itemId")
            {
                Description = "Id of item"
            };

            Option<int> addItemCountOption = new("--count")
            {
                DefaultValueFactory = count => 1,
                Description = "Number of items to add (max 127 items)",
            };
            addItemCountOption.Aliases.Add("--amount");

            Command addItemCommand = new("add", "Adds item with specified id to inventory") { _worldNumberArgument, itemIdArgument };
            addItemCommand.Options.Add(addItemCountOption);
            inventoryCommand.Subcommands.Add(addItemCommand);
            addItemCommand.SetAction(arguments => inventoryCommands.AddItemToInventory(arguments.GetValue(_worldNumberArgument), arguments.GetValue(itemIdArgument), arguments.GetValue(addItemCountOption)));

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
