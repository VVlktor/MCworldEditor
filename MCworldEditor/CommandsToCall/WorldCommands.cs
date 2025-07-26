using fNbt;
using MCworldEditor.Services;
using MCworldEditor.Services.Interfaces;

namespace MCworldEditor.CommandsToCall
{
    public class WorldCommands
    {
        private IPlayerPositionService _datHelper;
        private TimeService _timeService;
        private IFileService _fileService;

        public WorldCommands(IPlayerPositionService datHelper, TimeService timeService, IFileService fileService)
        {
            _datHelper = datHelper;
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
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldId}", "level.dat");
            NbtFile nbtFile = _fileService.ReadDatFile(path);
            var seedTag = nbtFile.RootTag.Get<NbtCompound>("Data")!.Get<NbtLong>("RandomSeed");
            seedTag!.Value = seed;
            nbtFile.SaveToFile(path, NbtCompression.GZip);
            return 0;
        }

        public int ReadSeed(int worldId)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldId}", "level.dat");
            NbtFile nbtFile = _fileService.ReadDatFile(path);
            var seedTag = nbtFile.RootTag.Get<NbtCompound>("Data")!.Get<NbtLong>("RandomSeed");
            Console.WriteLine($"Seed: {seedTag!.Value}");
            return 0;
        }

        public int ChunkDimensions(int worldId, int? x, int? z)
        {
            if (x is null || z is null)
            {
                var playerPosition = _datHelper.GetPlayerPositionInt(worldId);
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

        public int CountBlocksOnChunk(int worldId, int blockId, int? x, int? z)
        {
            var playerPosition = _datHelper.GetPlayerPositionInt(worldId);

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
