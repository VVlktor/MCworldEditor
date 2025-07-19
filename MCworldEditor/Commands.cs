using MCworldEditor.CommandsToCall;
using System.CommandLine;

namespace MCworldEditor
{
    public class Commands
    {
        private InventoryCommands inventoryCommands = new();
        private WorldCommands worldCommands = new();
        private PlayerCommands playerCommands = new();

        private RootCommand rootCommand;
        Option<int> worldOption;

        public Commands()
        {
            rootCommand = new($"CLI for editing Minecraft beta worlds.\nKeep your world in default directory for it to work ( {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves")} )");
            worldOption = CreateWorldOption();
            rootCommand.Options.Add(worldOption);
            AddInventoryCommands();
            AddWorldCommands();
            AddPlayerCommands();
        }

        public int CallCommand(string[] args)
        {
            return rootCommand.Parse(args).Invoke();
        }

        private void AddPlayerCommands()//komende na zmiane spawn x y z, komende na zmiane hp, komende na falldistance
        {
            Command playerCommand = new("player", "Commands realted to player.");
            rootCommand.Subcommands.Add(playerCommand);

            Command positionCommand = new("position", "Operations on player's position");
            playerCommand.Subcommands.Add(positionCommand);

            Argument<int> xPositionArgument = new("xPosition")
            {
                Description = "In-world X coordinates.",
            };
            Argument<int> yPositionArgument = new("yPosition")
            {
                Description = "In-world Y coordinates.",
            };
            Argument<int> zPositionArgument = new("zPosition")
            {
                Description = "In-world Z coordinates.",
            };

            Option<bool> safeNewPosition = new("--safe")
            {
                Description = "Command wont take effect if specified location is inside of a block."
            };

            Command changePosition = new("change", "Changes position of player to specified one") { xPositionArgument, yPositionArgument, zPositionArgument, safeNewPosition };
            changePosition.Aliases.Add("move");
            positionCommand.Subcommands.Add(changePosition);
            changePosition.SetAction(arguments => playerCommands.ChangePlayerPosition(arguments.GetValue(worldOption), arguments.GetValue(xPositionArgument), arguments.GetValue(yPositionArgument), arguments.GetValue(zPositionArgument), arguments.GetValue(safeNewPosition)));
            //implement ChangePlayerPosition (i ChangeSpawnPosition czy cos)
            Option<bool> specificReadPosition = new("--specific") { 
                Description = "Defines wheather coordinates should be rounded."
            };
            specificReadPosition.Aliases.Add("--exact");
            specificReadPosition.Aliases.Add("--not-rounded");

            Command readPosition = new("read", "Reads position of player and displays it") { specificReadPosition };
            readPosition.Aliases.Add("check");
            positionCommand.Subcommands.Add(readPosition);
            readPosition.SetAction(arguments => playerCommands.ReadPosition(arguments.GetValue(worldOption), arguments.GetValue(specificReadPosition)));

            Command spawnCommand = new("spawn", "Commands related to spawn coordinates.");
            playerCommand.Subcommands.Add(spawnCommand);

            Command readSpawn = new("read", "Displays coordinates of spawn.");
            spawnCommand.Subcommands.Add(readSpawn);
            readSpawn.Aliases.Add("check");
            readSpawn.SetAction(arguments => playerCommands.ReadSpawn(arguments.GetValue(worldOption)));
        }

        private void AddWorldCommands()//dodac komende na podejrzenie seeda, 
        {
            Command worldCommand = new("chunk", "Allows to operate on chunks");

            rootCommand.Subcommands.Add(worldCommand);

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

            Command searchForBlockCommand = new("count", "Searches for block with specified id on chunk with provided coordinates") { searchForBlockIdArgument, xOption, zOption };
            searchForBlockCommand.SetAction(arguments => worldCommands.CountBlocksOnChunk(arguments.GetValue(worldOption), arguments.GetValue(searchForBlockIdArgument), arguments.GetValue(xOption), arguments.GetValue(zOption)));
            worldCommand.Subcommands.Add(searchForBlockCommand);

            Command chunkDimensionsCommand = new("dimensions", "Displays the X, Y, Z coordinates of the chunk corners for the given position.") { xOption, zOption };
            chunkDimensionsCommand.Aliases.Add("corners");
            chunkDimensionsCommand.SetAction(arguments => worldCommands.ChunkDimensions(arguments.GetValue(xOption), arguments.GetValue(zOption)));
            worldCommand.Subcommands.Add(chunkDimensionsCommand);
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
            addItemCountOption.Validators.Add(result =>
            {
                if (result.Tokens.Count != 1)
                {
                    result.AddError("Number for amount is required.");
                    return;
                }
                if (!int.TryParse(result.Tokens[0].Value, out int amountNumber))
                {
                    result.AddError("Number is required.");
                    return;
                }
                if (amountNumber<1)
                {
                    result.AddError("Amount is too small.");
                    return;
                }
            });

            Command addItemCommand = new("add", "Adds item with specified id to inventory") { itemIdArgument };
            addItemCommand.Options.Add(addItemCountOption);
            inventoryCommand.Subcommands.Add(addItemCommand);
            addItemCommand.SetAction(arguments => inventoryCommands.AddItemToInventory(arguments.GetValue(worldOption), arguments.GetValue(itemIdArgument), arguments.GetValue(addItemCountOption)));

            Command clearInventoryCommand = new("clear", "Clears inventory");
            clearInventoryCommand.Aliases.Add("clean");
            inventoryCommand.Subcommands.Add(clearInventoryCommand);
            clearInventoryCommand.SetAction(arguments => inventoryCommands.ClearInventory(arguments.GetValue(worldOption)));
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
