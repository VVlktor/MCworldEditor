using fNbt;
using MCworldEditor.Services.Interfaces;

namespace MCworldEditor.Services
{
    public class SpawnService : ISpawnService
    {
        private IFileService _fileService;
        private IChunkService _chunkService;

        public SpawnService(IFileService fileService, IChunkService chunkService)
        {
            _fileService = fileService;
            _chunkService = chunkService;
        }

        public (int x, int y, int z) ReadSpawnPoint(int worldId)
        {
            string path =  _fileService.GetLevelDatPath(worldId);
            NbtFile nbtFile = _fileService.ReadDatFile(path);
            var dataTag = nbtFile.RootTag.Get<NbtCompound>("Data");
            var xSpawn = dataTag!.Get<NbtInt>("SpawnX")!.IntValue;
            var ySpawn = dataTag!.Get<NbtInt>("SpawnY")!.IntValue;
            var zSpawn = dataTag!.Get<NbtInt>("SpawnZ")!.IntValue;
            return (xSpawn, ySpawn, zSpawn);
        }

        public int SetSpawnPoint(int worldId, int x, int y, int z, bool safe)
        {
            string path = _fileService.GetLevelDatPath(worldId);
            NbtFile nbtFile = _fileService.ReadDatFile(path);

            var dataTag = nbtFile.RootTag.Get<NbtCompound>("Data");
            if (safe && y > 126)
            {
                Console.WriteLine("Unable to safely change the spawnpoint.");
                return 1;
            }
            if (safe && (_chunkService.CheckIfBlock(worldId, x, y + 1, z) || _chunkService.CheckIfBlock(worldId, x, y + 2, z)))//poprawic zeby CheckIfBlock nie robił exception przy y>128 (wywala byte) - niby poprawione, trzeba potestować - mialem to przetestowac dawno temu, teraz nie odkladaj tego na potem
            {
                Console.WriteLine("Unable to safely change the spawnpoint.");
                return 1;
            }
            dataTag!.Get<NbtInt>("SpawnX")!.Value = x;
            dataTag!.Get<NbtInt>("SpawnY")!.Value = y;
            dataTag!.Get<NbtInt>("SpawnZ")!.Value = z;

            nbtFile.SaveToFile(path, NbtCompression.GZip);
            return 0;
        }
    }
}
