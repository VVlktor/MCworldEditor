using MCworldEditor.Services.Interfaces;

namespace MCworldEditor.CommandsToCall
{
    public class WorldCommands
    {
        private IPlayerPositionService _playerPositionService;
        private ITimeService _timeService;
        private IFileService _fileService;
        private ISeedService _seedService;
        private IChunkService _chunkService;
        private IMobService _mobService;

        public WorldCommands(IPlayerPositionService playerPositionService, ITimeService timeService, IFileService fileService, ISeedService seedService, IChunkService chunkService, IMobService mobService)
        {
            _seedService = seedService;
            _playerPositionService = playerPositionService;
            _timeService = timeService;
            _fileService = fileService;
            _chunkService = chunkService;
            _mobService = mobService;
        }

        public int RemoveMobs(int worldOption, bool passive, bool hostile, int? area)
        {
            int response = _mobService.RemoveMobs(worldOption, passive, hostile, area);
            return response;
        }

        public int SpawnMob(int worldOption, string mobIdArgument, int? xOption, int? yOption, int? zOption, int? hp, int? count)
        {
            int response = _mobService.SpawnMob(worldOption, mobIdArgument, xOption, yOption, zOption, hp, count);
            return response;
        }

        public int SetDay(int worldId)
        {
            int response = _timeService.SetTime(worldId, 0);
            return response;
        }

        public int SetNight(int worldId)
        {
            int response = _timeService.SetTime(worldId, 13000);
            return response;
        }

        public int ReadTime(int worldId, bool isRaw)
        {
            var readTimeResult = _timeService.ReadTime(worldId);
            var formattedTime = _timeService.FormatTime(readTimeResult, isRaw);
            Console.WriteLine($"Time spent in world: {formattedTime}");
            return 0;
        }

        public int SetSeed(int worldId, long seed)
        {
            _seedService.SetSeed(worldId, seed);
            return 0;
        }

        public int ReadSeed(int worldId)
        {
            long result = _seedService.ReadSeed(worldId);
            Console.WriteLine($"Seed: {result}");
            return 0;
        }

        public int ChunkDimensions(int worldId, int? x, int? z)
        {
            var validCoords = _playerPositionService.GetValidCoords(worldId, x, z);
            var chunkDimensions = _chunkService.FindFirstBlockOfChunkByCoords(validCoords.x, validCoords.y, validCoords.z);
            Console.WriteLine($"Start:\n\tX: {x}\n\tY: 0\n\tZ: {z}\nEnd:\n\tX: {x + 15}\n\tY: 128\n\tZ: {z + 15}");
            return 0;
        }

        public int CountBlocksOnChunk(int worldId, int blockId, int? x, int? z)
        {
            var validCoords = _playerPositionService.GetValidCoords(worldId, x, z);
            var chunkPath = _fileService.GetChunkPathByCoordinates(worldId, validCoords.x, validCoords.y, validCoords.z);
            if (!File.Exists(chunkPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Chunk hasnt been generated yet.");
                Console.ResetColor();
                return 1;
            }
            int blockCount = _chunkService.CountBlocksOnChunk(worldId, blockId, x, z);
            Console.WriteLine($"Number of blocks with id {blockId}: {blockCount}");
            return 0;
        }
    }
}
