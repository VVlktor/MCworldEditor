using fNbt;

namespace MCworldEditor
{
    public class ChunkCommands
    {
        public int CountBlocksOnChunk(int worldId, int blockId, int x, int z)
        {
            string chunkPath = GetChunkPathByCoordinates(worldId, x, 0, z);
            if (!File.Exists(chunkPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Chunk hasnt been generated yet.");
                Console.ResetColor();
                return 1;
            }

            NbtFile nbtFile = new NbtFile(chunkPath);
            using (FileStream stream = File.OpenRead(chunkPath))
            {
                nbtFile.LoadFromStream(stream, NbtCompression.AutoDetect);
            }
            var levelTag = nbtFile.RootTag.Get<NbtCompound>("Level");
            var blocks = levelTag!.Get<NbtByteArray>("Blocks")!.Value;
            int blockCount = blocks.Count(x => x == blockId);
            Console.WriteLine($"Number of blocks with id {blockId}: {blockCount}");
            return 0;
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
            string path = Path.Combine(@"C:\Users\Admin\AppData\Roaming\.minecraft\saves\World"+worldNumber, f1, f2, $"c.{signed36X}.{signed36Z}.dat");
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
            if (value == 0) return "0";

            bool isNegative = value < 0;
            if (isNegative) 
                value = -value;

            var result = new System.Text.StringBuilder();

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
