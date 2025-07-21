using fNbt;
using System.Text;

namespace MCworldEditor.CommandsToCall
{
    public class WorldCommands
    {
        private DatHelper _datHelper;

        public WorldCommands(DatHelper datHelper)
        {
            _datHelper = datHelper;
        }

        public int ChunkDimensions(int? x, int? z)//TODO: pobierac pozycje gracza jesli nie podano x lub z
        {
            int searched = x < 0 ? -15 : 0;
            while (x % 16 != searched)
                x--;

            searched = z < 0 ? -15 : 0;
            while (z % 16 != searched)
                z--;

            Console.WriteLine($"Start:\n\tX: {x}\n\tY: 0\n\tZ: {z}\nEnd:\n\tX: {x + 15}\n\tY: 128\n\tZ: {z + 15}");
            return 0;
        }

        public int CountBlocksOnChunk(int worldId, int blockId, int? x, int? z)
        {
            var playerPosition = _datHelper.GetPlayerPositionInt(worldId);

            int X = x is null ? playerPosition.X : (int)x;
            int Z = z is null ? playerPosition.Z : (int)z;
            string chunkPath = _datHelper.GetChunkPathByCoordinates(worldId, X, 0, Z);
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
            int blockCount = blocks.Count(b => b == blockId);
            Console.WriteLine($"Number of blocks with id {blockId}: {blockCount}");
            return 0;
        }
    }
}
