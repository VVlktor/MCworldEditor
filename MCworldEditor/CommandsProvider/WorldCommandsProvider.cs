using MCworldEditor.CommandsToCall;
using MCworldEditor.Services.Interfaces;
using System.CommandLine;

namespace MCworldEditor.CommandsProvider
{
    public class WorldCommandsProvider
    {
        private IPlayerPositionService _datHelper;
        private WorldCommands _worldCommands;

        public WorldCommandsProvider(IPlayerPositionService datHelper, WorldCommands worldCommands)
        {
            _datHelper = datHelper;
            _worldCommands = worldCommands;
        }

        public void Register(Command rootCommand, Option<int> worldOption)//komenda na ustawienie dnia/nocy
        {
            Command worldCommand = new("map", "Allows to manipulate and read map data");

            rootCommand.Subcommands.Add(worldCommand);

            Option<int?> xOption = new("-x")
            {
                Description = "In-world X coordinates. If not specified player's position will be used.",
                DefaultValueFactory = a => null
            };

            Option<int?> zOption = new("-z")
            {
                Description = "In-world Z coordinates. If not specified player's position will be used.",
                DefaultValueFactory = a => null
            };

            RegisterSearchForBlockCommand(worldCommand, worldOption, xOption, zOption);
            RegisterChunkDimendionsCommand(worldCommand, worldOption, xOption, zOption);
            RegisterSeedCommand(worldCommand, worldOption);
            RegisterTimeCommand(worldCommand, worldOption);
        }

        private void RegisterTimeCommand(Command worldCommand, Option<int> worldOption)
        {
            Command timeCommand = new("time", "Commands ");
            worldCommand.Subcommands.Add(timeCommand);
            RegisterReadTimeCommand(timeCommand, worldOption);
        }

        private void RegisterReadTimeCommand(Command timeCommand, Option<int> worldOption)
        {
            Option<bool> rawTimeOption = new("--raw")
            {
                Description = "Displays number of ticks instead of formatted time."
            };
            Command readTimeCommand = new("read", "Displays time spent in world (HH:MM:SS).") { rawTimeOption };
            readTimeCommand.SetAction(context => _worldCommands.ReadTime(context.GetValue(worldOption), context.GetValue(rawTimeOption)));
            timeCommand.Subcommands.Add(readTimeCommand);
        }

        private void RegisterSeedCommand(Command worldCommand, Option<int> worldOption)
        {
            Command seedCommand = new("seed", "Operations related to seed value of the world.");
            worldCommand.Subcommands.Add(seedCommand);
            RegisterSetSeedCommand(seedCommand, worldOption);
            RegisterReadSeedCommand(seedCommand, worldOption);
        }

        private void RegisterReadSeedCommand(Command seedCommand, Option<int> worldOption)
        {
            Command readSeedCommand = new("read", "Displays value of seed.");
            seedCommand.Subcommands.Add(readSeedCommand);
            readSeedCommand.SetAction(context => _worldCommands.ReadSeed(context.GetValue(worldOption)));
        }

        private void RegisterSetSeedCommand(Command seedCommand, Option<int> worldOption)
        {
            Argument<long> setSeedArgument = new("seedId")
            {
                Description = "ID of new seed"
            };
            Command setSeedCommand = new("set", "Sets value of seed. Keep in mind this cannot be reversed and generation of chunks might be bugged.") { setSeedArgument };
            seedCommand.Subcommands.Add(setSeedCommand);
            setSeedCommand.SetAction(context => _worldCommands.SetSeed(context.GetValue(worldOption), context.GetValue(setSeedArgument)));
        }

        private void RegisterSearchForBlockCommand(Command worldCommand, Option<int> worldOption, Option<int?> xOption, Option<int?> zOption)
        {
            Argument<int> searchForBlockIdArgument = new("blockId")
            {
                Description = "ID of the block to search for."
            };
            Command searchForBlockCommand = new("count", "Searches for block with specified id on chunk with provided coordinates") { searchForBlockIdArgument, xOption, zOption };
            searchForBlockCommand.SetAction(context => _worldCommands.CountBlocksOnChunk(context.GetValue(worldOption), context.GetValue(searchForBlockIdArgument), context.GetValue(xOption), context.GetValue(zOption)));
            worldCommand.Subcommands.Add(searchForBlockCommand);
        }

        private void RegisterChunkDimendionsCommand(Command worldCommand, Option<int> worldOption, Option<int?> xOption, Option<int?> zOption)
        {
            Command chunkDimensionsCommand = new("dimensions", "Displays the X, Y, Z coordinates of the chunk corners for the given position.") { xOption, zOption };
            chunkDimensionsCommand.Aliases.Add("corners");
            chunkDimensionsCommand.SetAction(context => _worldCommands.ChunkDimensions(context.GetValue(worldOption), context.GetValue(xOption), context.GetValue(zOption)));
            worldCommand.Subcommands.Add(chunkDimensionsCommand);
        }
    }
}
