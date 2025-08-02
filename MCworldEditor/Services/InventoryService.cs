using fNbt;
using MCworldEditor.Services.Interfaces;

namespace MCworldEditor.Services
{
    public class InventoryService : IInventoryService
    {
        private IFileService _fileService;

        public InventoryService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public int AddItemToInventory(int worldId, int itemId, int count)
        {
            string path = _fileService.GetLevelDatPath(worldId);
            NbtFile nbtFile = _fileService.ReadDatFile(path);
            var allTags = nbtFile.RootTag.Names.ToList();
            var dataTag = nbtFile.RootTag.Get<NbtCompound>("Data");
            var playerTag = dataTag!.Get<NbtCompound>("Player");
            var inventoryData = playerTag!.Get<NbtList>("Inventory");

            if (inventoryData!.ListType != NbtTagType.Compound && inventoryData.Count == 0)
            {
                var newInventory = new NbtList("Inventory", NbtTagType.Compound);
                playerTag.Remove(inventoryData);
                playerTag.Add(newInventory);
                inventoryData = newInventory;
            }

            List<byte> tab = Enumerable.Range(0, 36).Select(i => (byte)i).ToList();

            foreach (NbtCompound x in inventoryData!)
                tab.Remove(x.Get<NbtByte>("Slot")!.ByteValue);

            if (tab.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Inventory is full.");
                Console.ResetColor();
                return 1;
            }

            while (count > 0 && tab.Count != 0)
            {
                byte byteCount = count > 64 ? (byte)64 : (byte)count;
                count -= byteCount;

                var newItem = new NbtCompound()
                {
                    new NbtByte("Slot", tab[0]),
                    new NbtShort("id", (short)itemId),
                    new NbtByte("Count", byteCount),
                    new NbtShort("Damage", 0)
                };

                tab.Remove(tab[0]);
                inventoryData.Add(newItem);
            }

            nbtFile.SaveToFile(path, NbtCompression.GZip);
            return 0;
        }

        public int CleanInventory(int worldId)
        {
            string path = _fileService.GetLevelDatPath(worldId);
            NbtFile nbtFile = _fileService.ReadDatFile(path);

            var allTags = nbtFile.RootTag.Names.ToList();
            var dataTag = nbtFile.RootTag.Get<NbtCompound>("Data");
            var playerTag = dataTag!.Get<NbtCompound>("Player");
            var inventoryData = playerTag!.Get<NbtList>("Inventory");

            inventoryData!.Clear();
            nbtFile.SaveToFile(path, NbtCompression.GZip);
            return 0;
        }

        public List<(int count, short id)> ReadItemsInInventory(int worldId)
        {
            string path = _fileService.GetLevelDatPath(worldId);
            NbtFile nbtFile = _fileService.ReadDatFile(path);

            var allTags = nbtFile.RootTag.Names.ToList();
            var dataTag = nbtFile.RootTag.Get<NbtCompound>("Data");
            var playerTag = dataTag!.Get<NbtCompound>("Player");
            var inventoryData = playerTag!.Get<NbtList>("Inventory");
            Dictionary<short, int> itemCounts = new();

            foreach (NbtCompound itemTag in inventoryData!)
            {
                short id = itemTag.Get<NbtShort>("id")!.Value;
                byte count = itemTag.Get<NbtByte>("Count")!.Value;

                if (itemCounts.ContainsKey(id))
                    itemCounts[id] += count;
                else
                    itemCounts[id] = count;
            }

            return itemCounts.Select(x => (x.Value, x.Key)).ToList();
        }
    }
}
