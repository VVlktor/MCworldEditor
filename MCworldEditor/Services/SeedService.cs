using fNbt;
using MCworldEditor.Services.Interfaces;

namespace MCworldEditor.Services
{
    public class SeedService : ISeedService
    {
        private IFileService _fileService;

        public SeedService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public long ReadSeed(int worldId)
        {
            string path = _fileService.GetLevelDatPath(worldId);
            NbtFile nbtFile = _fileService.ReadDatFile(path);
            long seed = nbtFile.RootTag.Get<NbtCompound>("Data")!.Get<NbtLong>("RandomSeed")!.Value;
            return seed;
        }

        public void SetSeed(int worldId, long seed)
        {
            string path = _fileService.GetLevelDatPath(worldId);
            NbtFile nbtFile = _fileService.ReadDatFile(path);
            var seedTag = nbtFile.RootTag.Get<NbtCompound>("Data")!.Get<NbtLong>("RandomSeed");
            seedTag!.Value = seed;
            nbtFile.SaveToFile(path, NbtCompression.GZip);
        }
    }
}
