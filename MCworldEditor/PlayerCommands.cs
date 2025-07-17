using fNbt;

namespace MCworldEditor
{
    public class PlayerCommands
    {
        public (int X, int Y, int Z) GetPlayerPositionInt(int worldId)
        {
            NbtFile nbtLevelFile = new NbtFile();
            string path = Path.Combine(@"C:\Users\Admin\AppData\Roaming\.minecraft\saves\World" + worldId, "level.dat");
            using (FileStream stream = File.OpenRead(path))
            {
                nbtLevelFile.LoadFromStream(stream, NbtCompression.AutoDetect);
            }
            var dataTag = nbtLevelFile.RootTag.Get<NbtCompound>("Data");
            var playerTag = dataTag!.Get<NbtCompound>("Player");
            var positionTag = playerTag!.Get<NbtList>("Pos");

            return ((int)positionTag![0].DoubleValue, (int)positionTag![1].DoubleValue, (int)positionTag![2].DoubleValue);
        }

        private (double X, double Y, double Z) GetPlayerPositionDouble(int worldId)
        {
            NbtFile nbtLevelFile = new NbtFile();
            string path = Path.Combine(@"C:\Users\Admin\AppData\Roaming\.minecraft\saves\World" + worldId, "level.dat");
            using (FileStream stream = File.OpenRead(path))
            {
                nbtLevelFile.LoadFromStream(stream, NbtCompression.AutoDetect);
            }
            var dataTag = nbtLevelFile.RootTag.Get<NbtCompound>("Data");
            var playerTag = dataTag!.Get<NbtCompound>("Player");
            var positionTag = playerTag!.Get<NbtList>("Pos");

            return (positionTag![0].DoubleValue, positionTag![1].DoubleValue, positionTag![2].DoubleValue);
        }

        public int ReadPosition(int worldId, bool specific)
        {
            var positionDouble = GetPlayerPositionDouble(worldId);
            if (specific)
                Console.WriteLine($"X: {positionDouble.X}\nY: {positionDouble.Y}\nZ: {positionDouble.Z}");
            else
                Console.WriteLine($"X: {(int)positionDouble.X}\nY: {(int)positionDouble.Y}\nZ: {(int)positionDouble.Z}");
            return 0;
        }
    }
}
