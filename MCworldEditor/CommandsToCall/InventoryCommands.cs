using fNbt;

namespace MCworldEditor.CommandsToCall
{
    public class InventoryCommands
    {
        private DatHelper _datHelper;

        public InventoryCommands(DatHelper datHelper)
        {
            _datHelper = datHelper;
        }

        public int AddItemToInventory(int worldNumber, int itemId, int count)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldNumber}", "level.dat");
            NbtFile nbtFile = _datHelper.ReadDatFile(path);

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

            while(count>0 && tab.Count != 0)
            {
                byte byteCount = count > 64 ? (byte)64 : (byte)count;
                count-=byteCount;

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

        public int ClearInventory(int worldNumber)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldNumber}", "level.dat");
            NbtFile nbtFile = _datHelper.ReadDatFile(path);

            var allTags = nbtFile.RootTag.Names.ToList();
            var dataTag = nbtFile.RootTag.Get<NbtCompound>("Data");
            var playerTag = dataTag!.Get<NbtCompound>("Player");
            var inventoryData = playerTag!.Get<NbtList>("Inventory");

            inventoryData!.Clear();
            nbtFile.SaveToFile(path, NbtCompression.GZip);

            return 0;
        }
    }
}
