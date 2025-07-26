using fNbt;
using MCworldEditor.Services.Interfaces;
using System.Text;

namespace MCworldEditor.Services
{
    public class FileService : IFileService
    {
        public string GetLevelDatPath(int worldId)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldId}", "level.dat");
        }

        public NbtFile ReadDatFile(string path)
        {
            NbtFile nbtFile = new NbtFile();
            using (FileStream stream = File.OpenRead(path))
            {
                nbtFile.LoadFromStream(stream, NbtCompression.AutoDetect);
            }
            return nbtFile;
        }

        public string GetChunkPathByCoordinates(int worldNumber, int x, int y, int z)
        {
            var coords = GetChunkCoordinatesFromBlock(x, z);
            var chunkX = coords.chunkX;
            var chunkZ = coords.chunkZ;
            string signed36X = ToSignedBase36(chunkX);
            string signed36Z = ToSignedBase36(chunkZ);
            string f1 = ToBase36(chunkX);
            string f2 = ToBase36(chunkZ);
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldNumber}", f1, f2, $"c.{signed36X}.{signed36Z}.dat");
            //string path = Path.Combine(@"C:\Users\Admin\AppData\Roaming\.minecraft\saves\World" + worldNumber, f1, f2, $"c.{signed36X}.{signed36Z}.dat");
            return path;
        }

        private (int chunkX, int chunkZ) GetChunkCoordinatesFromBlock(int blockX, int blockZ)
        {
            int chunkX = (int)Math.Floor(blockX / 16.0);
            int chunkZ = (int)Math.Floor(blockZ / 16.0);
            return (chunkX, chunkZ);
        }

        private string ToBase36(int v)
        {
            int av = (v < 0 ? v + 64 * ((-v + 63) / 64) : v) % 64;
            return ToSignedBase36(av);
        }

        private string ToSignedBase36(int value)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyz";
            if (value == 0)
                return "0";

            bool isNegative = value < 0;
            if (isNegative)
                value = -value;

            var result = new StringBuilder();

            while (value > 0)
            {
                int remainder = value % 36;
                result.Insert(0, chars[remainder]);
                value /= 36;
            }

            if (isNegative)
                result.Insert(0, '-');

            return result.ToString();
        }
    }
}
