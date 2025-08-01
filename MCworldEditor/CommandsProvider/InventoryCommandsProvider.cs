﻿using MCworldEditor.CommandsToCall;
using MCworldEditor.Services.Interfaces;
using System.CommandLine;

namespace MCworldEditor.CommandsProvider
{
    public class InventoryCommandsProvider
    {
        private readonly InventoryCommands _inventoryCommands;

        public InventoryCommandsProvider(InventoryCommands inventoryCommands)
        {
            _inventoryCommands = inventoryCommands;
        }

        public void Register(RootCommand rootCommand, Option<int> worldOption)
        {
            Command inventoryCommand = new("inventory", "Handles operations related to player's inventory.");
            rootCommand.Subcommands.Add(inventoryCommand);
            RegisterAddItemCommand(inventoryCommand, worldOption);
            RegisterClearInventoryCommand(inventoryCommand, worldOption);
            RegisterReadInventoryCommand(inventoryCommand, worldOption);
        }

        private void RegisterReadInventoryCommand(Command inventoryCommand, Option<int> worldOption)
        {
            Command readInventory = new("read", "Lists count and id of every item in inventory");
            readInventory.Aliases.Add("list");
            inventoryCommand.Subcommands.Add(readInventory);
            readInventory.SetAction(context=>_inventoryCommands.ReadInventory(context.GetValue(worldOption)));
        }

        private void RegisterAddItemCommand(Command inventoryCommand, Option<int> worldOption)
        {
            Argument<int> itemIdArgument = new("itemId")
            {
                Description = "Id of item"
            };

            Option<int> addItemCountOption = new("--count")
            {
                Required = true,
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
                if (amountNumber < 1)
                {
                    result.AddError("Amount is too small.");
                    return;
                }
            });

            Command addItemCommand = new("add", "Adds item with specified id to inventory") { itemIdArgument };
            addItemCommand.Options.Add(addItemCountOption);
            inventoryCommand.Subcommands.Add(addItemCommand);
            addItemCommand.SetAction(context => _inventoryCommands.AddItemToInventory(context.GetValue(worldOption), context.GetValue(itemIdArgument), context.GetValue(addItemCountOption)));
        }

        private void RegisterClearInventoryCommand(Command inventoryCommand, Option<int> worldOption)
        {
            Command clearInventoryCommand = new("clear", "Clears inventory");
            clearInventoryCommand.Aliases.Add("clean");
            inventoryCommand.Subcommands.Add(clearInventoryCommand);
            clearInventoryCommand.SetAction(context => _inventoryCommands.ClearInventory(context.GetValue(worldOption)));
        }
    }
}
