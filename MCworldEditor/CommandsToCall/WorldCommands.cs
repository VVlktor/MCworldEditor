using fNbt;
using MCworldEditor.Services;
using MCworldEditor.Services.Interfaces;

namespace MCworldEditor.CommandsToCall
{
    public class WorldCommands
    {
        private IPlayerPositionService _playerPositionService;
        private ITimeService _timeService;
        private IFileService _fileService;
        private ISeedService _seedService;

        public WorldCommands(IPlayerPositionService playerPositionService, ITimeService timeService, IFileService fileService, ISeedService seedService)
        {
            _seedService = seedService;
            _playerPositionService = playerPositionService;
            _timeService = timeService;
            _fileService = fileService;
        }

        public int ReadTime(int worldId, bool isRaw)
        {
            var readTimeResult = _timeService.ReadTime(worldId);
            var formattedTime = _timeService.FormatTime(readTimeResult, isRaw);
            Console.WriteLine(formattedTime);
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

        public int ChunkDimensions(int worldId, int? x, int? z)//przeniesc do ChunkService
        {
            if (x is null || z is null)
            {
                var playerPosition = _playerPositionService.GetPlayerPositionInt(worldId);
                x = x is null ? playerPosition.X : x;
                z = z is null ? playerPosition.Z : z;
            }
            int searched = x < 0 ? -15 : 0;
            while (x % 16 != searched)
                x--;

            searched = z < 0 ? -15 : 0;
            while (z % 16 != searched)
                z--;

            Console.WriteLine($"Start:\n\tX: {x}\n\tY: 0\n\tZ: {z}\nEnd:\n\tX: {x + 15}\n\tY: 128\n\tZ: {z + 15}");
            return 0;
        }

        public int CountBlocksOnChunk(int worldId, int blockId, int? x, int? z)//przeniesc do ChunkService
        {
            var playerPosition = _playerPositionService.GetPlayerPositionInt(worldId);

            int X = x is null ? playerPosition.X : (int)x;
            int Z = z is null ? playerPosition.Z : (int)z;
            string chunkPath = _fileService.GetChunkPathByCoordinates(worldId, X, 0, Z);
            if (!File.Exists(chunkPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Chunk hasnt been generated yet.");
                Console.ResetColor();
                return 1;
            }

            NbtFile nbtFile = _fileService.ReadDatFile(chunkPath);
            var levelTag = nbtFile.RootTag.Get<NbtCompound>("Level");
            var blocks = levelTag!.Get<NbtByteArray>("Blocks")!.Value;
            int blockCount = blocks.Count(b => b == blockId);
            Console.WriteLine($"Number of blocks with id {blockId}: {blockCount}");
            return 0;
        }
    }
}
