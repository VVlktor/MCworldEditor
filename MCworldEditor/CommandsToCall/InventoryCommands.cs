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

        public int AddItemToInventory(int worldNumber, int itemId, int count)//TODO: dodac petle zeby mozna bylo dodawac wiecej niz 127 itemow na raz
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldNumber}", "level.dat");
            NbtFile nbtFile = _datHelper.ReadDatFile(path);

            var allTags = nbtFile.RootTag.Names.ToList();
            var dataTag = nbtFile.RootTag.Get<NbtCompound>("Data");
            var playerTag = dataTag!.Get<NbtCompound>("Player");
            var inventoryData = playerTag!.Get<NbtList>("Inventory");

            List<byte> tab = new();
            foreach (NbtCompound x in inventoryData!)
                tab.Add(x.Get<NbtByte>("Slot")!.ByteValue);

            if (tab.Count >= 36)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Inventory is full.");
                Console.ResetColor();
                return 1;
            }

            byte slot = 0;

            while (slot < 36)
            {
                if (!tab.Contains(slot))
                    break;
                slot++;
            }

            var newItem = new NbtCompound()
            {
                new NbtByte("Slot", slot),
                new NbtShort("id", (byte)itemId),
                new NbtByte("Count", (byte)count),
                new NbtShort("Damage", 0)
            };

            inventoryData.Add(newItem);
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
