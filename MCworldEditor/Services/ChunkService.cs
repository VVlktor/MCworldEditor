using fNbt;
using MCworldEditor.Services.Interfaces;

namespace MCworldEditor.Services
{
    public class ChunkService : IChunkService
    {
        private IFileService _fileService;
        private IPlayerPositionService _playerPositionService;

        public ChunkService(IFileService fileService, IPlayerPositionService playerPositionService)
        {
            _playerPositionService = playerPositionService;
            _fileService = fileService;
        }

        public bool CheckIfBlock(int worldId, int x, int y, int z)
        {
            string path = _fileService.GetChunkPathByCoordinates(worldId, x, y, z);
            int block = FindByteInChunkByCoords(x, y, z);
            NbtFile nbtFile = _fileService.ReadDatFile(path);
            var levelTag = nbtFile.RootTag.Get<NbtCompound>("Level");
            var blocks = levelTag!.Get<NbtByteArray>("Blocks")!.Value;
            return blocks[block] != 0;
        }

        public int FindByteInChunkByCoords(int x, int y, int z)
        {
            int whichByte = 0;
            var firstChunkCoords = FindFirstBlockOfChunkByCoords(x, y, z);

            whichByte += 128 * Math.Abs(firstChunkCoords.z - z);
            whichByte += 2048 * Math.Abs(firstChunkCoords.x - x);
            whichByte += y;

            return whichByte;
        }

        public (int x, int y, int z) FindFirstBlockOfChunkByCoords(int x, int y, int z)
        {
            int searched = x < 0 ? -15 : 0;
            while (x % 16 != searched)
                x--;

            searched = z < 0 ? -15 : 0;
            while (z % 16 != searched)
                z--;

            //if (x < 0)
            //    while (x % 16 != -15)
            //        x--;
            //else
            //    while (x % 16 != 0)
            //        x--;


            //if (z < 0)
            //    while (z % 16 != -15)
            //        z--;
            //else
            //    while (z % 16 != 0)
            //        z--;

            return (x, 0, z);
        }

        public int CountBlocksOnChunk(int worldId, int blockId, int? x, int? z)
        {
            var validCoords = _playerPositionService.GetValidCoords(worldId, x, z);
            string chunkPath = _fileService.GetChunkPathByCoordinates(worldId, validCoords.x, validCoords.y, validCoords.z);
            NbtFile nbtFile = _fileService.ReadDatFile(chunkPath);
            var levelTag = nbtFile.RootTag.Get<NbtCompound>("Level");
            var blocks = levelTag!.Get<NbtByteArray>("Blocks")!.Value;
            int blockCount = blocks.Count(b => b == blockId);
            return blockCount;
        }
    }
}
