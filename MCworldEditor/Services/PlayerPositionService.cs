using fNbt;
using MCworldEditor.Services.Interfaces;
using System.Text;

namespace MCworldEditor.Services
{
    public class PlayerPositionService : IPlayerPositionService
    {
        private IFileService _fileService;

        public PlayerPositionService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public (int X, int Y, int Z) GetPlayerPositionInt(int worldId)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldId}", "level.dat");
            NbtFile nbtLevelFile = _fileService.ReadDatFile(path);
            var dataTag = nbtLevelFile.RootTag.Get<NbtCompound>("Data");
            var playerTag = dataTag!.Get<NbtCompound>("Player");
            var positionTag = playerTag!.Get<NbtList>("Pos");

            return ((int)positionTag![0].DoubleValue, (int)positionTag![1].DoubleValue, (int)positionTag![2].DoubleValue);
        }

        public (double X, double Y, double Z) GetPlayerPositionDouble(int worldId)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldId}", "level.dat");
            NbtFile nbtLevelFile = _fileService.ReadDatFile(path);
            var dataTag = nbtLevelFile.RootTag.Get<NbtCompound>("Data");
            var playerTag = dataTag!.Get<NbtCompound>("Player");
            var positionTag = playerTag!.Get<NbtList>("Pos");

            return (positionTag![0].DoubleValue, positionTag![1].DoubleValue, positionTag![2].DoubleValue);
        }
    }
}
