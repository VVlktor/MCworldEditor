using System.CommandLine;

namespace MCworldEditor
{
    public class Commands
    {
        private InventoryCommands inventoryCommands = new();

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
        }

        public int CallCommand(string[] args)
        {
            return rootCommand.Parse(args).Invoke();
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

            Command addItemCommand = new("add", "Adds item with specified id to inventory") { _worldNumberArgument, addItemArgument };
            addItemCountOption.Aliases.Add("--amount");
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

            return worldNumberArgument;
        }
    }
}
