using fNbt;
using MCworldEditor.Services.Interfaces;

namespace MCworldEditor.Services
{
    public class ChunkService : IChunkService
    {
        private IFileService _fileService;

        public ChunkService(IFileService fileService)
        {
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
            if (x < 0)
                while (x % 16 != -15)
                    x--;
            else
                while (x % 16 != 0)
                    x--;


            if (z < 0)
                while (z % 16 != -15)
                    z--;
            else
                while (z % 16 != 0)
                    z--;

            return (x, 0, z);
        }
    }
}
