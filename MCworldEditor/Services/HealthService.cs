using fNbt;
using MCworldEditor.Services.Interfaces;

namespace MCworldEditor.Services
{
    public class HealthService : IHealthService
    {
        private IFileService _fileService;

        public HealthService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public int SetHealth(int worldId, short hp)
        {
            string path = _fileService.GetLevelDatPath(worldId);
            NbtFile nbtFile = _fileService.ReadDatFile(path);
            var healthTag = nbtFile.RootTag.Get<NbtCompound>("Data")!.Get<NbtCompound>("Player")!.Get<NbtShort>("Health");
            healthTag!.Value = hp;
            nbtFile.SaveToFile(path, NbtCompression.GZip);
            return 0;
        }
    }
}
