using fNbt;
using MCworldEditor.Services.Interfaces;

namespace MCworldEditor.Services
{
    public class MobService : IMobService
    {
        private IPlayerPositionService _playerPositionService;
        private IFileService _fileService;

        private string[] passiveMobs = [ "Sheep", "Cow", "Chicken", "Pig", "Squid" ];
        private string[] hostileMobs = [ "Creeper", "Skeleton", "Zombie", "Spider", "Giant", "Ghast" ];

        public MobService(IPlayerPositionService playerPositionService, IFileService fileService)
        {
            _playerPositionService = playerPositionService;
            _fileService = fileService;
        }

        public int RemoveMobs(int worldOption, bool passive, bool hostile, int? area)
        {
            int trueArea = area ?? 0;

            var playerPosition = _playerPositionService.GetPlayerPositionInt(worldOption);

            int offsetX = playerPosition.X - (16 * trueArea), offetZ = playerPosition.Z - (16 * trueArea);

            for(int i = 0; i < trueArea*2+1; i++)
            {
                for(int j=0; j < trueArea*2+1; j++)
                {
                    string currentPath = _fileService.GetChunkPathByCoordinates(worldOption, offsetX + i * 16, playerPosition.Y, offetZ + j * 16);

                    if (!File.Exists(currentPath)) continue;

                    NbtFile nbtFile = _fileService.ReadDatFile(currentPath);

                    var level = nbtFile.RootTag.Get<NbtCompound>("Level")!;
                    var entities = level.Get<NbtList>("Entities")!;

                    if (passive)
                    {
                        var passiveEntities = entities.OfType<NbtCompound>().Where(x => passiveMobs.Contains(x.Get<NbtString>("id")!.Value)).ToList();
                        foreach (var entity in passiveEntities)
                            entities.Remove(entity);
                    }

                    if (hostile)
                    {
                        var hostileEntities = entities.OfType<NbtCompound>().Where(x => hostileMobs.Contains(x.Get<NbtString>("id")!.Value)).ToList();
                        foreach (var entity in hostileEntities)
                            entities.Remove(entity);
                    }

                    nbtFile.SaveToFile(currentPath, NbtCompression.GZip);
                }
            }

            return 0;
        }

        public int SpawnMob(int worldOption, string mobId, int? x, int? y, int? z, int? hp, int? count)
        {
            if (!passiveMobs.Contains(mobId) && !hostileMobs.Contains(mobId)) return 1;

            var playerPosition = _playerPositionService.GetPlayerPositionInt(worldOption);

            x = x ?? playerPosition.X;
            y = y ?? playerPosition.Y;
            z = z ?? playerPosition.Z;

            hp = hp ?? 10;

            count = count ?? 1;

            string path = _fileService.GetChunkPathByCoordinates(worldOption, (int)x, (int)y, (int)z);
            NbtFile nbtFile = _fileService.ReadDatFile(path);

            var level = nbtFile.RootTag.Get<NbtCompound>("Level")!;
            var entities = level.Get<NbtList>("Entities");

            if (entities is null || entities.ListType != NbtTagType.Compound)
            {
                NbtList newEntities = new("Entities", NbtTagType.Compound);

                if (entities is not null)
                    foreach (var entity in entities)
                        if (entity is NbtCompound c)
                            newEntities.Add((NbtTag)c.Clone());
                
                level["Entities"] = newEntities;
                entities = newEntities;
            }

            NbtCompound compound = [
                new NbtList("Motion", NbtTagType.Double) { new NbtDouble(0), new NbtDouble(0), new NbtDouble(0) },
                new NbtShort("Health", 10),
                new NbtShort("Air", 300),
                new NbtByte("OnGround", 1),
                new NbtList("Rotation", NbtTagType.Float) { new NbtFloat(0), new NbtFloat(0) },
                new NbtFloat("FallDistance", 0),
                new NbtList("Pos", NbtTagType.Double) { new NbtDouble((double)x), new NbtDouble((double)y), new NbtDouble((double)z) },
                new NbtShort("DeathTime", 0),
                new NbtShort("Fire", -1),
                new NbtString("id", mobId),
                new NbtShort("HurtTime", 0),
                new NbtShort("AttackTime", 0)
            ];

            // sheep-only
            if(mobId == "Sheep")
                compound.AddRange([new NbtByte("Sheared", 0), new NbtByte("Color", 0)]);

            for (int i = 0; i < count; i++)
                entities.Add((NbtCompound)compound.Clone());

            nbtFile.SaveToFile(path, NbtCompression.GZip);

            return 0;
        }
    }
}
