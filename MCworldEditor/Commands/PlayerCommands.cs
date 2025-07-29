using fNbt;
using MCworldEditor.Services.Interfaces;

namespace MCworldEditor.CommandsToCall
{
    public class PlayerCommands
    {
        private IPlayerPositionService _playerPositionService;
        private IFileService _fileService;
        private IChunkService _chunkService;
        private ISpawnService _spawnService;
        private IHealthService _healthService;

        public PlayerCommands(IPlayerPositionService playerPositionService, IFileService fileService, IChunkService chunkService, ISpawnService spawnService, IHealthService healthService)
        {
            _playerPositionService = playerPositionService;
            _fileService = fileService;
            _chunkService = chunkService;
            _spawnService = spawnService;
            _healthService = healthService;
        }
        //komende na falldistance, komende na uratowanie - ustawienie hp na max, sprawdzenie czy nie jest w lawie, usuniecie potworow dookola etc.
        public int SaveFromDying(int worldId)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldId}", "level.dat");
            
           

            SetHealth(worldId, 100);

            var positionInt = _playerPositionService.GetPlayerPositionInt(worldId);

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
            int response = _healthService.SetHealth(worldId, hp);
            return response;
        }

        public int SetSpawnPoint(int worldId, int x, int y, int z, bool safe)
        {
            int response = _spawnService.SetSpawnPoint(worldId, x, y, z, safe);
            return response;
        }

        public int ReadPosition(int worldId, bool specific)
        {
            var positionDouble = _playerPositionService.GetPlayerPositionDouble(worldId);
            if (specific)
                Console.WriteLine($"X: {positionDouble.X}\nY: {positionDouble.Y}\nZ: {positionDouble.Z}");
            else
                Console.WriteLine($"X: {(int)positionDouble.X}\nY: {(int)positionDouble.Y}\nZ: {(int)positionDouble.Z}");
            return 0;
        }

        public int SetPlayerPosition(int worldId, int x, int y, int z, bool safe)
        {
            if (safe && (_chunkService.CheckIfBlock(worldId, x, y, z) || y < 1 || _chunkService.CheckIfBlock(worldId, x, y - 1, z)))
            {
                Console.WriteLine("Unable to safely change the player's position.");
                return 1;
            }
            var response = _playerPositionService.SetPlayerPosition(worldId, x, y, z);
            return response;
        }

        public int ReadSpawnPoint(int worldId)
        {
            var spawnpoint = _spawnService.ReadSpawnPoint(worldId);
            Console.WriteLine($"X: {spawnpoint.x}\nY: {spawnpoint.y}\nZ: {spawnpoint.z}");
            return 0;
        }
    }
}
