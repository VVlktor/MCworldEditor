using fNbt;
using MCworldEditor.Services.Interfaces;

namespace MCworldEditor.CommandsToCall
{
    public class PlayerCommands
    {
        private IPlayerPositionService _datHelper;
        private IFileService _fileService;
        private IChunkService _chunkService;

        public PlayerCommands(IPlayerPositionService datHelper, IFileService fileService, IChunkService chunkService)
        {
            _datHelper = datHelper;
            _fileService = fileService;
            _chunkService = chunkService;
        }
        //komende na falldistance, komende na uratowanie - ustawienie hp na max, sprawdzenie czy nie jest w lawie, usuniecie potworow dookola etc.
        public int SaveFromDying(int worldId)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldId}", "level.dat");
            
           

            SetHealth(worldId, 100);

            var positionInt = _datHelper.GetPlayerPositionInt(worldId);

            string chunkPath = _fileService.GetChunkPathByCoordinates(worldId, positionInt.X, positionInt.Y, positionInt.Z);

            int blockByte = _chunkService.FindByteInChunkByCoords(positionInt.X, positionInt.Y, positionInt.Z);

            NbtFile nbtChunkFile = _fileService.ReadDatFile(chunkPath);
            var levelTag = nbtChunkFile.RootTag.Get<NbtCompound>("Level");
            var blocks = levelTag!.Get<NbtByteArray>("Blocks")!.Value;
            int oile = 0;
            while (blocks[blockByte]!=0)
            {
                blockByte++;//TODO - wiadomo
                oile++;
            }
            SetPlayerPosition(worldId, positionInt.X, positionInt.Y + oile + 2, positionInt.Z, false);
            blocks[blockByte] = 1;//TODO kontynuowac

            //var fallDistance = nbtFile.RootTag.Get<NbtCompound>("Player")!.Get<NbtFloat>("FallDistance");
           // if (fallDistance.Value > 0)
            //{
                //zmienic na 0 i ustawic blok pod graczem
           // }

            nbtChunkFile.SaveToFile(chunkPath, NbtCompression.GZip);
            return 0;
        }

        public int SetHealth(int worldId, short hp)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldId}", "level.dat");
            NbtFile nbtFile = _fileService.ReadDatFile(path);
            var healthTag = nbtFile.RootTag.Get<NbtCompound>("Data")!.Get<NbtCompound>("Player")!.Get<NbtShort>("Health");
            healthTag!.Value = hp;

            nbtFile.SaveToFile(path, NbtCompression.GZip);
            return 0;
        }

        public int SetSpawnPoint(int worldId, int x, int y, int z, bool safe)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldId}", "level.dat");
            NbtFile nbtFile = _fileService.ReadDatFile(path);

            var dataTag = nbtFile.RootTag.Get<NbtCompound>("Data");
            if(safe && y > 126)
            {
                Console.WriteLine("Unable to safely change the spawnpoint.");
                return 1;
            }
            if (safe && (_chunkService.CheckIfBlock(worldId, x, y + 1, z) || _chunkService.CheckIfBlock(worldId, x, y + 2, z)))//poprawic zeby CheckIfBlock nie robil exception przy y>128 (jak dam y=200000 to wywala byte) - niby poprawione, trzeba potestować
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
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldId}", "level.dat");
            NbtFile nbtFile = _fileService.ReadDatFile(path);

            var posTag = nbtFile.RootTag.Get<NbtCompound>("Data")!.Get<NbtCompound>("Player")!.Get<NbtList>("Pos");
            posTag!.Clear();
            if (safe && (_chunkService.CheckIfBlock(worldId, x, y, z) || y < 1 || _chunkService.CheckIfBlock(worldId, x, y - 1, z)))
            {
                Console.WriteLine("Unable to safely change the player's position.");
                return 1;
            }
            posTag.Add(new NbtDouble() { Value = x + (x >= 0 ? 0.5 : -0.5) });
            posTag.Add(new NbtDouble() { Value = y + 0.7 });
            posTag.Add(new NbtDouble() { Value = z + (z >= 0 ? 0.5 : -0.5) });

            nbtFile.SaveToFile(path, NbtCompression.GZip);
            return 0;
        }

        public int ReadSpawn(int worldId)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldId}", "level.dat");
            NbtFile nbtFile = _fileService.ReadDatFile(path);
            var dataTag = nbtFile.RootTag.Get<NbtCompound>("Data");
            var xSpawn = dataTag!.Get<NbtInt>("SpawnX")!.IntValue;
            var ySpawn = dataTag!.Get<NbtInt>("SpawnY")!.IntValue;
            var zSpawn = dataTag!.Get<NbtInt>("SpawnZ")!.IntValue;
            Console.WriteLine($"X: {xSpawn}\nY: {ySpawn}\nZ: {zSpawn}");
            return 0;
        }
    }
}
