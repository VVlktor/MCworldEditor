using fNbt;
using System.Text;

namespace MCworldEditor
{
    public class DatHelper
    {
        public NbtFile ReadDatFile(string path)
        {
            NbtFile nbtFile = new NbtFile();
            using (FileStream stream = File.OpenRead(path))
            {
                nbtFile.LoadFromStream(stream, NbtCompression.AutoDetect);
            }
            return nbtFile;
        }

        public (int X, int Y, int Z) GetPlayerPositionInt(int worldId)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldId}", "level.dat");
            NbtFile nbtLevelFile = ReadDatFile(path);
            var dataTag = nbtLevelFile.RootTag.Get<NbtCompound>("Data");
            var playerTag = dataTag!.Get<NbtCompound>("Player");
            var positionTag = playerTag!.Get<NbtList>("Pos");

            return ((int)positionTag![0].DoubleValue, (int)positionTag![1].DoubleValue, (int)positionTag![2].DoubleValue);
        }

        public (double X, double Y, double Z) GetPlayerPositionDouble(int worldId)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldId}", "level.dat");
            NbtFile nbtLevelFile = ReadDatFile(path);
            var dataTag = nbtLevelFile.RootTag.Get<NbtCompound>("Data");
            var playerTag = dataTag!.Get<NbtCompound>("Player");
            var positionTag = playerTag!.Get<NbtList>("Pos");

            return (positionTag![0].DoubleValue, positionTag![1].DoubleValue, positionTag![2].DoubleValue);
        }

        public bool CheckIfBlock(int worldId, int x, int y, int z)
        {
            string path = GetChunkPathByCoordinates(worldId, x, y, z);
            int block = FindByteInChunkByCoords(x, y, z);
            NbtFile nbtFile = ReadDatFile(path);
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

        private (int x, int y, int z) FindFirstBlockOfChunkByCoords(int x, int y, int z)
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
