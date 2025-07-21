using MCworldEditor.CommandsToCall;
using Microsoft.VisualBasic.FileIO;
using System.CommandLine;

namespace MCworldEditor.CommandsProvider
{
    public class WorldCommandsProvider
    {
        private DatHelper _datHelper;
        private WorldCommands _worldCommands;

        public WorldCommandsProvider(DatHelper datHelper, WorldCommands worldCommands)
        {
            _datHelper = datHelper;
            _worldCommands = worldCommands;
        }

        public void Register(Command rootCommand, Option<int> worldOption)//dodac komende na podejrzenie seeda, 
        {
            Command worldCommand = new("chunk", "Allows to operate on chunks");

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
            RegisterChunkDimendionsCommand(worldCommand, xOption, zOption);
        }

        private void RegisterSearchForBlockCommand(Command worldCommand, Option<int> worldOption, Option<int?> xOption, Option<int?> zOption)
        {
            Argument<int> searchForBlockIdArgument = new("blockId")
            {
                Description = "ID of the block to search for."
            };
            Command searchForBlockCommand = new("count", "Searches for block with specified id on chunk with provided coordinates") { searchForBlockIdArgument, xOption, zOption };
            searchForBlockCommand.SetAction(arguments => _worldCommands.CountBlocksOnChunk(arguments.GetValue(worldOption), arguments.GetValue(searchForBlockIdArgument), arguments.GetValue(xOption), arguments.GetValue(zOption)));
            worldCommand.Subcommands.Add(searchForBlockCommand);
        }

        private void RegisterChunkDimendionsCommand(Command worldCommand, Option<int?> xOption, Option<int?> zOption)
        {
            Command chunkDimensionsCommand = new("dimensions", "Displays the X, Y, Z coordinates of the chunk corners for the given position.") { xOption, zOption };
            chunkDimensionsCommand.Aliases.Add("corners");
            chunkDimensionsCommand.SetAction(arguments => _worldCommands.ChunkDimensions(arguments.GetValue(xOption), arguments.GetValue(zOption)));
            worldCommand.Subcommands.Add(chunkDimensionsCommand);
        }
    }
}
