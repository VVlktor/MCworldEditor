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
            string path = _fileService.GetLevelDatPath(worldId);
            NbtFile nbtLevelFile = _fileService.ReadDatFile(path);
            var dataTag = nbtLevelFile.RootTag.Get<NbtCompound>("Data");
            var playerTag = dataTag!.Get<NbtCompound>("Player");
            var positionTag = playerTag!.Get<NbtList>("Pos");

            return ((int)positionTag![0].DoubleValue, (int)positionTag![1].DoubleValue, (int)positionTag![2].DoubleValue);
        }

        public (double X, double Y, double Z) GetPlayerPositionDouble(int worldId)
        {
            string path = _fileService.GetLevelDatPath(worldId); 
            NbtFile nbtLevelFile = _fileService.ReadDatFile(path);
            var dataTag = nbtLevelFile.RootTag.Get<NbtCompound>("Data");
            var playerTag = dataTag!.Get<NbtCompound>("Player");
            var positionTag = playerTag!.Get<NbtList>("Pos");

            return (positionTag![0].DoubleValue, positionTag![1].DoubleValue, positionTag![2].DoubleValue);
        }

        public (int x, int y, int z) GetValidCoords(int worldId, int? x, int? z)
        {
            var playerPosition = GetPlayerPositionInt(worldId);
            x = x is null ? playerPosition.X : x;
            z = z is null ? playerPosition.Z : z;
            return ((int)x, 0, (int)z);
        }

        public int SetPlayerPosition(int worldId, int x, int y, int z)
        {
            string path = _fileService.GetLevelDatPath(worldId);
            NbtFile nbtFile = _fileService.ReadDatFile(path);

            var posTag = nbtFile.RootTag.Get<NbtCompound>("Data")!.Get<NbtCompound>("Player")!.Get<NbtList>("Pos");
            posTag!.Clear();
            
            posTag.Add(new NbtDouble() { Value = x + (x >= 0 ? 0.5 : -0.5) });
            posTag.Add(new NbtDouble() { Value = y + 0.7 });
            posTag.Add(new NbtDouble() { Value = z + (z >= 0 ? 0.5 : -0.5) });

            nbtFile.SaveToFile(path, NbtCompression.GZip);
            return 0;
        }
    }
}
