using MCworldEditor.CommandsToCall;
using System.CommandLine;

namespace MCworldEditor.CommandsProvider
{
    public class WorldCommandsProvider
    {
        private WorldCommands _worldCommands;

        public WorldCommandsProvider(WorldCommands worldCommands)
        {
            _worldCommands = worldCommands;
        }

        public void Register(Command rootCommand, Option<int> worldOption)
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
            RegisterMobCommand(worldCommand, worldOption, xOption, zOption);
        }

        private void RegisterMobCommand(Command worldCommand, Option<int> worldOption, Option<int?> xOption, Option<int?> zOption)
        {
            Command mobCommand = new("mob", "Commands related to mobs.");
            worldCommand.Subcommands.Add(mobCommand);
            RegisterSpawnMobCommand(mobCommand, worldOption, xOption, zOption);
            RegisterRemoveMobCommand(mobCommand, worldOption);
        }

        private void RegisterRemoveMobCommand(Command mobCommand, Option<int> worldOption)
        {
            Option<bool> hostileOption = new("--hostile")
            {
                Description = "If provided, hostile mobs will be removed."
            };

            Option<bool> passiveOption = new("--passive")
            {
                Description = "If provided, passive mobs will be removed."
            };

            Option<int?> areaOption = new("-area")
            {
                Description = "Radius around the player, measured in chunks, from which mobs should be removed. If not specified, mobs are removed only from player's current chunk."
            };

            Command removeMobCommand = new("remove", "Removes mobs from the chunk player is currently in. If neither [--passive] or [--hostile] is used, no mobs will be removed.") { worldOption, passiveOption, hostileOption, areaOption };
            mobCommand.Subcommands.Add(removeMobCommand);
            removeMobCommand.Aliases.Add("vanish");
            removeMobCommand.SetAction(context => _worldCommands.RemoveMobs(context.GetValue(worldOption), context.GetValue(passiveOption), context.GetValue(hostileOption), context.GetValue(areaOption)));
        }

        private void RegisterSpawnMobCommand(Command mobCommand, Option<int> worldOption, Option<int?> xOption, Option<int?> zOption)
        {
            Option<int?> yOption = new("-y")
            {
                Description = "In-world Y coordinates. If not specified player's position will be used.",
                DefaultValueFactory = a => null
            };

            Option<int?> hpOption = new("-hp")
            {
                Description = "Health points of mob.",
                DefaultValueFactory = a => 10
            };

            Argument<string> mobIdArgument = new("mobId")
            {
                Description = "Id of mob you want to spawn.In beta versions, mobs id was its name with first letter capitalized.",
                DefaultValueFactory = a => ""
            };

            Option<int?> countOption = new("-count")
            {
                Description = "Number of mobs you want to spawn."
            };
            countOption.Aliases.Add("-amount");
            
            Command spawnMobCommand = new("spawn", "Spawns mob with specified id.") { worldOption, mobIdArgument, xOption, zOption, yOption, hpOption, countOption };
            mobCommand.Subcommands.Add(spawnMobCommand);
            spawnMobCommand.SetAction(context => _worldCommands.SpawnMob(context.GetValue(worldOption), context.GetValue(mobIdArgument)!, context.GetValue(xOption), context.GetValue(yOption), context.GetValue(zOption), context.GetValue(hpOption), context.GetValue(countOption)));
        }

        private void RegisterTimeCommand(Command worldCommand, Option<int> worldOption)
        {
            Command timeCommand = new("time", "Commands related to in-game time.");
            worldCommand.Subcommands.Add(timeCommand);
            RegisterReadTimeCommand(timeCommand, worldOption);
            RegisterDayCommand(timeCommand, worldOption);
            RegisterNightCommand(timeCommand, worldOption);
        }

        private void RegisterDayCommand(Command timeCommand, Option<int> worldOption)
        {
            Command dayCommand = new("day", "Set the time to daytime.");
            timeCommand.Subcommands.Add(dayCommand);
            dayCommand.SetAction(context => _worldCommands.SetDay(context.GetValue(worldOption)));
        }

        private void RegisterNightCommand(Command timeCommand, Option<int> worldOption)
        {
            Command nightCommand = new("night", "Set the time to nighttime.");
            timeCommand.Subcommands.Add(nightCommand);
            nightCommand.SetAction(context => _worldCommands.SetNight(context.GetValue(worldOption)));
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
