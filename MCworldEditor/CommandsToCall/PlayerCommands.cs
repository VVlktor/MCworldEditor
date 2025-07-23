using fNbt;
using System.CommandLine;
using System.Drawing;
using System.IO;

namespace MCworldEditor.CommandsToCall
{
    public class PlayerCommands
    {
        private DatHelper _datHelper;

        public PlayerCommands(DatHelper datHelper)
        {
            _datHelper = datHelper;
        }

        public int SetHealth(int worldId, short hp)
        {
            NbtFile nbtFile = new NbtFile();

            string path = Path.Combine(@"C:\Users\Admin\AppData\Roaming\.minecraft\saves\World" + worldId, "level.dat");
            using (FileStream stream = File.OpenRead(path))
            {
                nbtFile.LoadFromStream(stream, NbtCompression.AutoDetect);
            }
            var healthTag = nbtFile.RootTag.Get<NbtCompound>("Data")!.Get<NbtCompound>("Player")!.Get<NbtShort>("Health");
            healthTag!.Value = hp;

            nbtFile.SaveToFile(path, NbtCompression.GZip);
            return 0;
        }

        public int SetSpawnPoint(int worldId, int x, int y, int z, bool safe)
        {
            NbtFile nbtFile = new NbtFile();
            string path = Path.Combine(@"C:\Users\Admin\AppData\Roaming\.minecraft\saves\World" + worldId, "level.dat");
            using (FileStream stream = File.OpenRead(path))
            {
                nbtFile.LoadFromStream(stream, NbtCompression.AutoDetect);
            }

            var dataTag = nbtFile.RootTag.Get<NbtCompound>("Data");
            if (safe && (_datHelper.CheckIfBlock(worldId, x, y+1, z) || _datHelper.CheckIfBlock(worldId, x, y + 2, z) || y > 126))//poprawic zeby CheckIfBlock nie robil exception przy y>128 (jak dam y=200000 to wywala byte)
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

        public int ReadPosition(int worldId, bool specific)
        {
            var positionDouble = _datHelper.GetPlayerPositionDouble(worldId);
            if (specific)
                Console.WriteLine($"X: {positionDouble.X}\nY: {positionDouble.Y}\nZ: {positionDouble.Z}");
            else
                Console.WriteLine($"X: {(int)positionDouble.X}\nY: {(int)positionDouble.Y}\nZ: {(int)positionDouble.Z}");
            return 0;
        }

        public int SetPlayerPosition(int worldId, int x, int y, int z, bool safe)
        {
            NbtFile nbtFile = new NbtFile();
            string path = Path.Combine(@"C:\Users\Admin\AppData\Roaming\.minecraft\saves\World" + worldId, "level.dat");
            using (FileStream stream = File.OpenRead(path))
            {
                nbtFile.LoadFromStream(stream, NbtCompression.AutoDetect);
            }

            var posTag = nbtFile.RootTag.Get<NbtCompound>("Data")!.Get<NbtCompound>("Player")!.Get<NbtList>("Pos");
            posTag!.Clear();
            if(safe && (_datHelper.CheckIfBlock(worldId, x, y, z) || y<1 || _datHelper.CheckIfBlock(worldId, x, y-1, z)))
            {
                Console.WriteLine("Unable to safely change the player's position.");
                return 1;
            }
            posTag.Add(new NbtDouble() { Value = x + (x>=0 ? 0.5 : -0.5) });
            posTag.Add(new NbtDouble() { Value = y + 0.7 });
            posTag.Add(new NbtDouble() { Value = z + (z>=0 ? 0.5 : -0.5) });

            nbtFile.SaveToFile(path, NbtCompression.GZip);
            return 0;
        }

        public int ReadSpawn(int worldId)
        {
            NbtFile nbtFile = new NbtFile();
            string path = Path.Combine(@"C:\Users\Admin\AppData\Roaming\.minecraft\saves\World" + worldId, "level.dat");
            using (FileStream stream = File.OpenRead(path))
            {
                nbtFile.LoadFromStream(stream, NbtCompression.AutoDetect);
            }
            var dataTag = nbtFile.RootTag.Get<NbtCompound>("Data");
            var xSpawn = dataTag!.Get<NbtInt>("SpawnX")!.IntValue;
            var ySpawn = dataTag!.Get<NbtInt>("SpawnY")!.IntValue;
            var zSpawn = dataTag!.Get<NbtInt>("SpawnZ")!.IntValue;
            Console.WriteLine($"X: {xSpawn}\nY: {ySpawn}\nZ: {zSpawn}");
            return 0;
        }
    }
}
